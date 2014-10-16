using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSMSL;
using CSMSL.IO.OMSSA;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using IMP.PhosphoRS;
using CSMSL.Util;

namespace Coon.Compass.PhosphoRS
{
    public class SampleDataSource : ThreadManagement.IDataConection, IDisposable
    {
        readonly AminoAcidModification _modificationToLocalize;
        int _currentNr;
        private double masstolerance = 0.35;
        private readonly int maxIsoformCount = 500;
        private readonly int maxPTMCount = 20;
        private const int maxPackageSize = 2000;
        private readonly string neutralLoss = "automatic";

        private readonly SQLiteCommand _selectPsms;
        private readonly SQLiteCommand _selectPsm;
        private readonly SQLiteCommand _selectIsoform;
        private readonly SQLiteCommand _insertIsoform;
        private readonly List<Modification> _fixedModifications; 
        private SQLiteConnection _dbConnection;
        public ThreadManagement PhosphoRS;

        public int TotalPsms { get; private set; }
        public string BaseName { get; private set; }

        public event EventHandler<ProgressEvent> OnProgress;

        public event EventHandler<MessageEvent> OnMessage;

        protected void OnProgressUpdate(double percent)
        {
            var handler = OnProgress;
            if (handler != null)
            {
                handler(this, new ProgressEvent(percent));
            }
        }

        protected void OnMessageUpdate(string message)
        {
            var handler = OnMessage;
            if (handler != null)
            {
                handler(this, new MessageEvent(message));
            }
        }
        
        public SampleDataSource(AminoAcidModification modificationToLocalize, IEnumerable<Modification> fixedMods, string filePath, string baseName, string neutralloss = "automatic", double massTolerance = 0.35, int maxIsoforms = 500, int maxPTMs = 20)
        {
            masstolerance = massTolerance;
            maxIsoformCount = maxIsoforms;
            maxPTMCount = maxPTMs;
            BaseName = baseName;
            neutralLoss = neutralloss;
            _fixedModifications = fixedMods.ToList();
            _dbConnection = new SQLiteConnection(@"Data Source=" + filePath + ";Version=3;Journal Mode=Off;");
          
            _dbConnection.Open();
            var countPsms = new SQLiteCommand(@"SELECT count(id) FROM psms", _dbConnection);
            _selectPsms = new SQLiteCommand(@"SELECT * FROM psms LIMIT @offset, @limit", _dbConnection);
            _selectPsm = new SQLiteCommand(@"SELECT * FROM psms WHERE id = @id LIMIT 1", _dbConnection);
            _selectIsoform = new SQLiteCommand(@"SELECT * FROM isoforms WHERE id = @id LIMIT 1", _dbConnection);
            _insertIsoform = new SQLiteCommand(@"INSERT INTO isoforms (id, psmID, sequence, sites) VALUES (@id, @psmID, @sequence, @sites)", _dbConnection);
            TotalPsms = (int)(long)countPsms.ExecuteScalar();
            _modificationToLocalize = modificationToLocalize;
            _psms = new Dictionary<int, ExtendedPeptideSpectrumMatch>();
        }

        private readonly BlockingCollection<ThreadManagement.progressMessage> _progressMessageQueue =
            new BlockingCollection<ThreadManagement.progressMessage>(new ConcurrentQueue<ThreadManagement.progressMessage>());
        
        public List<ThreadManagement.SpectraPackageItem> GetNewDataPackage(int maxSizeOfPackage, out int numberOfSpectraPacked, out int numberOfPeptidesPacked)
        {
            numberOfSpectraPacked = 0;
            numberOfPeptidesPacked = 0;
            if (_currentNr >= TotalPsms)
                return null;

            List<ThreadManagement.SpectraPackageItem> package = new List<ThreadManagement.SpectraPackageItem>();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                foreach (ExtendedPeptideSpectrumMatch psm in GetPsms(_currentNr, maxPackageSize))
                {
                    var modLocations = psm.Sequence.GetPositionsOf(_modificationToLocalize.TargetAminoAcids, true).Select(n => n - 1).ToArray(); // need to be 0-based
                    int ptms = psm.Sequence.GetPositionsOf(_modificationToLocalize).Count;
                    var id2ModMap = new List<Tuple<int, List<int>>>();
                    foreach (int[] combo in Combinatorics.Combinations(modLocations, ptms))
                    {
                        var tuple = psm.AddIsoform(combo);
                        _insertIsoform.Parameters.AddWithValue("@id", tuple.Item1);
                        _insertIsoform.Parameters.AddWithValue("@psmID", psm.ID);
                        _insertIsoform.Parameters.AddWithValue("@sequence", ExtendedPeptideSpectrumMatch.ConvertString(psm.sequenceString, combo));

                        StringBuilder siteSB = new StringBuilder();
                        foreach (int pos in combo.OrderBy(i => i))
                        {
                            char c = psm.sequenceString[pos];
                            siteSB.Append(c);
                            siteSB.Append('(');
                            siteSB.Append(pos + psm.StartResidue);
                            siteSB.Append(')');
                            siteSB.Append(';');
                        }
                        if (siteSB.Length > 1)
                            siteSB.Remove(siteSB.Length - 1, 1);

                        _insertIsoform.Parameters.AddWithValue("@sites", siteSB.ToString());
                        _insertIsoform.ExecuteNonQuery();
                        id2ModMap.Add(tuple);
                    }
                    package.Add(new ThreadManagement.SpectraPackageItem(psm, 1.0/TotalPsms, id2ModMap));
                }
                transaction.Commit();
            }

