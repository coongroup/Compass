using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using IMP.PhosphoRS;
using LumenWorks.Framework.IO.Csv;
using CSMSL.IO.OMSSA;

namespace Coon.Compass.PhosphoRS
{
    public partial class PhosphoRsForm : Form
    {
        public Dictionary<IMass, AminoAcidModification> ModificationDictionary;
        public List<Modification> FixedModifications; 
        public static char modID = 'A';
        public Modification Phosphorylation = new ChemicalFormulaModification("H3PO3", "Phosphorylation", ModificationSites.S | ModificationSites.T | ModificationSites.Y);
        public AminoAcidModification phospho = new AminoAcidModification(modID++, "Phosphorylation", "Pho", "H3PO4", 80, 98, AminoAcidSequence.ParseAASequence("STY"));
        public string HeaderLine;
        public string QuantHeaderLine;
        public int QuantColumns;
        public bool IncludeQuant = false;
        private string _tempDbFile;
        private CancellationTokenSource _cancellationToken;
        private Task _mainTask;

        public PhosphoRsForm()
        {
            InitializeComponent();
            SetTitle();
            ExtendedPeptideSpectrumMatch.AddModification(Phosphorylation, phospho);
            OmssaModification.GroupedModifications["phosphorylation of S with neutral loss"] = Phosphorylation;
            OmssaModification.GroupedModifications["phosphorylation of S"] = Phosphorylation;
            OmssaModification.GroupedModifications["phosphorylation of T with neutral loss"] = Phosphorylation;
            OmssaModification.GroupedModifications["phosphorylation of T"] = Phosphorylation;
            OmssaModification.GroupedModifications["phosphorylation of Y with neutral loss"] = Phosphorylation;
            OmssaModification.GroupedModifications["phosphorylation of Y"] = Phosphorylation;

            ResetModifications();
        }

        public void ResetModifications()
        {
            checkedListBox1.DataSource = OmssaModification.GetAllModifications().ToList();

            OmssaModification CAM;
            OmssaModification.TryGetModification(3, out CAM);

            int index = checkedListBox1.Items.IndexOf(CAM);
            checkedListBox1.SetItemChecked(index, true);
        }

        private void Start()
        {
            string parisomonyPeptidesFile = textBox1.Text;
            string baseName = Path.GetFileNameWithoutExtension(parisomonyPeptidesFile);
            string rawFileDirectory = textBox2.Text;
            string outputDirectory = textBox3.Text;
            double massTolerance = (double) numericUpDown1.Value;
            double localizationPercent = (double) numericUpDown2.Value/100.0;
            int maxIsoforms = (int) numericUpDown3.Value;
            int maxPtms = (int) numericUpDown4.Value;
            string neutralLoss = comboBox1.SelectedText;

            var modificationToLocalize = phosphoRB.Checked ? phospho : null;

            FixedModifications = new List<Modification>();
            foreach (Modification mod in checkedListBox1.CheckedItems.OfType<Modification>())
            {
                FixedModifications.Add(mod);
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
            
            button1.Text = "Cancel";
            button1.Click -= button1_Click;
            button1.Click += cancelButton;
            richTextBox1.Clear();
            _cancellationToken = new CancellationTokenSource();

            _tempDbFile = Path.Combine(outputDirectory, string.Format("{0}.sqlite", DateTime.Now.ToFileTime()));
            _mainTask = Task.Factory.StartNew(() =>
            {
                LoadPsms(parisomonyPeptidesFile, rawFileDirectory, modificationToLocalize, _tempDbFile, _cancellationToken.Token);
                using (var source = new SampleDataSource(modificationToLocalize, FixedModifications, _tempDbFile, baseName, neutralLoss, massTolerance, maxIsoforms, maxPtms))
                {
                    source.OnProgress += (sender, @event) => UpdateProgress(@event.Percent);
                    source.OnMessage += (sender, @event) => SetMessage(@event.Message);
                    source.Start(_cancellationToken);
                    WriteResults(source, outputDirectory, localizationPercent);
                }
            }, _cancellationToken.Token).ContinueWith((result) =>
            {
                button1.Text = "Localize";
                button1.Click += button1_Click;
                button1.Click -= cancelButton;
                UpdateProgress(0.0);
                _cancellationToken = null;
                SQLiteConnection.ClearAllPools();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (File.Exists(_tempDbFile))
                    File.Delete(_tempDbFile);
                SetTitle();
                SetMessage("Finished");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void cancelButton(object sender, EventArgs e)
        {
            CancelJob();
        }

        public void UpdateProgress(double percent)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<double>(UpdateProgress), percent);
                return;
            }
            
            progressBar1.Value = (int)(progressBar1.Maximum * percent);

            SetTitle((percent * 100.0).ToString("F1") + "%");
        }

        public void SetMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(SetMessage), message);
                return;
            }

