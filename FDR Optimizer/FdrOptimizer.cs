using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.IO.MzTab;
using CSMSL.IO.Thermo;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.IO.OMSSA;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.FdrOptimizer
{
    public class FdrOptimizer
    {
        private Regex accessionRegex = new Regex(@"\|(.+)\|", RegexOptions.Compiled);

        private List<Peptide> _allPeptides;

        public event EventHandler Starting;

        protected virtual void OnStarting(EventArgs e)
        {
            var handler = Starting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void OnUpdateProgress(ProgressEventArgs e)
        {
            var handler = UpdateProgress;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        
        public event EventHandler Finished;

        protected virtual void OnFinished(EventArgs e)
        {
            var handler = Finished;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<StatusEventArgs> UpdateLog;

        protected virtual void OnUpdateLog(StatusEventArgs e)
        {
            var handler = UpdateLog;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void Log(string message)
        {
            OnUpdateLog(new StatusEventArgs(message));
        }

        private readonly IList<string> _csvFilepaths;
        private readonly string _rawFolder;
        private readonly IList<Modification> _fixedModifications;
        private readonly double _maximumFalseDiscoveryRate;
        private readonly double _evalueThresholdPPMError = 1e-3;
        private readonly string _outputFolder;
        private readonly string _outputPsmFolder;
        private readonly string _outputScansFolder;
        private readonly string _outputPeptideFolder;
        private readonly bool _isBatched;
        private readonly bool _is2DFDR;
        private readonly bool _includeFixedMods;
        private readonly UniquePeptideType _uniquePeptideType;
        private readonly double _maximumPPMError;

        public FdrOptimizer(IList<string> csvFilepaths, string rawFolder,
            IList<Modification> fixedModifications,
            double maximumFalseDiscoveryRate,
            double maximumPPMError,           
            UniquePeptideType uniquePeptideType,
            string outputFolder, bool isBatched = false, bool is2DFDR = true, bool includeFixedMods = false,  double evalueThresholdForPPMError = 1e-2)
        {
            _csvFilepaths = csvFilepaths;
            _rawFolder = rawFolder;
            _fixedModifications = fixedModifications;
            _maximumFalseDiscoveryRate = maximumFalseDiscoveryRate/100.0;
            _maximumPPMError = maximumPPMError;
            _uniquePeptideType = uniquePeptideType;
            _outputFolder = outputFolder;
            _outputPsmFolder = Path.Combine(outputFolder, "psms");
            _outputPeptideFolder = Path.Combine(outputFolder, "peptides");
            _outputScansFolder = Path.Combine(outputFolder, "scans");
            _isBatched = isBatched;
            _is2DFDR = is2DFDR;
            _includeFixedMods = includeFixedMods;
            _evalueThresholdPPMError = evalueThresholdForPPMError;
        }

        private void Setup()
        {
            OnStarting(new EventArgs());
            Log("Starting Fdr Optimizer v" + Assembly.GetExecutingAssembly().GetName().Version);
            Directory.CreateDirectory(_outputFolder);
            Directory.CreateDirectory(_outputPsmFolder);
            Directory.CreateDirectory(_outputPeptideFolder);
            Directory.CreateDirectory(_outputScansFolder);
        }

        public void Optimize()
        {
            try
            {
                Setup();

                // Read in each CSV File
                List<InputFile> csvFiles = ReadInCSVFiles(_csvFilepaths, _fixedModifications).ToList();

                // Get precursor mass accuracy data if requested
                UpdatePsmInformation(csvFiles, _rawFolder);
                
                // Convert PSMs into Peptides for fdr calculations
                ReducePsms(csvFiles, _uniquePeptideType, _isBatched);

                // Calculate 2D FDR based on precursor mass error
                if (_is2DFDR)
                {
                    Calculate2DFdr(csvFiles, _isBatched);
                }
                else
                {
                    CalculateBasicFDR(csvFiles, _isBatched);
                }

                WriteFiles(csvFiles, _isBatched);
            }
            catch(Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
                Cleanup();
            }
        }

        private enum OmssaCvnColumns
        {
            Spectrum = 0,
            FileName = 1,
            Sequence = 2,
            EValue = 3,
            Mass = 4,
            GI = 5,
            Accession = 6,
            Start = 7,
            Stop = 8,
            Defline = 9,
            Mods = 10,
            Charge = 11,
            TheorecticalMass = 12,
            PValue = 13,
            NISTScore = 14
        }

        private void WriteFiles(IEnumerable<InputFile> csvFiles, bool isBatched = false)
        {
            Log("Writing output files...");
            List<StreamWriter> openWriters = new List<StreamWriter>();
            const string headerLine = "Spectrum number,Filename/id,Peptide,E-value,Mass,gi,Accession,Start,Stop,Defline,Mods,Charge,Theo Mass,P-value,NIST score,Precursor Isolation m/z (Th),Precursor Theoretical m/z (Th),Precursor Isotope Selected, Adjusted Precursor m/z (Th),Precursor Mass Error (ppm),Adjusted Precursor Mass Error (ppm)";

            string outputSummaryFile =
                Path.Combine(_outputFolder, string.Format("FDR summary_{0:yyyyMMddhhmmss}.csv", DateTime.Now));
            StreamWriter summaryWriter = new StreamWriter(outputSummaryFile);
            openWriters.Add(summaryWriter);
            Dictionary<PSM, Peptide> overallBestPsms = null;
            StreamWriter batchTargetUniqueWriter = null, batchDecoyUniqueWriter = null;
            bool firstHeader = true;
            int batchTotalPsms = 0;
            int batchTotalPeptides = 0;
            int batchTotalDecoyPsms = 0;
            int batchTotalDecoyPeptides = 0;
            StreamWriter batchScansWriter = new StreamWriter(Path.Combine(_outputFolder, "scans.csv"));
            StreamWriter batchDecoyWriter = new StreamWriter(Path.Combine(_outputFolder, "decoy_psms.csv"));
            StreamWriter batchTargetWriter = new StreamWriter(Path.Combine(_outputFolder, "psms.csv"));
            openWriters.Add(batchScansWriter);
            openWriters.Add(batchDecoyWriter);
            openWriters.Add(batchTargetWriter);
            if (isBatched)
            {
                batchTargetUniqueWriter = new StreamWriter(Path.Combine(_outputFolder, "peptides.csv"));
                batchDecoyUniqueWriter = new StreamWriter(Path.Combine(_outputFolder, "decoy_peptides.csv"));
                openWriters.Add(batchTargetUniqueWriter);
                openWriters.Add(batchDecoyUniqueWriter);
                overallBestPsms = _allPeptides.ToDictionary(pep => pep.BestMatch);
            }
            
            MzTabMetaData metaData = new MzTabMetaData(description: "FDR Optimizer Output");

            Dictionary<string, int> mzTabIDs = new Dictionary<string, int>();
            metaData.MsRunLocations = new List<string>();
            List<MzTabPSM> mzTabPsms = new List<MzTabPSM>();
            int mzTabPsmId = 1;

            summaryWriter.WriteLine("CSV File,Raw File,Total MS Spectra,Total MS/MS Spectra,Average MS/MS Inj Time (ms),Max MS/MS Inj Time (ms),Average # of MS/MS per Cycle, Max # of MS/MS per Cycle,Total Scored Spectra,Total PSMs,Systematic Precursor Mass Error (ppm),Maximum Precursor Mass Error (ppm),E-Value Threshold,PSMs,Decoy PSMs,PSM FDR (%),Peptides,Decoy Peptides,Peptide FDR (%)");
            StringBuilder summaryStringBuilder = new StringBuilder();
            int totalPsms = 0;
            double totalError = 0;
            double totalMaximalError = 0;
            int totalPeptides = 0;
            int totalDecoyPeptides = 0;
            double totalThreshold = 0;
            int totalDecoyPsms = 0;
            int totalMS = 0;
            int totalInitialPsms = 0;
            int totalMSMS = 0;
            foreach (InputFile csvFile in csvFiles)
            {
                metaData.MsRunLocations.Add(csvFile.RawFilePath);

                summaryStringBuilder.Clear();
                string outputTargetFile = Path.Combine(_outputPsmFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_psms.csv");
                string outputDecoyFile = Path.Combine(_outputPsmFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_decoy_psms.csv");
                string outputScansFile = Path.Combine(_outputScansFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_scans.csv");
                string outputTargetUniqueFile = Path.Combine(_outputPeptideFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_peptides.csv");
                string outputDecoyUniqueFile = Path.Combine(_outputPeptideFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_decoy_peptides.csv");
                Log("Writing output files for " + Path.GetFileNameWithoutExtension(csvFile.Name) + " in " +
                    _outputFolder + "...");

                summaryStringBuilder.Append(csvFile.FilePath);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.RawFilePath);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.TotalMSscans);
                totalMS += csvFile.TotalMSscans;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.TotalMSMSscans);
                totalMSMS += csvFile.TotalMSMSscans;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.AverageMSMSInjectionTime.ToString("F5"));
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.MaxMSMSInjectionTime);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.AverageMSMSSCansBetweenMS.ToString("F5"));
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.MaxMSMSScansBetweenMS);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.TotalScans);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.PsmCount);
                totalInitialPsms += csvFile.PsmCount;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.SystematicPrecursorMassError.ToString("F5"));
                totalError += csvFile.SystematicPrecursorMassError;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.PrecursorMassToleranceThreshold.ToString("F5"));
                totalMaximalError += csvFile.PrecursorMassToleranceThreshold;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.ScoreThreshold);
                totalThreshold += csvFile.ScoreThreshold;
                summaryStringBuilder.Append(',');

                int totalPSMs = csvFile.FdrFilteredPSMs.Count;
                int targetPSMs = csvFile.FdrFilteredPSMs.Count(pep => !pep.IsDecoy);
                int decoyPSMs = totalPSMs - targetPSMs;

                summaryStringBuilder.Append(targetPSMs);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(decoyPSMs);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(100*decoyPSMs/(double) targetPSMs);
                summaryStringBuilder.Append(',');

                int total = csvFile.FdrFilteredPeptides.Count;
                int targets = csvFile.FdrFilteredPeptides.Count(pep => !pep.IsDecoy);
                int decoys = total - targets;

                summaryStringBuilder.Append(targets);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(decoys);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(100*decoys/(double) targets);
                
                if (csvFile.ScoreType == PeptideSpectralMatchScoreType.OmssaEvalue)
                {
                    using (StreamWriter targetWriter = new StreamWriter(outputTargetFile),
                        decoyWriter = new StreamWriter(outputDecoyFile),
                        scansWriter = new StreamWriter(outputScansFile),
                        targetUniqueWriter = new StreamWriter(outputTargetUniqueFile),
                        decoyUniqueWriter = new StreamWriter(outputDecoyUniqueFile))
                    {
                        Dictionary<string, PSM> allPsms = csvFile.PeptideSpectralMatches.ToDictionary(psm => psm.FileName + psm.Peptide.Sequence);

                        HashSet<PSM> fdrPSMs = new HashSet<PSM>(csvFile.FdrFilteredPSMs);

                        Dictionary<PSM, Peptide> fdrPeptides = csvFile.FdrFilteredPeptides.ToDictionary(pep => pep.BestMatch);

                        HashSet<int> scansProcessed = new HashSet<int>();
                        StringBuilder sb = new StringBuilder();
                        using (CsvReader reader = new CsvReader(new StreamReader(csvFile.FilePath), true))
                        {
                            string[] headers = reader.GetFieldHeaders();
                            int headerCount = headers.Length;
                           
                            string[] data = new string[headerCount];

                            decoyWriter.WriteLine(headerLine);
                            targetWriter.WriteLine(headerLine);
                            scansWriter.WriteLine(headerLine);
                            targetUniqueWriter.WriteLine(headerLine);
                            decoyUniqueWriter.WriteLine(headerLine);
                            if (firstHeader)
                            {
                                batchScansWriter.WriteLine(headerLine);
                                batchDecoyWriter.WriteLine(headerLine);
                                batchTargetWriter.WriteLine(headerLine);
                                if (isBatched)
                                {
                                    batchDecoyUniqueWriter.WriteLine(headerLine);
                                    batchTargetUniqueWriter.WriteLine(headerLine);
                                }
                                firstHeader = false;
                            }
                            while (reader.ReadNextRecord())
                            {
                                PSM psm;
                                int spectralNumber = int.Parse(reader["Spectrum number"]);
                                if (scansProcessed.Contains(spectralNumber))
                                    continue;
                                string fileName = reader["Filename/id"];
                                string sequence = reader["Peptide"].ToUpper();
                                
                                if (allPsms.TryGetValue(fileName + sequence, out psm))
                                {
                                    bool isNegative = psm.Charge < 0;

                                    MzTabPSM mztabPsm = new MzTabPSM();
                                    mzTabPsms.Add(mztabPsm);

                                    string seq = psm.Peptide.Sequence;
                                    int id;
                                    if (!mzTabIDs.TryGetValue(seq, out id))
                                    {
                                        id = mzTabIDs.Count + 1;
                                        mzTabIDs.Add(seq, id);
                                    }

                                    mztabPsm.Sequence = psm.Peptide.Sequence;
                                    mztabPsm.ID = id;
                                    mztabPsm.SearchEngines = new List<CVParamater>() { "[MS,MS:1001475,OMSSA,]" };
                                    mztabPsm.SpectraReference = "ms_run[1]:scan=" + spectralNumber;
                                    mztabPsm.RetentionTime = new List<double>() { psm.RetentionTimeSeconds };

                                    scansProcessed.Add(spectralNumber);
                                    sb.Clear();
                                    reader.CopyCurrentRecordTo(data);
                                    for (int i = 0; i < 15; i++)
                                    {
                                        string datum = data[i];

                                        switch (i)
                                        {
                                            case (int)OmssaCvnColumns.EValue:
                                                mztabPsm.SearchEngineScores = new List<double>() { double.Parse(datum) }; break;
                                            case (int)OmssaCvnColumns.Start:
                                                mztabPsm.StartResiduePosition = int.Parse(datum); break;
                                            case (int)OmssaCvnColumns.Stop:
                                                mztabPsm.EndResiduePosition = int.Parse(datum); break;
                                            case (int)OmssaCvnColumns.Accession:
                                                mztabPsm.Accession = "TBD"; break;
                                            case (int)OmssaCvnColumns.Charge:
                                                mztabPsm.Charge = psm.Charge;
                                                if (isNegative)
                                                {
                                                    datum = psm.Charge.ToString();
                                                }
                                                break;
                                            case (int)OmssaCvnColumns.Mods:
                                                if (_includeFixedMods)
                                                    datum = OmssaModification.WriteModificationString(psm.Peptide);
                                                break;
                                        }
                                        

                                        if (datum.Contains('"'))
                                            datum = datum.Replace("\"", "\"\"");

                                        if (datum.Contains(','))
                                        {
                                            sb.Append('"');
                                            sb.Append(datum);
                                            sb.Append('"');
                                        }
                                        else
                                        {
                                            sb.Append(datum);
                                        }

                                        sb.Append(',');
                                    }
                                    sb.Append(psm.IsolationMz);
                                    sb.Append(',');
                                    double theoMZ = Mass.MzFromMass(psm.MonoisotopicMass, psm.Charge);
                                    sb.Append(theoMZ);
                                    mztabPsm.TheoreticalMZ = theoMZ;
                                    sb.Append(',');
                                    sb.Append(psm.IsotopeSelected);
                                    sb.Append(',');
                                    double experimentalMZ = Mass.MzFromMass(psm.AdjustedIsolationMass, psm.Charge);
                                    sb.Append(experimentalMZ);
                                    mztabPsm.ExperimentalMZ = experimentalMZ;
                                    sb.Append(',');
                                    sb.Append(psm.PrecursorMassError);
                                    sb.Append(',');
                                    sb.Append(psm.CorrectedPrecursorMassError);

                                    string line = sb.ToString();

                                    scansWriter.WriteLine(line);
                                    batchScansWriter.WriteLine(line);

                                    // Passes FDR, write out
                                    if (fdrPSMs.Contains(psm))
                                    {
                                        if (psm.IsDecoy)
                                        {
                                            totalDecoyPsms++;
                                            batchTotalDecoyPsms++;
                                            decoyWriter.WriteLine(line);
                                            batchDecoyWriter.WriteLine(line);
                                        }
                                        else
                                        {
                                            totalPsms++;
                                            batchTotalPsms++;
                                            targetWriter.WriteLine(line);
                                            batchTargetWriter.WriteLine(line);

                                        }

                                        Peptide pep;
                                        // Is this the best unique psm?
                                        if (fdrPeptides.TryGetValue(psm, out pep))
                                        {
                                            if (pep.IsDecoy)
                                            {
                                                totalDecoyPeptides++;
                                                decoyUniqueWriter.WriteLine(line);
                                            }
                                            else
                                            {
                                                totalPeptides++;
                                                targetUniqueWriter.WriteLine(line);
                                            }
                                        }

                                        if (isBatched && overallBestPsms.TryGetValue(psm, out pep))
                                        {
                                            if (pep.IsDecoy)
                                            {
                                                batchTotalDecoyPeptides++;
                                                batchDecoyUniqueWriter.WriteLine(line);
                                            }
                                            else
                                            {
                                                batchTotalPeptides++;
                                                batchTargetUniqueWriter.WriteLine(line);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                summaryWriter.WriteLine(summaryStringBuilder.ToString());
            }

            summaryWriter.WriteLine();

            //int count = csvFiles.Count;
            //totalPsms /= count;
            //totalDecoyPsms /= count;
            //double totalPsmFdr = 100*totalDecoyPsms/(double) totalPsms;
            //totalPeptides /= count;
            //totalDecoyPeptides /= count;
            //double totalPeptideFdr = 100*totalDecoyPeptides/(double) totalPeptides;
            //summaryWriter.WriteLine("Average Results,,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", totalMS / count, totalMSMS / count, "", "", totalScans / count, totalInitialPsms / count, totalError / count, totalMaximalError / count, totalThreshold / count, totalPsms, totalDecoyPsms, "", totalPeptides, totalDecoyPeptides, "");

            if (isBatched)
            {
              
                double psmFDR = 100*batchTotalDecoyPsms/(double) batchTotalPsms;
                double peptideFDR = 100 *batchTotalDecoyPeptides/(double) batchTotalPeptides;

                summaryWriter.WriteLine("Batched Results,,,,,,,,,,,,,{0},{1},{2},{3},{4},{5}", batchTotalPsms, batchTotalDecoyPsms, psmFDR, batchTotalPeptides, batchTotalDecoyPeptides, peptideFDR);
                Log(string.Format("{0:N0} peptides ({1:N0} decoys FDR = {2:F4}) in total [Batched]", batchTotalPeptides, batchTotalDecoyPeptides, peptideFDR));
            }

            // Write out settings
            summaryWriter.WriteLine();
            summaryWriter.WriteLine("FDR Optimizer, {0}-bit (v{1})", IntPtr.Size * 8, Assembly.GetExecutingAssembly().GetName().Version);
            summaryWriter.WriteLine("2D FDR, {0}", _is2DFDR);
            summaryWriter.WriteLine("Batched, {0}", _isBatched);
            summaryWriter.WriteLine("Reduce PSMs, {0}", _uniquePeptideType);
            summaryWriter.WriteLine("Maximum FDR, {0}%", _maximumFalseDiscoveryRate * 100);
            summaryWriter.WriteLine("Maximum PPM error, {0}", _maximumPPMError);
            summaryWriter.WriteLine("Include fixed mods, {0}", _includeFixedMods);
            foreach (var mod in _fixedModifications)
            {
                summaryWriter.WriteLine(", {0}", mod.NameAndSites);
            }

            // mzTab outputs

            string mztabLocation = Path.Combine(_outputFolder,
                    string.Format("FDR summary_{0:yyyyMMddhhmmss}.mzTab", DateTime.Now));
            
            using (MzTabWriter mzTabWriter = new MzTabWriter(mztabLocation))
            {
                mzTabWriter.WriteComment("Experimental FDR optimizer output in mzTab format");
                mzTabWriter.WriteMetaData(metaData);
                mzTabWriter.WriteLine();
                mzTabWriter.WritePsmData(mzTabPsms);
            }
            
            foreach (StreamWriter writer in openWriters)
            {
                writer.Close();
            }
        }

        private void ReducePsms(IList<InputFile> csvFiles, UniquePeptideType uniquePeptideType, bool isBatched = false)
        {
            string msg = "Converting PSMs into unique peptides based on ";
            IEqualityComparer<Peptide> comparer;
            switch (uniquePeptideType)
            {
                default:
                    msg += "sequence only";
                    comparer = new SequenceComparer();
                    break;
                case UniquePeptideType.Mass:
                    msg += "mass";
                    comparer = new MassComparer();
                    break;
                case UniquePeptideType.SequenceAndModifications:
                    msg += "sequence and positional modifications";
                    comparer = new SequenceModComparer();
                    break;
                case UniquePeptideType.SequenceAndModLocations:
                    msg += "sequence and modification locations";
                    comparer = new SequenceAndModPositionComparer();
                    break;
                case UniquePeptideType.SequenceAndMass:
                    msg += "sequence and mass";
                    comparer = new SequenceMassComparer();
                    break;
                case UniquePeptideType.Nothing:
                    msg += "nothing (no reduction)";
                    comparer = new IdentityComparer<Peptide>();
                    break;
            }
            Log(msg);

            foreach (InputFile csvFile in csvFiles)
            {
                csvFile.ReducePsms(comparer);
                Log(string.Format("{0:N0} unique peptides remain from {1:N0} PSMs from {2}", csvFile.Peptides.Count, csvFile.PeptideSpectralMatches.Count, csvFile.Name));
            }

            if (!isBatched) 
                return;
           
            Dictionary<Peptide, Peptide> peptideDictionary = new Dictionary<Peptide, Peptide>(comparer);
            foreach (Peptide peptide in csvFiles.SelectMany(csvFile => csvFile.Peptides))
            {
                Peptide realPeptide;
                if (peptideDictionary.TryGetValue(peptide, out realPeptide))
                {
                    foreach (PSM psm in peptide.PSMs)
                    {
                        realPeptide.AddPsm(psm);
                    }
                }
                else
                {
                    Peptide newPeptide = new Peptide(peptide);
                    peptideDictionary.Add(newPeptide, newPeptide);
                }
            }
            _allPeptides = peptideDictionary.Values.ToList();
            Log(string.Format("{0:N0} unique peptides from all files [Batched]", _allPeptides.Count));
        }
        
        private Tuple<double,double> CalculateBestPPMError(IEnumerable<Peptide> inputPeptides, double maximumFalseDisoveryRate = 0.01, int steps = 10, double minimumIncrement = 0.05)
        {
            var peptideComparer = Comparer<Peptide>.Default;
            //List<Peptide> peptides = inputPeptides.OrderBy(pep => pep.CorrectedPrecursorErrorPPM).ThenBy(pep => pep.BestMatch).ToList();
            List<Peptide> peptides = inputPeptides.OrderBy(pep => pep.BestMatch).ToList();
           // peptides.Sort(peptideComparer);

            PeptideSpectralMatchScoreType scoreType = peptides[0].BestMatch.ScoreType;

           // double[] precursorPPMs = peptides.Select(pep => pep.CorrectedPrecursorErrorPPM).ToArray();

            double bestppmError = 0;
            double max = peptides.Max(pep => pep.CorrectedPrecursorErrorPPM);
            double maxPrecursorError = Math.Min(max, _maximumPPMError);
            const double minPrecursorError = 0;

            double increment = (maxPrecursorError - minPrecursorError)/steps;

            increment = Math.Max(increment, minimumIncrement);

            double bestCount = 0;

            var scoreComparer = Comparer<double>.Create((a,b) => a.CompareTo(b) * Math.Sign((int)scoreType));

            for (double ppmError = minPrecursorError; ppmError <= maxPrecursorError; ppmError += increment)
            {
                List<Peptide> testPeptides = peptides.Where(pep => pep.CorrectedPrecursorErrorPPM <= ppmError).ToList();
                //int index = Array.BinarySearch(precursorPPMs, ppmError);
                //if (index < 0)
                //    index = ~index;
                
                //int count = FalseDiscoveryRate<Peptide, double>.Count(peptides.Take(index).ToList(), peptideComparer, scoreComparer, maximumFalseDisoveryRate, preSorted: false);
                int count = FalseDiscoveryRate<Peptide, double>.Count(testPeptides, peptideComparer, scoreComparer, maximumFalseDisoveryRate, preSorted: true);

                if (count <= bestCount)
                    continue;
                bestCount = count;
                bestppmError = ppmError;
            }
       
            List<Peptide> filteredPeptides = new List<Peptide>(peptides.Where(pep => pep.CorrectedPrecursorErrorPPM <= bestppmError));
            
            // Calculate the e-value threshold for those filtered peptides
            double threshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(filteredPeptides, maximumFalseDisoveryRate);

            return new Tuple<double, double>(bestppmError, threshold);
        }

        private void Calculate2DFdr(IList<InputFile> csvFiles, bool isBatched = false, int steps = 250, double minimumIncrement = 0.05)
        {
            string msg = "Calculating second order FDR threshold";

            if (isBatched)
            {
                msg += " in batch...";
                Log(msg);        

                Tuple<double,double> ppmThreshold = CalculateBestPPMError(_allPeptides, _maximumFalseDiscoveryRate, steps, minimumIncrement);
                foreach (InputFile csvFile in csvFiles)
                {
                    csvFile.PrecursorMassToleranceThreshold = ppmThreshold.Item1;
                    csvFile.ScoreThreshold = ppmThreshold.Item2;
                }
            }
            else
            {
                msg += " separately...";
                Log(msg);
                foreach (InputFile csvFile in csvFiles)
                {
                    Tuple<double, double> ppmThreshold = CalculateBestPPMError(csvFile.Peptides, _maximumFalseDiscoveryRate, steps, minimumIncrement);
                    csvFile.PrecursorMassToleranceThreshold = ppmThreshold.Item1;
                    csvFile.ScoreThreshold = ppmThreshold.Item2;
                }
            }

            foreach (InputFile csvFile in csvFiles)
            {
                double ppmError = csvFile.PrecursorMassToleranceThreshold;
                double threshold = csvFile.ScoreThreshold;
                int sign = Math.Sign((int)csvFile.ScoreType);

                List<Peptide> passingPeptides = csvFile.Peptides.Where(pep => pep.CorrectedPrecursorErrorPPM <= ppmError && pep.FdrScoreMetric * sign <= threshold * sign).ToList();
                csvFile.FdrFilteredPeptides = passingPeptides;

                List<PSM> passingPsms = passingPeptides.SelectMany(p => p.PSMs).Where(psm => Math.Abs(psm.CorrectedPrecursorMassError) <= ppmError && psm.FdrScoreMetric * sign <= threshold * sign).ToList();

                List<PSM> passingPsms2 = csvFile.PeptideSpectralMatches.Where(psm => Math.Abs(psm.CorrectedPrecursorMassError) <= ppmError && psm.FdrScoreMetric * sign <= threshold * sign).ToList();
                int a = passingPsms2.Count;
                csvFile.FdrFilteredPSMs = passingPsms;

                int total = csvFile.FdrFilteredPeptides.Count;
                int decoys = csvFile.FdrFilteredPeptides.Count(peptide => peptide.IsDecoy);
                int targets = total - decoys;
                Log(
                    string.Format(
                        "{0:N0} peptides ({1:N0} decoys FDR = {2:F4}) pass the {3} threshold of {4} {5:G4} and <= {6:G3} PPM for {7}",
                        targets, decoys, 100.0 * decoys / (double)targets, csvFile.ScoreType, ((int)csvFile.ScoreType <= 0) ? ">=" : "<=", threshold, ppmError, csvFile.Name));
            }
        }

        private void CalculateBasicFDR(IList<InputFile> csvFiles, bool isBatched = false)
        {
            string msg = "Calculating first order FDR threshold";

            if(isBatched){
                msg += " in batch...";
                Log(msg);
                // Calculate global threshold
                double threshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(_allPeptides, _maximumFalseDiscoveryRate);
                foreach (InputFile csvFile in csvFiles)
                {
                    csvFile.ScoreThreshold = threshold;
                }
            }
            else
            {
                msg += " separately...";
                Log(msg);
                // Calculate each file separately
                foreach (InputFile csvFile in csvFiles)
                {
                    csvFile.ScoreThreshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(csvFile.Peptides, _maximumFalseDiscoveryRate); 
                }
            }
           
            foreach (InputFile csvFile in csvFiles)
            {
                double threshold = csvFile.ScoreThreshold;
                List<Peptide> passingPeptides = csvFile.Peptides.Where(peptide => peptide.FdrScoreMetric <= threshold).ToList();
                csvFile.FdrFilteredPeptides = passingPeptides;

                List<PSM> passingPsms = csvFile.PeptideSpectralMatches.Where(psm => psm.FdrScoreMetric <= threshold).ToList();
                csvFile.FdrFilteredPSMs = passingPsms;
                int total = csvFile.FdrFilteredPeptides.Count;
                int decoys = csvFile.FdrFilteredPeptides.Count(peptide => peptide.IsDecoy);
                int targets = total - decoys;
                Log(
                    string.Format(
                        "{0:N0} peptides ({1:N0} decoys FDR = {2:F4}) pass the e-value threshold of {3:G4} for {4}",
                        targets, decoys, 100.0 * decoys / (double)targets, csvFile.ScoreThreshold, csvFile.Name));
            }
        }
 
        private void UpdatePsmInformation(IList<InputFile> csvFiles, string rawFolder, bool useMedian = true)
        {
            Log("Reading MS data...");
            MSDataFile.CacheScans = false;
            List<string> rawFileNames = Directory.EnumerateFiles(rawFolder, "*.raw", SearchOption.AllDirectories).ToList();

            foreach (InputFile csvFile in csvFiles)
            {
                string rawFileName = csvFile.RawFileName;
                if (string.IsNullOrEmpty(rawFileName))
                {
                    throw new ArgumentException("Cannot parse the file name for: " + csvFile.FilePath);
                }
                csvFile.RawFilePath = "";
                foreach (string file in rawFileNames)
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    if (name != null && !rawFileName.Equals(name))
                        continue;
                    csvFile.RawFilePath = file;
                    break;
                }
                if (string.IsNullOrEmpty(csvFile.RawFilePath))
                {
                    throw new ArgumentException("Cannot find the associated raw file for: " + csvFile.FilePath);
                }
            }

            // update the precursor mass error
            foreach (InputFile csvFile in csvFiles)
            {
                using (MSDataFile dataFile = new ThermoRawFile(csvFile.RawFilePath))
                {
                    dataFile.Open();
                    csvFile.UpdatePsmInformation(dataFile, _is2DFDR, useMedian, _evalueThresholdPPMError);
                }

                if (_is2DFDR)
                {
                    Log(string.Format("{0:F2} ppm {1} precursor mass error in {2}",
                        csvFile.SystematicPrecursorMassError, useMedian ? "median" : "average", csvFile.Name));
                }
            }
        }

        private IEnumerable<InputFile> ReadInCSVFiles(IEnumerable<string> csvFilePaths, IList<Modification> fixedModifications, int numberOfTopHits = 1)
        {
            Log("Reading in Peptide Spectral Matches...");
            int totalDecoyPSMs = 0;
            int totalTargetPSMs = 0;
            int files = 0;
            foreach (var csvFile in csvFilePaths.Select(csvFilePath => new InputFile(csvFilePath)))
            {
                files++;
                csvFile.Read(fixedModifications, numberOfTopHits);
                int psms = csvFile.PeptideSpectralMatches.Count;
                int decoys = csvFile.PeptideSpectralMatches.Count(psm => psm.IsDecoy);
                totalTargetPSMs += psms - decoys;
                totalDecoyPSMs += decoys;
                Log(string.Format("{0:N0} targets {1:N0} decoys PSMs were read in from {2}", psms - decoys, decoys, csvFile.Name));
                yield return csvFile;
            }
            Log(string.Format("{0:N0} targets {1:N0} decoys PSMs in total from {2:N0} files", totalTargetPSMs, totalDecoyPSMs, files));
        }

        private void Cleanup()
        {
            Log("Finished");
            OnFinished(new EventArgs());
        }
    }
}