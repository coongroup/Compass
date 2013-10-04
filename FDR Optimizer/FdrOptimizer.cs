using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CSMSL.Analysis.Identification;
using CSMSL.IO;
using CSMSL.IO.Thermo;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.IO.OMSSA;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.FdrOptimizer
{
    public class FdrOptimizer
    {
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
        private readonly string _outputFolder;
        private readonly bool _isBatched;
        private readonly bool _is2DFDR;
        private readonly bool _includeFixedMods;
        private readonly UniquePeptideType _uniquePeptideType;

        public FdrOptimizer(IList<string> csvFilepaths, string rawFolder,
            IList<Modification> fixedModifications,
            double maximumFalseDiscoveryRate,
            UniquePeptideType uniquePeptideType,
            string outputFolder, bool isBatched = false, bool is2DFDR = true, bool includeFixedMods = false)
        {
            _csvFilepaths = csvFilepaths;
            _rawFolder = rawFolder;
            _fixedModifications = fixedModifications;
            _maximumFalseDiscoveryRate = maximumFalseDiscoveryRate/100.0;
            _uniquePeptideType = uniquePeptideType;
            _outputFolder = outputFolder;
            _isBatched = isBatched;
            _is2DFDR = is2DFDR;
            _includeFixedMods = includeFixedMods;
        }

        private void Setup()
        {
            OnStarting(new EventArgs());
            Log("Starting Fdr Optimizer v" + Assembly.GetExecutingAssembly().GetName().Version);
            Directory.CreateDirectory(_outputFolder);
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

        private void WriteFiles(IEnumerable<InputFile> csvFiles, bool isBatched = false)
        {
            Log("Writing output files...");
            List<StreamWriter> openWriters = new List<StreamWriter>();

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
            
            summaryWriter.WriteLine("CSV File,Raw File,Total MS Spectra,Total MS/MS Spectra,Average MS/MS Inj Time (ms),Max MS/MS Inj Time (ms),Total Scored Spectra,Total PSMs,Systematic Precursor Mass Error (ppm),Maximum Precursor Mass Error (ppm),E-Value Threshold,PSMs,Decoy PSMs,PSM FDR (%),Peptides,Decoy Peptides,Peptide FDR (%)");
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
                summaryStringBuilder.Clear();
                string outputTargetFile = Path.Combine(_outputFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_psms.csv");
                string outputDecoyFile = Path.Combine(_outputFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_decoy_psms.csv");
                string outputScansFile = Path.Combine(_outputFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_scans.csv");
                string outputTargetUniqueFile = Path.Combine(_outputFolder,
                    Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_peptides.csv");
                string outputDecoyUniqueFile = Path.Combine(_outputFolder,
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
                summaryStringBuilder.Append(csvFile.AverageMSMSInjectionTime);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.MaxMSMSInjectionTime);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.TotalScans);
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.PsmCount);
                totalInitialPsms += csvFile.PsmCount;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.SystematicPrecursorMassError);
                totalError += csvFile.SystematicPrecursorMassError;
                summaryStringBuilder.Append(',');
                summaryStringBuilder.Append(csvFile.PrecursorMassToleranceThreshold);
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



                using (StreamWriter targetWriter = new StreamWriter(outputTargetFile),
                    decoyWriter = new StreamWriter(outputDecoyFile),
                    scansWriter = new StreamWriter(outputScansFile),
                    targetUniqueWriter = new StreamWriter(outputTargetUniqueFile),
                    decoyUniqueWriter = new StreamWriter(outputDecoyUniqueFile))
                {
                    Dictionary<string, PSM> allPsms =
                        csvFile.PeptideSpectralMatches.ToDictionary(psm => psm.FileName + psm.Peptide.Sequence);

                    HashSet<PSM> fdrPSMs = new HashSet<PSM>(csvFile.FdrFilteredPSMs);

                    Dictionary<PSM, Peptide> fdrPeptides = csvFile.FdrFilteredPeptides.ToDictionary(pep => pep.BestMatch);

                    HashSet<int> scansProcessed = new HashSet<int>();
                    StringBuilder sb = new StringBuilder();
                    using (CsvReader reader = new CsvReader(new StreamReader(csvFile.FilePath), true))
                    {
                        string[] headers = reader.GetFieldHeaders();
                        int headerCount = headers.Length;
                        int modsColumnIndex = reader.GetFieldIndex("Mods");
                        string[] data = new string[headerCount];
                        string headerLine = string.Join(",", headers) +
                                            ",Precursor Isolation m/z (Th),Precursor Theoretical m/z (Th),Precursor Isotope Selected, Adjusted Precursor m/z (Th),Precursor Mass Error (ppm),Adjusted Precursor Mass Error (ppm)";

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
                                scansProcessed.Add(spectralNumber);
                                sb.Clear();
                                reader.CopyCurrentRecordTo(data);
                                for (int i = 0; i < headerCount; i++)
                                {
                                    string datum = data[i];

                                    if (_includeFixedMods && i == modsColumnIndex)
                                    {
                                        datum = OmssaModification.WriteModificationString(psm.Peptide);
                                    }

                                    if (datum.Contains(','))
                                    {
                                        sb.Append("\"");
                                        sb.Append(datum);
                                        sb.Append("\"");
                                    }
                                    else
                                    {
                                        sb.Append(datum);
                                    }

                                    sb.Append(',');
                                }
                                sb.Append(psm.IsolationMz);
                                sb.Append(',');
                                sb.Append(Mass.MzFromMass(psm.MonoisotopicMass, psm.Charge));
                                sb.Append(',');
                                sb.Append(psm.IsotopeSelected);
                                sb.Append(',');
                                sb.Append(Mass.MzFromMass(psm.AdjustedIsolationMass, psm.Charge));
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

                summaryWriter.WriteLine("Batched Results,,,,,,,,,,,{0},{1},{2},{3},{4},{5}", batchTotalPsms, batchTotalDecoyPsms, psmFDR, batchTotalPeptides, batchTotalDecoyPeptides, peptideFDR);
                Log(string.Format("{0:N0} peptides ({1:N0} decoys FDR = {2:F4}) in total [Batched]", batchTotalPeptides, batchTotalDecoyPeptides, peptideFDR));
            }

            foreach (StreamWriter writer in openWriters)
            {
                writer.Close();
            }
        }

        private void ReducePsms(IList<InputFile> csvFiles, UniquePeptideType uniquePeptideType, bool isBatched = false)
        {
            string msg = "Converting PSMs into unique peptides based ";
            IEqualityComparer<Peptide> comparer;
            switch (uniquePeptideType)
            {
                default:
                    msg += "on sequence only";
                    comparer = new SequenceComparer();
                    break;
                case UniquePeptideType.Mass:
                    msg += "on mass";
                    comparer = new MassComparer();
                    break;
                case UniquePeptideType.SequenceAndModifactions:
                    msg += "on sequence and positional modifications";
                    comparer = new SequenceModComparer();
                    break;
                case UniquePeptideType.SequenceAndMass:
                    msg += "on sequence and mass";
                    comparer = new SequenceMassComparer();
                    break;
                case UniquePeptideType.Nothing:
                    msg += "on nothing (no reduction)";
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
            List<Peptide> peptides = inputPeptides.OrderBy(pep => pep.CorrectedPrecursorErrorPPM).ToList();
            double[] precursorPPMs = peptides.Select(pep => pep.CorrectedPrecursorErrorPPM).ToArray();

            double bestppmError = 0;
            double max = peptides[peptides.Count - 1].CorrectedPrecursorErrorPPM;
            double maxPrecursorError = max;
            double minPrecursorError = 0;
            double bestCount = 0;
          
            double increment = (maxPrecursorError - minPrecursorError) / steps;
            while (increment > minimumIncrement)
            {
                for (double ppmError = minPrecursorError; ppmError <= maxPrecursorError; ppmError += increment)
                {
                    int index = Array.BinarySearch(precursorPPMs, ppmError);
                    if (index < 0)
                        index = ~index;

                    int count = FalseDiscoveryRate<Peptide, double>.Count(peptides.Take(index), maximumFalseDisoveryRate);

                    if (count <= bestCount)
                        continue;
                    bestCount = count;
                    bestppmError = ppmError;
                }
                minPrecursorError = Math.Max(bestppmError - increment, 0);
                maxPrecursorError = Math.Min(bestppmError + increment, max);
                increment = (maxPrecursorError - minPrecursorError) / steps;
            }
            List<Peptide> filteredPeptides = new List<Peptide>(peptides.Where(pep => pep.CorrectedPrecursorErrorPPM <= bestppmError));

            // Calculate the e-value threshold for those filtered peptides
            double threshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(filteredPeptides, maximumFalseDisoveryRate);

            return new Tuple<double, double>(bestppmError, threshold);
        }

        private void Calculate2DFdr(IList<InputFile> csvFiles, bool isBatched = false, int steps = 10, double minimumIncrement = 0.05)
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
                List<Peptide> passingPeptides = csvFile.Peptides.Where(pep => pep.CorrectedPrecursorErrorPPM <= ppmError && pep.FdrScoreMetric <= threshold).ToList();
                csvFile.FdrFilteredPeptides = passingPeptides;

                List<PSM> passingPsms = csvFile.PeptideSpectralMatches.Where(psm => Math.Abs(psm.CorrectedPrecursorMassError) <= ppmError && psm.FdrScoreMetric <= threshold).ToList();
                csvFile.FdrFilteredPSMs = passingPsms;

                int total = csvFile.FdrFilteredPeptides.Count;
                int decoys = csvFile.FdrFilteredPeptides.Count(peptide => peptide.IsDecoy);
                int targets = total - decoys;
                Log(
                    string.Format(
                        "{0:N0} peptides ({1:N0} decoys FDR = {2:F4}) pass the e-value threshold of {3:G4} and <= {4:G3} PPM for {5}",
                        targets, decoys, 100.0 * decoys / (double)targets, threshold,ppmError, csvFile.Name));
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
                string csvName = Path.GetFileNameWithoutExtension(csvFile.FilePath);
                csvFile.RawFilePath = "";
                foreach (string file in rawFileNames)
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    if (name != null && (csvName != null && !csvName.StartsWith(name)))
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
                    csvFile.UpdatePsmInformation(dataFile, _is2DFDR, useMedian);
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