            _currentNr += package.Count;
            numberOfSpectraPacked = package.Count;
            numberOfPeptidesPacked = package.Count;

            return package;
        }

        private IEnumerable<ExtendedPeptideSpectrumMatch> GetPsms(int start, int count)
        {
            _selectPsms.Parameters.AddWithValue("@offset", start);
            _selectPsms.Parameters.AddWithValue("@limit", count);
            List<ExtendedPeptideSpectrumMatch> psms = new List<ExtendedPeptideSpectrumMatch>();
            using (var reader = _selectPsms.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)(long)reader["id"];
                    string seq = reader["sequence"].ToString();
                    Peptide peptide = new Peptide(seq);
                    peptide.SetModifications(_fixedModifications);
                    string modLine = reader["modLine"].ToString();
                    peptide.SetModifications(modLine);
                    int charge = (int)reader["charge"];
                    SpectrumType type = (SpectrumType)reader["dissType"];
                    byte[] spectrumBytes = (byte[])reader["spectrum"];
                    MZSpectrum spectrum = new MZSpectrum(spectrumBytes);

                    var psm = new ExtendedPeptideSpectrumMatch(id, peptide, spectrum, charge, type) {StartResidue = (int) reader["startResidue"]};
                    psms.Add(psm);
                }
            }
            return psms;
        }

        public void Dispose()
        {
            _progressMessageQueue.Dispose();
            _dbConnection.Close();
            _dbConnection.Dispose();
            _selectPsms.Dispose();
            _selectIsoform.Dispose();
            _selectPsm.Dispose();
            _insertIsoform.Dispose();
            _dbConnection = null;
        }

        public BlockingCollection<ThreadManagement.progressMessage> GetProgressMessageQueue()
        {
            return _progressMessageQueue;
        }
        
        public void Start(CancellationTokenSource searchCancel)
        {
            _currentNr = 0;

            Task.Factory.StartNew(() =>
            {
                double lastProgress = -1;
                while (!_progressMessageQueue.IsCompleted)
                {
                    if (_progressMessageQueue.Count > 0)
                    {
                        ThreadManagement.progressMessage msg = _progressMessageQueue.Take();
                        if (msg.type == ThreadManagement.progressMessage.typeOfMessage.stringMessage)
                        {
                            if (msg.message != null)
                            {
                                OnMessageUpdate(msg.message);
                            }
                        }
                        else
                        {
                            //spectraProcessed is not a integer. 
                            //see GetNewDataPackage. 
                            //after each spetra this funtion will be called (asynchronously). 
                            var currentProgress = msg.spectraProcessed;
                            if (currentProgress >= lastProgress + 0.005)
                            {
                                lastProgress = currentProgress;
                                OnProgressUpdate(currentProgress);
                            }
                        }
                    }
                }
            }, searchCancel.Token);

            PhosphoRS = new ThreadManagement(this, searchCancel, maxIsoformCount, maxPTMCount, neutralLoss, masstolerance, _modificationToLocalize, TotalPsms);
            
            PhosphoRS.StartPTMLocalisation();
        }

        readonly Dictionary<int, ExtendedPeptideSpectrumMatch> _psms; 

        public ExtendedPeptideSpectrumMatch GetPsm(int peptideID)
        {
            int psmID = ExtendedPeptideSpectrumMatch.GetPsm(peptideID);

            ExtendedPeptideSpectrumMatch psm;
            if (_psms.TryGetValue(psmID, out psm))
            {
                return psm;
            }

            _selectPsm.Parameters.AddWithValue("@id", psmID);
            using (var reader = _selectPsm.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)(long)reader["id"];
                    string seq = reader["sequence"].ToString();
                    Peptide peptide = new Peptide(seq);
                    peptide.SetModifications(_fixedModifications);
                    string modLine = reader["modLine"].ToString();
                    peptide.SetModifications(modLine);
                    int charge = (int)reader["charge"];
                    SpectrumType type = (SpectrumType)reader["dissType"];
                    //byte[] spectrumBytes = (byte[])reader["spectrum"];
                    //MZSpectrum spectrum = new MZSpectrum(spectrumBytes);
                    psm = new ExtendedPeptideSpectrumMatch(id, peptide, null, charge, type);
                    psm.Line = reader["lineData"].ToString();
                    psm.StartResidue = (int)reader["startResidue"];
                    psm.ProteinGroup = reader["proteinGroup"].ToString();
                    psm.Defline = reader["defline"].ToString();
                    byte[] data = reader["quantData"] as byte[];
                    if (data != null)
                    {
                        psm.QuantData = data.GetDoubles();
                    }
                    _psms.Add(psmID, psm);
                    return psm;
                }
            }
            return null;
        }

        public Tuple<string, string> GetIsoform(int peptideID)
        {
            _selectIsoform.Parameters.AddWithValue("@id", peptideID);
            using (var reader = _selectIsoform.ExecuteReader())
            {
                while (reader.Read())
                {
                    //int id = (int)(long)reader["id"];
                    string seq = reader["sequence"].ToString();
                    string sites = reader["sites"].ToString();
                    return new Tuple<string, string>(seq, sites);
                }
            }
            return null;
        }
    }
}