            richTextBox1.AppendText(string.Format("[{0}]\t{1}\n", DateTime.Now.ToLongTimeString(), message));
            richTextBox1.ScrollToCaret();
        }

        readonly Regex proteinResidueRegex = new Regex(@"\((\d+)\)", RegexOptions.Compiled);

        public void WriteResults(SampleDataSource source, string outputDirectory, double localizeCutoff = 0.75)
        {
            const string extraHeader = ",Number of Isoforms,Localized?,Localized Sequence,Localized Site(s),Localized Percent (%),Localized Score,Residue Probabilities,Protein Residue Probabilties";
            int psms = 0;
            int localized = 0;

            Dictionary<string, List<int>> groupings = new Dictionary<string, List<int>>();

            string baseName = Path.Combine(outputDirectory, source.BaseName);

            SetMessage("Writing Results to " + outputDirectory + "...");

            // PSMs
            using (StreamWriter localizedWriter = new StreamWriter(baseName + "_psms_localized.csv"))
            using (StreamWriter allWriter = new StreamWriter(baseName + "_psms_all.csv"))
            {
                // Write Headers
                localizedWriter.Write(HeaderLine);
                localizedWriter.WriteLine(extraHeader);
                allWriter.Write(HeaderLine);
                allWriter.WriteLine(extraHeader);

                // Write Psms
                StringBuilder sb = new StringBuilder();
                foreach (var isoformGroup in source.PhosphoRS.PTMResult.IsoformGroupList.OrderBy(g => g.SequenceString))
                {
                    if (isoformGroup.Error)
                    {
                        SetMessage("Error -> " + isoformGroup.Message);
                        continue;
                    }
                    var bestPeptide = isoformGroup.Peptides.OrderByDescending(pep => pep.Score).First();
                    int isoforms = isoformGroup.Peptides.Count;

                    bool isLocalized = bestPeptide.Probability >= localizeCutoff;
                    var tuple = source.GetIsoform(bestPeptide.ID);
                    string sequence = tuple.Item1;
                    string sites = tuple.Item2;

                    ExtendedPeptideSpectrumMatch psm = source.GetPsm(bestPeptide.ID);
                    int startResidue = psm.StartResidue;
                    string siteScores = isoformGroup.GetSiteProbabilityString();
                    string globalSiteScores = proteinResidueRegex.Replace(siteScores, (match) =>
                    {
                        int v = int.Parse(match.Groups[1].Value);
                        return "(" + (v + startResidue - 1) + ")";
                    });

                    sb.Clear();
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5:F2},{6},{7},{8}", psm.Line, isoforms, isLocalized, sequence,sites, bestPeptide.Probability*100.0, bestPeptide.Score, siteScores, globalSiteScores);
   
                    string key = psm.ProteinGroup + "|" + sites;
                    List<int> ids;
                    if (!groupings.TryGetValue(key, out ids))
                    {
                        ids = new List<int>() { bestPeptide.ID };
                        groupings.Add(key, ids);
                    }
                    else
                    {
                        ids.Add(bestPeptide.ID);
                    }
                    
                    string line = sb.ToString();

                    if (isLocalized)
                    {
                        localized++;
                        localizedWriter.WriteLine(line);
                    }
                    allWriter.WriteLine(line);
                    psms++;
                }
            }

            Dictionary<string, Tuple<string, string, int, int>> proteinGroupings = new Dictionary<string, Tuple<string, string, int, int>>();

            // Isoforms
            using (StreamWriter isoformLocalizedWriter = new StreamWriter(baseName + "_isoforms_localized.csv"))
            using (StreamWriter isoformAllWriter = new StreamWriter(baseName + "_isoforms_all.csv"))
            {
                string header = "Protein Group,Defline,Isoform,# of Sites,PSMs Identified,PSMs Localized,Localized?,Max Localization Probability (%)";
                isoformLocalizedWriter.Write(header);
                if (IncludeQuant)
                {
                    isoformLocalizedWriter.WriteLine("," + QuantHeaderLine);
                }
                else
                {
                    isoformLocalizedWriter.WriteLine();
                }
                isoformAllWriter.WriteLine(header);
                foreach (var ids in groupings.Values)
                {
                    ExtendedPeptideSpectrumMatch psm;
                    int count = 0;
                    int localizedCount = 0;
                    int bestId = -1;
                    double bestProb = 0;
                    double[] quant = new double[QuantColumns];
                    foreach (int id in ids)
                    {
                        count++;
                        double prob = source.PhosphoRS.PTMResult.PeptideIdPrsProbabilityMap[id];
                        if(prob > bestProb) {
                            bestProb = prob;
                            bestId = id;
                        }

                        if (prob >= localizeCutoff)
                        {
                            psm = source.GetPsm(id);
                            localizedCount++;
                            if (IncludeQuant)
                            {
                                for (int i = 0; i < QuantColumns; i++)
                                {
                                    quant[i] += psm.QuantData[i];
                                }
                            }
                        }
                    }

                    psm = source.GetPsm(bestId);
                    var tuple = source.GetIsoform(bestId);
                    string sites = tuple.Item2;
                    bool isLocalized = localizedCount > 0;

                    if (isLocalized)
                    {
                        Tuple<string, string, int, int> tuple2;
                        if (!proteinGroupings.TryGetValue(psm.ProteinGroup, out tuple2))
                        {
                            tuple2 = new Tuple<string, string, int, int>(psm.Defline, sites, 1, localizedCount);
                            proteinGroupings.Add(psm.ProteinGroup, tuple2);
                        }
                        else
                        {
                            proteinGroupings[psm.ProteinGroup] = new Tuple<string, string, int, int>(tuple2.Item1, tuple2.Item2 + "|" + sites, tuple2.Item3 + 1, tuple2.Item4 + localizedCount);
                        }
                    }
                    
                    string line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7:F2}", psm.ProteinGroup, psm.Defline, sites, sites.Count(c => c.Equals(';')) + 1, count, localizedCount, isLocalized, bestProb * 100.0);

                    if (isLocalized)
                    {
                        isoformLocalizedWriter.Write(line);
                        if (IncludeQuant)
                        {
                            isoformLocalizedWriter.WriteLine("," + string.Join(",", quant));
                        }
                        else
                        {
                            isoformLocalizedWriter.WriteLine();
                        }

                    }
                    isoformAllWriter.WriteLine(line);
                }
            }

            using (StreamWriter proteinWriter = new StreamWriter(baseName + "_isoforms_localized_reduced.csv"))
            {
                const string header = "Protein Group,Defline,Isoforms,# of Localized Isoforms,# of Localized PSMs";
                proteinWriter.WriteLine(header);

                foreach (var ids in proteinGroupings)
                {
                    proteinWriter.WriteLine("{0},{1},{2},{3},{4}", ids.Key, ids.Value.Item1, ids.Value.Item2, ids.Value.Item3, ids.Value.Item4);
                }
            }

        }

        private void LoadPsms(string filePath, string rawFileDirectory, AminoAcidModification modToLocalize, string tempFile, CancellationToken cancelToken)
        {
            SetMessage("Loading PSMs from " + filePath + "...");
            var rawFilePaths = Directory.EnumerateFiles(rawFileDirectory, "*.raw").ToDictionary(Path.GetFileNameWithoutExtension, p => new ThermoRawFile(p));

            var openRawFiles = new Dictionary<string, ThermoRawFile>();

            double lastProgress = 0;
            using (var dbConnection = new SQLiteConnection(@"Data Source=" + tempFile+";Version=3;Journal Mode=Off;"))
            {
                try
                {
                    dbConnection.Open();
                } catch(Exception e) {
                    SetMessage("Error creating database: " + e.Message);
                }

                var sql = @"CREATE TABLE IF NOT EXISTS files (
                        id INTEGER PRIMARY KEY ASC, 
                        filePath TEXT UNIQUE ON CONFLICT IGNORE)";
                var cmd = new SQLiteCommand(sql, dbConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sql = @"CREATE TABLE IF NOT EXISTS psms (
                        id INTEGER PRIMARY KEY ASC, 
                        fileID INTEGER,
                        spectrumNumber INT, 
                        sequence TEXT,
                        startResidue INT,
                        charge INT,   
                        dissType INT,   
                        modLine TEXT,                       
                        spectrum BLOB,
                        lineData TEXT,
                        proteinGroup TEXT,
                        defline TEXT,
                        quantData BLOB,
                        FOREIGN KEY(fileID) REFERENCES files(id),
                        UNIQUE (fileID, spectrumNumber) ON CONFLICT IGNORE)";
                cmd = new SQLiteCommand(sql, dbConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                sql = @"CREATE TABLE IF NOT EXISTS isoforms (
                        id INTEGER PRIMARY KEY ASC, 
                        psmID INTEGER,                      
                        sequence TEXT,         
                        sites TEXT,             
                        FOREIGN KEY(psmID) REFERENCES psms(id))";
                cmd = new SQLiteCommand(sql, dbConnection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();

                using (var insertFile = new SQLiteCommand(@"INSERT INTO files (filePath) VALUES (@filePath)", dbConnection))
                using (var insertPsm = new SQLiteCommand(@"INSERT INTO psms (fileID, spectrumNumber, sequence, startResidue, charge, modLine, dissType, lineData, proteinGroup, spectrum, defline, quantData) VALUES ((SELECT id FROM files WHERE filePath = @filePath), @spectrumNumber, @sequence, @startResidue, @charge, @modLine, @dissType, @lineData, @proteinGroup, @spectrum, @defline, @quantData)", dbConnection))
                {
                    int lineNumber = 0;
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        StreamReader streamReader = new StreamReader(filePath);
                        using (CsvReader reader = new CsvReader(streamReader, true))
                        {
                            string[] data = new string[reader.FieldCount];
                            string[] headers = reader.GetFieldHeaders();
                            HeaderLine = headers.ToCsvString();

                            int channelsDetectedIndex = reader.GetFieldIndex("Channels Detected");
                            int firstQuantIndex = 0;
                            IncludeQuant = channelsDetectedIndex >= 0;

                            if (IncludeQuant)
                            {

                                int index = 0;
                                while (index < reader.FieldCount && !headers[index].Contains(" NL)"))
                                {
                                    index++;
                                }
                                firstQuantIndex = index;
                                QuantColumns = channelsDetectedIndex - firstQuantIndex;
                                QuantHeaderLine = headers.SubArray(firstQuantIndex, QuantColumns).ToCsvString();
                            }

                            while (reader.ReadNextRecord())
                            {
                                if (cancelToken.IsCancellationRequested)
                                {
                                    return;
                                }

                                int spectrumNumber = int.Parse(reader["Spectrum number"]);
                                string fileName = reader["Filename/id"].Split('.')[0];
                                string peptideSequence = reader["Peptide"].ToUpper();
                                string omssaModificationLine = reader["Mods"];
                                int charge = int.Parse(reader["Charge"]);
                                int startResidue = int.Parse(reader["Start"]);
                                string proteinGroup = reader["Best PG Name"];
                                string defline = reader["Defline"];

                                reader.CopyCurrentRecordTo(data);
                                string line = data.ToCsvString();

                                double[] quantData = null;
                                if (IncludeQuant)
                                {
                                    quantData = data.SubArray(firstQuantIndex, QuantColumns).Select(double.Parse).ToArray();
                                }

                                ThermoRawFile rawFile;
                                if (!openRawFiles.TryGetValue(fileName, out rawFile))
                                {
                                    if (!rawFilePaths.TryGetValue(fileName, out rawFile))
                                    {
                                        throw new ArgumentException("Cannot find Raw File " + fileName);
                                    }
                                    rawFile.Open();
                                    insertFile.Parameters.AddWithValue("@filePath", rawFile.FilePath);
                                    insertFile.ExecuteNonQuery();
                                    openRawFiles.Add(fileName, rawFile);
                                }

                             
                                Peptide peptide = new Peptide(peptideSequence);

                                // Set Fixed modifications
                                peptide.SetModifications(FixedModifications);

                                // Set Variable modifications
                                peptide.SetModifications(omssaModificationLine);

                                var aasequence = ExtendedPeptideSpectrumMatch.ConvertPeptide(peptide);

                                if (aasequence.HasModification(modToLocalize))
                                {
                                    var spectrum = rawFile.GetSpectrum(spectrumNumber).ToMZSpectrum();
                                    DissociationType dissociationType = rawFile.GetDissociationType(spectrumNumber);
                                    
                                    insertPsm.Parameters.AddWithValue("@filePath", rawFile.FilePath);
                                    insertPsm.Parameters.AddWithValue("@spectrumNumber", spectrumNumber);
                                    insertPsm.Parameters.AddWithValue("@sequence", peptideSequence);
                                    insertPsm.Parameters.AddWithValue("@startResidue", startResidue);
                                    insertPsm.Parameters.AddWithValue("@charge", charge);
                                    insertPsm.Parameters.AddWithValue("@modLine", omssaModificationLine);
                                    insertPsm.Parameters.AddWithValue("@dissType", ExtendedPeptideSpectrumMatch.ConvertSpectrumType(dissociationType));
                                    insertPsm.Parameters.AddWithValue("@spectrum", spectrum.ToBytes());
                                    insertPsm.Parameters.AddWithValue("@lineData", line);
                                    insertPsm.Parameters.AddWithValue("@proteinGroup", proteinGroup);
                                    insertPsm.Parameters.AddWithValue("@defline", defline);
                                    insertPsm.Parameters.AddWithValue("@quantData", quantData.GetBytes());
                                    insertPsm.ExecuteNonQuery();
                                }
                                lineNumber++;
                                double progress = (double)streamReader.BaseStream.Position/streamReader.BaseStream.Length;
                                if (progress >= lastProgress + 0.005)
                                {
                                    lastProgress = progress;
                                    UpdateProgress(progress);
                                }
                                
                            }
                        }
                        transaction.Commit();
                    }
                }
            }

            // Close raw files
            foreach (var openedRawFile in openRawFiles.Values)
            {
                openedRawFile.Dispose();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (data == null)
                    return;
                foreach (string datum in data)
                {
                    if (datum.EndsWith(".csv"))
                    {
                        LoadPeptideFile(datum);
                    }
                    else if (datum.EndsWith(".xml"))
                    {
                        LoadXmlModifications(datum);
                    }
                }
            }
        }

        private void LoadXmlModifications(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                OmssaModification.LoadOmssaModifications(filePath);
                ResetModifications();
            }
        }

        private void LoadPeptideFile(string filePath)
        {
            textBox1.Text = filePath;
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                textBox2.Text = Path.GetDirectoryName(filePath);
            }
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox3.Text = Path.GetDirectoryName(filePath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadPeptideFile(openFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public void SetTitle(string extra = null)
        {
            StringBuilder sb = new StringBuilder("PhosphoRS " + IntPtr.Size * 8 + "-bit");
            if (!string.IsNullOrWhiteSpace(extra))
            {
                sb.Append(" - ");
                sb.Append(extra);
            }
            Text = sb.ToString();
        }

        private void PhosphoRsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.DoEvents();

            if (_mainTask == null || _mainTask.IsCompleted) 
                return;
            
            if (MessageBox.Show("Job currently running, close anyways?", "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _mainTask.ContinueWith(t => Close(), TaskScheduler.FromCurrentSynchronizationContext());
                CancelJob();
            }
            e.Cancel = true;
            Application.DoEvents();
        }

        private void CancelJob()
        {
            if (_cancellationToken == null)
                return;
            _cancellationToken.Cancel();
            button1.Text = "Canceling...";
            SetTitle("Canceling...");
        }
    }
}
