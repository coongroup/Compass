using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CSMSL;
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
        private const double MAXIMUM_FDR_FOR_SYSTEMATIC_PRECURSOR_MASS_ERROR = 1.0;

        public event EventHandler Starting;

        protected virtual void onStarting(EventArgs e)
        {
            var handler = Starting;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> StartingFile;

        protected virtual void onStartingFile(FilepathEventArgs e)
        {
            var handler = StartingFile;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void onUpdateProgress(ProgressEventArgs e)
        {
            var handler = UpdateProgress;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ExceptionEventArgs> ThrowException;

        protected virtual void onThrowException(ExceptionEventArgs e)
        {
            var handler = ThrowException;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> FinishedFile;

        protected virtual void onFinishedFile(FilepathEventArgs e)
        {
            var handler = FinishedFile;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Finished;

        protected virtual void onFinished(EventArgs e)
        {
            var handler = Finished;

            if(handler != null)
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

        private IList<string> csvFilepaths;
        private string rawFolder;
        private IList<Modification> fixedModifications;
        private bool higherScoresAreBetter;
        private double maximumFalseDiscoveryRate;
        private bool UseUniqueSequence;
        private bool overallOutputs;
        private bool phosphopeptideOutputs;
        private string _outputFolder;
        private bool IsBatched;
        private bool Is2DFDR;
        private bool IncludeFixedMods;
        private UniquePeptideType uniquePeptideType;

        private string _logFolder;
        private string _scansFolder;
        private string _targetDecoyFolder;
        private string _uniqueFolder;
        private string _overallScansFilepath;
        private string _overallScansPhosphoFilepath;
        private string _overallPhosphoOutputFolder;
        private string _scansPhosphoFolder;
        private string _targetDecoyPhosphoFolder;
        private string _uniquePhosphoFolder;

        private string _overallTargetFilepath;
        private string _overallDecoyFilepath;
        private string _overallTargetPhosphoFilepath;
        private string _overallDecoyPhosphoFilepath;

        private string _overallTargetUniqueFilepath;
        private string _overallDecoyUniqueFilepath;
        private string _overallTargetUniquePhosphoFilepath;
        private string _overallDecoyUniquePhosphoFilepath;

        string _extendedHeaderLine = null;
        bool _overallHeaderWritten = false;
        int overall_scans = 0;
        int overall_scans_phospho = 0;
        int overall_target = 0;
        int overall_target_phospho = 0;
        int overall_decoy = 0;
        int overall_decoy_phospho = 0;
        int overall_target_unique = 0;
        int overall_decoy_unique = 0;
        int overall_target_unique_phospho = 0;
        int overall_decoy_unique_phospho = 0;
        int overall_target_unique_unique_phospho = 0;
        int overall_decoy_unique_unique_phospho = 0;

        StreamWriter overall_scans_output = null;
        StreamWriter overall_target_output = null;
        StreamWriter overall_decoy_output = null;
        StreamWriter overall_target_unique_output = null;
        StreamWriter overall_decoy_unique_output = null;
        StreamWriter overall_scans_phospho_output = null;
        StreamWriter overall_target_phospho_output = null;
        StreamWriter overall_decoy_phospho_output = null;
        StreamWriter overall_target_unique_phospho_output = null;
        StreamWriter overall_decoy_unique_phospho_output = null;
        StreamWriter overall_log = null;
        StreamWriter log = null;
        StreamWriter _summaryStreamWriter = null;
       // IXRawfile4 raw = null;
        StreamReader csv = null;
        StreamWriter scans_output = null;
        StreamWriter scans_phospho_output = null;
        StreamWriter target_output = null;
        StreamWriter decoy_output = null;
        StreamWriter target_phospho_output = null;
        StreamWriter decoy_phospho_output = null;
        StreamWriter target_unique_output = null;
        StreamWriter decoy_unique_output = null;
        StreamWriter target_unique_phospho_output = null;
        StreamWriter decoy_unique_phospho_output = null;
        StreamWriter overall_target_unique_unique_output = null;
        StreamWriter overall_decoy_unique_unique_output = null;
        StreamWriter overall_target_unique_unique_phospho_output = null;
        StreamWriter overall_decoy_unique_unique_phospho_output = null;


        private SortedDictionary<string, PeptideHit> overall_target_peptides;
        private SortedDictionary<string, PeptideHit> overall_decoy_peptides;

        public FdrOptimizer(IList<string> csvFilepaths, string rawFolder, 
            IList<Modification> fixedModifications, 
            bool higherScoresAreBetter, 
            double maximumFalseDiscoveryRate,
            UniquePeptideType uniquePeptideType, 
            bool overallOutputs, bool phosphopeptideOutputs, string _outputFolder, bool isBatched = false, bool is2DFDR = true, bool includeFixedMods = false)
        {
            this.csvFilepaths = csvFilepaths;
            this.rawFolder = rawFolder;
            this.fixedModifications = fixedModifications;
            this.higherScoresAreBetter = higherScoresAreBetter;
            this.maximumFalseDiscoveryRate = maximumFalseDiscoveryRate / 100;
            this.uniquePeptideType = uniquePeptideType;
            this.overallOutputs = overallOutputs;
            this.phosphopeptideOutputs = phosphopeptideOutputs;
            this._outputFolder = _outputFolder;
            IsBatched = isBatched;
            Is2DFDR = is2DFDR;
            IncludeFixedMods = includeFixedMods;

            overall_target_peptides = new SortedDictionary<string, PeptideHit>();
            overall_decoy_peptides = new SortedDictionary<string, PeptideHit>();

            _logFolder = Path.Combine(_outputFolder, "log");
            _scansFolder = Path.Combine(_outputFolder, "scans");
            _targetDecoyFolder = Path.Combine(_outputFolder, "target-decoy");
            _uniqueFolder = Path.Combine(_outputFolder, "unique");
            _overallScansFilepath = Path.Combine(_outputFolder, "scans.csv");
            _overallPhosphoOutputFolder = Path.Combine(_outputFolder, "phospho");
            _overallScansPhosphoFilepath = Path.Combine(_overallPhosphoOutputFolder, "scans_phospho.csv");
            _scansPhosphoFolder = Path.Combine(_scansFolder, "phospho");
            _targetDecoyPhosphoFolder = Path.Combine(_targetDecoyFolder, "phospho");
            _uniquePhosphoFolder = Path.Combine(_uniqueFolder, "phospho");

            _overallTargetFilepath = Path.Combine(_outputFolder, "target.csv");
            _overallDecoyFilepath = Path.Combine(_outputFolder, "decoy.csv");
            _overallTargetPhosphoFilepath = Path.Combine(_overallPhosphoOutputFolder, "target_phospho.csv");
            _overallDecoyPhosphoFilepath = Path.Combine(_overallPhosphoOutputFolder, "decoy_phospho.csv");

            _overallTargetUniqueFilepath = Path.Combine(_outputFolder, "target_unique.csv");
            _overallDecoyUniqueFilepath = Path.Combine(_outputFolder, "decoy_unique.csv");
            _overallTargetUniquePhosphoFilepath = Path.Combine(_overallPhosphoOutputFolder, "target_unique_phospho.csv");
            _overallDecoyUniquePhosphoFilepath = Path.Combine(_overallPhosphoOutputFolder, "decoy_unique_phospho.csv");

            //overall_scans_output = new StreamWriter(_overallScansFilepath);
            //overall_target_output = new StreamWriter(_overallTargetFilepath);
           // overall_decoy_output = new StreamWriter(_overallDecoyFilepath);
            //overall_target_unique_output = new StreamWriter(_overallTargetUniqueFilepath);
            //overall_decoy_unique_output = new StreamWriter(_overallDecoyUniqueFilepath);

            if (phosphopeptideOutputs)
            {
               // overall_scans_phospho_output = new StreamWriter(_overallScansPhosphoFilepath);
                //overall_target_phospho_output = new StreamWriter(_overallTargetPhosphoFilepath);
                //overall_decoy_phospho_output = new StreamWriter(_overallDecoyPhosphoFilepath);
                //overall_target_unique_phospho_output = new StreamWriter(_overallTargetUniquePhosphoFilepath);
                //overall_decoy_unique_phospho_output = new StreamWriter(_overallDecoyUniquePhosphoFilepath);
            }
        }

        private string GetModificationString()
        {
            StringBuilder fixed_modifications_sb = new StringBuilder();
            foreach (Modification modification in fixedModifications)
            {
                fixed_modifications_sb.Append(modification.Name);
                fixed_modifications_sb.Append(", ");
            }
            if (fixed_modifications_sb.Length > 0)
            {
                fixed_modifications_sb = fixed_modifications_sb.Remove(fixed_modifications_sb.Length - 2, 2);
            }
            return fixed_modifications_sb.ToString();
        }
        
        private StreamWriter _overallLog;

        private void WriteToOverallLog(string message = "")
        {
            if (overallOutputs)
            {
                _overallLog.WriteLine(message);
            }
        }

        private Dictionary<string, RawFile> _inputFiles;

        private void Setup()
        {
            onStarting(new EventArgs());
            Log("Starting Fdr Optimizer v"+Assembly.GetExecutingAssembly().GetName().Version);
            _inputFiles = new Dictionary<string, RawFile>();
          
            foreach (string csv_filepath in csvFilepaths)
            {
                string truncated_filename = Path.GetFileNameWithoutExtension(csv_filepath);
                string[] raw_filepaths = null;
                do
                {
                    if (rawFolder != null && rawFolder != string.Empty && Directory.Exists(rawFolder))
                    {
                        raw_filepaths = Directory.GetFiles(rawFolder, truncated_filename + ".raw", SearchOption.AllDirectories);
                    }
                    else
                    {
                        raw_filepaths = Directory.GetFiles(Directory.GetParent(csv_filepath).ToString(), truncated_filename + ".raw", SearchOption.AllDirectories);
                    }
                    truncated_filename = truncated_filename.Substring(0, truncated_filename.Length - 1);
                    if (truncated_filename.Length == 0)
                    {
                        throw new Exception("No corresponding .raw file found for " + csv_filepath);
                    }
                } while (raw_filepaths.Length == 0);
                RawFile rawFile;
                if (!_inputFiles.TryGetValue(raw_filepaths[0], out rawFile))
                {
                    rawFile = new RawFile(raw_filepaths[0]);
                    _inputFiles.Add(rawFile.FilePath, rawFile);
                }
                rawFile.CsvFiles.Add(csv_filepath);
            }

            Directory.CreateDirectory(_outputFolder);
            //Directory.CreateDirectory(_logFolder);
            //Directory.CreateDirectory(_scansFolder);
            //Directory.CreateDirectory(_targetDecoyFolder);
            //Directory.CreateDirectory(_uniqueFolder);

            if (phosphopeptideOutputs)
            {
               // Directory.CreateDirectory(_scansPhosphoFolder);
                //Directory.CreateDirectory(_targetDecoyPhosphoFolder);
               // Directory.CreateDirectory(_uniquePhosphoFolder);
               // if (overallOutputs)
                //    Directory.CreateDirectory(_overallPhosphoOutputFolder);
            }

           // if (overallOutputs)
            //{
                _overallLog = new StreamWriter(Path.Combine(_outputFolder, "FDR_Optimizer_log.txt"));
                _overallLog.AutoFlush = true;
                WriteToOverallLog("FDR Optimizer PARAMETERS");
                WriteToOverallLog("Fixed Modifications: " + GetModificationString());
                WriteToOverallLog("Higher Scores are Better: " + higherScoresAreBetter.ToString());
                WriteToOverallLog("Maximum False Discovery Rate (%): " + maximumFalseDiscoveryRate.ToString());
                WriteToOverallLog("FDR Calculation and Optimization Based on Unique Peptide Sequences: " + UseUniqueSequence.ToString());
                WriteToOverallLog();
          // }

            _summaryStreamWriter = new StreamWriter(Path.Combine(_outputFolder, "summary.csv"));
            _summaryStreamWriter.AutoFlush = true;
            _summaryStreamWriter.WriteLine("CSV Filepath,Raw Filepath,Preliminary E-Value Score Threshold,Preliminary Target Peptides,Preliminary Decoy Peptides,Preliminary FDR (%),Systematic (Median) Precursor Mass Error (ppm),Scans," + (phosphopeptideOutputs ? "Phosphopeptide Scans," : null) + "Q-Value Threshold (%),E-Value Score Threshold,Maximum Precursor Mass Error (ppm),Target Peptides,Decoy Peptides," + (phosphopeptideOutputs ? "Target Phosphopeptides,Decoy Phosphopeptides," : null) + (UseUniqueSequence ? null : "FDR (%),") + "Unique Target Peptides,Unique Decoy Peptides," + (phosphopeptideOutputs ? "Unique Target Phosphopeptides,Unique Decoy Phosphopeptides," : null) + (UseUniqueSequence ? "FDR (%)" : null));
        }

        public void Optimize()
        {
            try
            {
                Setup();

                if (IsBatched)
                {
                    BatchOptimize(csvFilepaths);
                }               
          

                // Read in each CSV File
                List<InputFile> csvFiles = ReadInCSVFiles(csvFilepaths, fixedModifications, 1).ToList();

                // Get precursor mass accuracy data if requested
                if (Is2DFDR)
                {
                    UpdatePsmInformation(csvFiles, rawFolder);
                }

                // Group similar files together to be analyzed together
                if (IsBatched)
                {
                    csvFiles = BatchFiles(csvFiles).ToList();
                }

                // Convert PSMs into Peptides for fdr calculations
                ReducePsms(csvFiles, uniquePeptideType);

                // Calculate FDR based on score first
                CalculateBasicFDR(csvFiles);

                // Calculate 2D FDR based on precursor mass error
                if (Is2DFDR)
                {
                    Calculate2dFdr(csvFiles);
                }

                WriteFiles(csvFiles);

            }
            catch(Exception ex)
            {
                onThrowException(new ExceptionEventArgs(ex));
            }
            finally
            {
                Cleanup();
            }
        }

        private void BatchOptimize(IList<string> csvFilepaths)
        {
            throw new NotImplementedException();
        }

        private void WriteFiles(IList<InputFile> csvFiles)
        {
            foreach (InputFile csvFile in csvFiles)
            {               
                string outputTargetFile = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_target.csv");
                string outputDecoyFile = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_decoy.csv");
                string outputScansFile = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_scans.csv");
                string outputTargetUniqueFile = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_target_unique.csv");
                string outputDecoyUniqueFile = Path.Combine(_outputFolder, Path.GetFileNameWithoutExtension(csvFile.FilePath) + "_decoy_unique.csv");
                Log("Writing output files for " + Path.GetFileNameWithoutExtension(csvFile.FilePath) + " in " + _outputFolder + "...");
                using (StreamWriter targetWriter = new StreamWriter(outputTargetFile), 
                    decoyWriter = new StreamWriter(outputDecoyFile),
                    scansWriter = new StreamWriter(outputScansFile),
                    targetUniqueWriter = new StreamWriter(outputTargetUniqueFile),
                    decoyUniqueWriter = new StreamWriter(outputDecoyUniqueFile))
                {                    
                    Dictionary<string, PeptideSpectralMatch> psms = csvFile.PeptideSpectralMatches.ToDictionary(psm => psm.FileName + psm.Peptide.Sequence);
                    HashSet<PeptideSpectralMatch> successfulPSMs = new HashSet<PeptideSpectralMatch>(csvFile.PeptideSpectralMatches.Where(psm => psm.Score <= csvFile.ScoreThreshold && Math.Abs(psm.CorrectedPrecursorMassError.Value) < csvFile.PrecursorMassToleranceThreshold.Value));
                    Dictionary<PeptideSpectralMatch, Peptide> successfulPeptides = csvFile.Peptides.Where(psm => psm.FdrScoreMetric <= csvFile.ScoreThreshold && Math.Abs(psm.CorrectedPrecursorErrorPPM) < csvFile.PrecursorMassToleranceThreshold.Value)
                        .ToDictionary(pep => pep.BestMatch);

                    HashSet<int> scansProcessed = new HashSet<int>();
                    StringBuilder sb = new StringBuilder();
                    using (CsvReader reader = new CsvReader(new StreamReader(csvFile.FilePath), true))
                    {
                        string[] headers = reader.GetFieldHeaders();
                        int headerCount = headers.Length;
                        int modsColumnIndex = reader.GetFieldIndex("Mods");
                        string[] data = new string[headerCount];
                        string headerLine = string.Join(",", headers) + "Precursor Isolation m/z,Precursor Isolation Mass (Da),Precursor Theoretical Neutral Mass (Da),Precursor Experimental Neutral Mass (Da),Precursor Mass Error (ppm),Adjusted Precursor Mass Error (ppm),Q-Value (%)";

                        decoyWriter.WriteLine(headerLine);
                        targetWriter.WriteLine(headerLine);
                        scansWriter.WriteLine(headerLine);
                        targetUniqueWriter.WriteLine(headerLine);
                        decoyUniqueWriter.WriteLine(headerLine);
                        while (reader.ReadNextRecord())
                        {
                            PeptideSpectralMatch psm;
                            int spectralNumber = int.Parse(reader["Spectrum number"]);
                            if (scansProcessed.Contains(spectralNumber))
                                continue;                            
                            string fileName = reader["Filename/id"];
                            string sequence = reader["Peptide"].ToUpper();
                        
                            if (psms.TryGetValue(fileName + sequence, out psm))
                            {
                                scansProcessed.Add(spectralNumber);
                                sb.Clear();
                                reader.CopyCurrentRecordTo(data);
                                for (int i = 0; i < headerCount; i++)
                                {
                                    string datum = data[i];

                                    if (IncludeFixedMods && i == modsColumnIndex)
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
                                sb.Append(Mass.MassFromMz(psm.IsolationMz, psm.Charge));
                                sb.Append(',');
                                sb.Append("n/a");
                                sb.Append(',');
                                sb.Append(psm.PrecursorMassError.Value);
                                sb.Append(',');
                                sb.Append(psm.CorrectedPrecursorMassError.Value);
                                sb.Append(',');
                                sb.Append("n/a");

                                string line = sb.ToString();

                                scansWriter.WriteLine(line);

                                // Passes FDR, write out
                                if (successfulPSMs.Contains(psm))
                                {
                                    if (psm.IsDecoy)
                                        decoyWriter.WriteLine(line);
                                    else
                                        targetWriter.WriteLine(line);


                                    Peptide pep;
                                    // Is this the best unique psm?
                                    if (successfulPeptides.TryGetValue(psm, out pep))
                                    {
                                        if (pep.IsDecoy)
                                            decoyUniqueWriter.WriteLine(line);
                                        else
                                            targetUniqueWriter.WriteLine(line);
                                    }
                                }
                            }
                        }
                    }                 
                }
            }
        }

        private void ReducePsms(IList<InputFile> csvFiles, UniquePeptideType uniquePeptideType)
        {
            string msg = "Converting PSMs into unique peptides based ";
            IEqualityComparer<Peptide> comparer;
            switch (uniquePeptideType)
            {
                default:
                case UniquePeptideType.Sequence:
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
                Log(string.Format("{0:N0} unique peptides remain from {1:N0} PSMs from {2}", csvFile.Peptides.Count, csvFile.PeptideSpectralMatches.Count, csvFile.FilePath));
            }
        }

        private void Calculate2dFdr(IList<InputFile> csvFiles, int steps = 20, double minimumIncrement = 0.1)
        {
            Log("Calculating second order FDR levels...");
            foreach (InputFile csvFile in csvFiles)
            {
                double bestppmError = 0;
                int bestTargets = 0;

                List<Peptide> peptides = csvFile.Peptides.OrderBy(pep => Math.Abs(pep.CorrectedPrecursorErrorPPM)).ToList();
                double[] precursorPPMs = peptides.Select(pep => Math.Abs(pep.CorrectedPrecursorErrorPPM)).ToArray();
             
                double maxPrecursorError = csvFile.MaximumPrecursorMassError;
                double minPrecursorError = 0;
                double increment = (maxPrecursorError - minPrecursorError)/steps;
                while (increment > minimumIncrement)
                {
                    for (double ppmError = minPrecursorError; ppmError <= maxPrecursorError; ppmError += increment)
                    {
                        int index = Array.BinarySearch(precursorPPMs, ppmError);
                        if(index < 0)
                            index = ~index;
                    
                        int targets =
                            FalseDiscoveryRate<Peptide, double>.Count(peptides.Take(index),
                                maximumFalseDiscoveryRate);
                        if (targets > bestTargets)
                        {
                            bestTargets = targets;
                            bestppmError = ppmError;
                        }
                    }
                    minPrecursorError = Math.Max(bestppmError - increment, 0);
                    maxPrecursorError = Math.Min(bestppmError + increment, csvFile.MaximumPrecursorMassError);
                    increment = (maxPrecursorError - minPrecursorError) / steps;
                }
                // 2d Threshold
                List<Peptide> peptides2 = new List<Peptide>(csvFile.Peptides.Where(pep => Math.Abs(pep.CorrectedPrecursorErrorPPM) <= bestppmError));
                double threshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(peptides2, maximumFalseDiscoveryRate);
                
                bestTargets = csvFile.PeptideSpectralMatches.Count(psm =>  psm.Score <= threshold && Math.Abs(psm.CorrectedPrecursorMassError.Value) <= bestppmError);
                List<Peptide> unique = new List<Peptide>(csvFile.Peptides.Where(pep =>  pep.FdrScoreMetric < threshold && Math.Abs(pep.CorrectedPrecursorErrorPPM) <= bestppmError));
                
                csvFile.ScoreThreshold = threshold;
                csvFile.PrecursorMassToleranceThreshold = MassTolerance.FromPPM(bestppmError);
                Log(string.Format("{0:N0} PSMs ({1:N0} unique) were found at {2:g2} PPM and score threshold {3:g5}", bestTargets, unique.Count, bestppmError, threshold));
            }
        }

        private void CalculateBasicFDR(IList<InputFile> csvFiles)
        {
            Log("Calculating first order FDR threshold...");

            foreach (InputFile csvFile in csvFiles)
            {
                csvFile.ScoreThreshold = FalseDiscoveryRate<Peptide, double>.CalculateThreshold(csvFile.Peptides,
                    maximumFalseDiscoveryRate);
                List<PeptideSpectralMatch> passingPSMs = csvFile.PeptideSpectralMatches.Where(psm => psm.Score <= csvFile.ScoreThreshold).ToList();
                List<Peptide> passingPeptides =
                    csvFile.Peptides.Where(pep => pep.FdrScoreMetric < csvFile.ScoreThreshold).ToList();
                int total = passingPSMs.Count;
                int decoys = passingPSMs.Count(psm => psm.IsDecoy);
                int targets = total - decoys;
                Log(string.Format("{0:N0} PSMs ({1:N0} decoys) {2:N0} unique pass the threshold of {3:G4} for {4}",
                    targets,
                    decoys,passingPeptides.Count, csvFile.ScoreThreshold,
                    csvFile.FilePath));
            }
        }

        private IEnumerable<InputFile> BatchFiles(IList<InputFile> csvFiles)
        {
            Log("Combining similiar csv files...");
            foreach (InputFile csvFile in csvFiles)
            {
                yield return csvFile;
            }
        }

        private void UpdatePsmInformation(IList<InputFile> csvFiles, string folder, bool useMedian = true)
        {
            Log("Determining Precursor Mass Error...");

            List<string> rawFileNames = Directory.EnumerateFiles(folder, "*.raw", SearchOption.AllDirectories).ToList();
       
            foreach (InputFile csvFile in csvFiles)
            {
                string csvName = Path.GetFileNameWithoutExtension(csvFile.FilePath);
                csvFile.RawFilePath = "";
                foreach (string file in rawFileNames)
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    if (!csvName.StartsWith(name))
                        continue;
                    csvFile.RawFilePath = file;
                    break;
                }
                if (string.IsNullOrEmpty(csvFile.RawFilePath))
                {
                    throw new ArgumentException("Cannot find the associated raw file for: "+csvFile.FilePath);
                }
            }
            

            // update the precursor mass error
            foreach (InputFile csvFile in csvFiles)
            {
                using (MSDataFile dataFile = new ThermoRawFile(csvFile.RawFilePath))
                {
                    dataFile.Open();
                    csvFile.UpdatePsmInformation(dataFile, useMedian);
                    Log(string.Format("{0:F2} ppm {1} precursor mass error from {2}",
                        csvFile.SystematicPrecursorMassError, useMedian ? "median" : "average", dataFile.FilePath));
                }
            }
        }

        private IEnumerable<InputFile> ReadInCSVFiles(IEnumerable<string> csvFilePaths, IList<Modification> fixedModifications, int numberOfTopHits = 1)
        {
            Log("Reading in Peptide Spectral Matches...");
            int totalPSMs = 0;
            int totalDecoyPSM = 0;
            int files = 0;
            foreach (var csvFile in csvFilePaths.Select(csvFilePath => new InputFile(csvFilePath)))
            {
                files++;
                csvFile.Read(fixedModifications, numberOfTopHits);
                int psms = csvFile.PeptideSpectralMatches.Count;
                int decoys = csvFile.PeptideSpectralMatches.Count(psm => psm.IsDecoy);
                totalPSMs += psms;
                totalDecoyPSM += decoys;
                Log(string.Format("{0:N0} PSMs ({1:N0} decoys {2:F1}%) were read in from {3}", psms, decoys,
                    100*(double)decoys/psms, csvFile.FilePath));
                yield return csvFile;
            }
            Log(string.Format("{0:N0} PSMs ({1:N0} decoys {2:F1}%) in total from {3:N0} files", totalPSMs, totalDecoyPSM, 100 * (double)totalDecoyPSM / totalPSMs, files));
        }

        //private void AnalyzeRawFile(RawFile rawFile)
        //{
        //    string raw_filepath = rawFile.FilePath;

        //    raw = (IXRawfile4)new XRawfile();
        //    raw.Open(raw_filepath);
        //    raw.SetCurrentController(0, 1);

        //    int first_scan_number = 1;
        //    raw.GetFirstSpectrumNumber(ref first_scan_number);

        //    foreach (string csv_filepath in rawFile.CsvFiles)
        //    {
        //        onStartingFile(new FilepathEventArgs(csv_filepath));

        //        onUpdateProgress(new ProgressEventArgs(0));

        //        FileInfo file_info = new FileInfo(csv_filepath);

        //        string log_filepath = Path.Combine(_logFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_log.txt");
        //        log = new StreamWriter(log_filepath);
        //        log.AutoFlush = true;

        //        log.WriteLine("FDR Optimizer PARAMETERS");
        //        log.WriteLine("Fixed Modifications: " + GetModificationString());
        //        log.WriteLine("Maximum Precursor Mass Error (ppm): ±" + maximumPrecursorMassError.ToString());
        //        log.WriteLine("Precursor Mass Error Increment (ppm): " + precursorMassErrorIncrement.ToString());
        //        log.WriteLine("Higher Scores are Better: " + higherScoresAreBetter.ToString());
        //        log.WriteLine("Maximum False Discovery Rate (%): " + maximumFalseDiscoveryRate.ToString());
        //        log.WriteLine("FDR Calculation and Optimization Based on Unique Peptide Sequences: " + UseUniqueSequence.ToString());
        //        log.WriteLine();

        //        if (overallOutputs)
        //        {
        //            WriteToOverallLog(raw_filepath);
        //            WriteToOverallLog();
        //        }
        //        log.WriteLine(raw_filepath);
        //        log.WriteLine();

        //        SortedDictionary<int, PeptideHit> scans_peptides = new SortedDictionary<int, PeptideHit>();

        //        if (overallOutputs)
        //        {
        //            WriteToOverallLog(csv_filepath);

        //            _summaryStreamWriter.Write(csv_filepath + ',');
        //            _summaryStreamWriter.Write(raw_filepath + ',');
        //        }
        //        log.WriteLine(csv_filepath);

        //        // DJB addition
        //        StreamReader csv_file = new StreamReader(csv_filepath);
        //        using (CsvReader reader = new CsvReader(csv_file, true))
        //        {
        //            string header_line = string.Join(",", reader.GetFieldHeaders());
        //            _extendedHeaderLine = header_line + ", Precursor Isolation m/z, Precursor Isolation Mass (Da), Precursor Theoretical Neutral Mass (Da), Precursor Experimental Neutral Mass (Da), Precursor Mass Error (ppm), Adjusted Precursor Mass Error (ppm), Q-Value (%)";
        //            if (overallOutputs && !_overallHeaderWritten)
        //            {
        //                overall_scans_output.WriteLine(_extendedHeaderLine);
        //                overall_target_output.WriteLine(_extendedHeaderLine);
        //                overall_decoy_output.WriteLine(_extendedHeaderLine);
        //                overall_target_unique_output.WriteLine(_extendedHeaderLine);
        //                overall_decoy_unique_output.WriteLine(_extendedHeaderLine);
        //                if (phosphopeptideOutputs)
        //                {
        //                    overall_scans_phospho_output.WriteLine(_extendedHeaderLine);
        //                    overall_target_phospho_output.WriteLine(_extendedHeaderLine);
        //                    overall_decoy_phospho_output.WriteLine(_extendedHeaderLine);
        //                    overall_target_unique_phospho_output.WriteLine(_extendedHeaderLine);
        //                    overall_decoy_unique_phospho_output.WriteLine(_extendedHeaderLine);
        //                }

        //                _overallHeaderWritten = true;
        //            }
        //            int counter = 0;
        //            string[] data = new string[reader.FieldCount];
        //            PeptideHit storedHit = null;
        //            int msn = 0;
        //            int lastMS1 = -1;
        //            int lastscan = -1;
        //            double header_mass = 0;
        //            double[,] labels = null;
        //            List<double> ms1Peaks = null;
        //            while (reader.ReadNextRecord())
        //            {
        //                int scan_number = int.Parse(reader["Spectrum number"]);

        //                // Check to see if there was an MS1 between this and the last record read
        //                if (scan_number > lastscan + 1)
        //                {
        //                    // Preprocess the MS1 scan
        //                    int ms1_scan = scan_number - 1;
        //                    while (ms1_scan > lastMS1)
        //                    {
        //                        raw.GetMSOrderForScanNum(ms1_scan, ref msn);
        //                        if (msn == 1)
        //                        {
        //                            break;
        //                        }
        //                        ms1_scan--;
        //                    }
        //                    if (lastMS1 != ms1_scan)
        //                    {
        //                        lastMS1 = ms1_scan;
        //                        object labels_obj = null;
        //                        object flags_obj = null;
        //                        raw.GetLabelData(ref labels_obj, ref flags_obj, ref lastMS1);
        //                        labels = (double[,])labels_obj;
        //                        ms1Peaks = new List<double>(labels.GetUpperBound(1) + 1);
        //                        int max = labels.GetUpperBound(1);
        //                        for (int i = labels.GetLowerBound(1); i <= max; i++)
        //                        {
        //                            ms1Peaks.Add(labels[(int)RawLabelDataColumn.MZ, i]);
        //                        }
        //                    }
        //                }
        //                // Save this scan number
        //                lastscan = scan_number;

        //                // Read in the e-value
        //                double evalue_score = double.Parse(reader["E-value"]);

        //                // Read in the defline and see if it is a decoy peptide
        //                string defline = reader["Defline"];
        //                bool decoy = defline.StartsWith("DECOY") || defline.StartsWith("REVERSED");

        //                // Have we already processed a peptide for this scan number?
        //                bool containsSNAlready = false;
        //                if (containsSNAlready = scans_peptides.TryGetValue(scan_number, out storedHit))
        //                {
        //                    // We have proceessed a peptide for this scan number before, retrieve that result
        //                    double storedEvalue = storedHit.EValueScore;
        //                    bool storedDecoy = storedHit.IsDecoy;

        //                    // If the current evalue equals the stored value, and the stored value is a decoy, keep the decoy
        //                    if (evalue_score.Equals(storedEvalue) && storedDecoy)
        //                    {
        //                        continue; // If scores are equivalent, keep the decoy on the stack (more conservative)
        //                    }

        //                    if (higherScoresAreBetter)
        //                    {
        //                        // Higher scores are better, so if the current score is lower the the stored score, skip it
        //                        if (evalue_score < storedEvalue) continue;
        //                    }
        //                    else
        //                    {
        //                        // Lower scores are better, so if the current score is greater the the stored score, skip it
        //                        if (evalue_score > storedEvalue) continue;
        //                    }
        //                }

        //                string sequence = reader["Peptide"];
        //                string dynamic_modifications = reader["Mods"];
        //                int charge = int.Parse(reader["Charge"]);
        //                Peptide peptide = new Peptide(sequence, fixedModifications, dynamic_modifications);

        //                StringBuilder sb = new StringBuilder();
        //                for (int i = 0; i < reader.FieldCount; i++)
        //                {
        //                    sb.Append(reader[i].Replace(',', ';'));
        //                    sb.Append(',');
        //                }
        //                sb.Remove(sb.Length - 1, 1);
        //                string line = sb.ToString();
        //                raw.GetPrecursorMassForScanNum(scan_number, 2, ref header_mass);
        //                PeptideHit hit = new PeptideHit(line, sequence, dynamic_modifications, evalue_score, decoy, peptide, ms1Peaks, header_mass, charge);

        //                if (containsSNAlready)
        //                {
        //                    // We already processed this scan number before, so replace this hit
        //                    scans_peptides[scan_number] = hit;
        //                }
        //                else
        //                {
        //                    // We haven't process this scan number before, so add the hit
        //                    scans_peptides.Add(scan_number, hit);
        //                }

        //                if (dynamic_modifications.Contains("phosphorylation"))
        //                {
        //                    overall_scans_phospho++;
        //                }

        //                counter++;
        //                if (counter > 100)
        //                {
        //                    double progress = (double)csv_file.BaseStream.Position / csv_file.BaseStream.Length;
        //                    onUpdateProgress(new ProgressEventArgs((int)(progress * 100.0)));
        //                    counter = 0;
        //                }
        //            }
        //        }

        //        onUpdateProgress(new ProgressEventArgs(0));
                
        //        overall_scans += scans_peptides.Count;

        //        if (overallOutputs)
        //        {
        //            WriteToOverallLog();
        //        }
        //        log.WriteLine();

        //        List<PeptideHit> peptides = new List<PeptideHit>(scans_peptides.Values);
        //        peptides.Sort(new AscendingPeptideHitEValueScoreComparer(higherScoresAreBetter));
        //        int cumulative_target_peptides = 0;
        //        int cumulative_decoy_peptides = 0;
        //        double best_low_res_evalue_score_threshold = higherScoresAreBetter ? double.PositiveInfinity : double.NegativeInfinity;
        //        double best_low_res_false_discovery_rate = double.NaN;
        //        int best_target_peptides = 0;
        //        int best_decoy_peptides = 0;
        //        int p = 0;
        //        while (p < peptides.Count)
        //        {
        //            PeptideHit peptide = peptides[p];
        //            if (!peptide.IsDecoy)
        //            {
        //                cumulative_target_peptides++;
        //            }
        //            else
        //            {
        //                cumulative_decoy_peptides++;
        //            }

        //            p++;

        //            if (p < peptides.Count)
        //            {
        //                PeptideHit next_peptide = peptides[p];
        //                if (next_peptide.EValueScore == peptide.EValueScore)
        //                {
        //                    continue;
        //                }
        //            }

        //            double false_discovery_rate = (double)cumulative_decoy_peptides / cumulative_target_peptides * 100.0;
        //            if (false_discovery_rate <= MAXIMUM_FDR_FOR_SYSTEMATIC_PRECURSOR_MASS_ERROR)
        //            {
        //                if (cumulative_target_peptides > best_target_peptides
        //                    || (cumulative_target_peptides == best_target_peptides && false_discovery_rate < best_low_res_false_discovery_rate))
        //                {
        //                    best_low_res_evalue_score_threshold = peptide.EValueScore;
        //                    best_low_res_false_discovery_rate = false_discovery_rate;
        //                    best_target_peptides = cumulative_target_peptides;
        //                    best_decoy_peptides = cumulative_decoy_peptides;
        //                }
        //            }
        //        }

        //        List<double> precursor_mass_errors = new List<double>(cumulative_target_peptides);
        //        foreach (PeptideHit peptide in peptides)
        //        {
        //            if ((!higherScoresAreBetter && peptide.EValueScore > best_low_res_evalue_score_threshold)
        //                || (higherScoresAreBetter && peptide.EValueScore < best_low_res_evalue_score_threshold))
        //            {
        //                break;
        //            }

        //            if (!peptide.IsDecoy)
        //            {
        //                precursor_mass_errors.Add(peptide.PrecursorMassError);
        //            }
        //        }

        //        double median_precursor_mass_error = 0.0;
        //        if (precursor_mass_errors.Count > 0)
        //        {
        //            median_precursor_mass_error = CalculateMedian(precursor_mass_errors);
        //        }
        //        foreach (PeptideHit peptide in scans_peptides.Values)
        //        {
        //            peptide.AdjustedPrecursorMassError = peptide.PrecursorMassError - median_precursor_mass_error;
        //        }

        //        int q_cumulative_target_peptides = 0;
        //        int q_cumulative_decoy_peptides = 0;
        //        Dictionary<string, int> q_target_peptides = new Dictionary<string, int>();
        //        Dictionary<string, int> q_decoy_peptides = new Dictionary<string, int>();
        //        int p3 = 0;
        //        while (p3 < peptides.Count)
        //        {
        //            PeptideHit peptide = peptides[p3];
        //            if (!peptide.IsDecoy)
        //            {
        //                q_cumulative_target_peptides++;
        //                if (!q_target_peptides.ContainsKey(peptide.Sequence))
        //                {
        //                    q_target_peptides.Add(peptide.Sequence, 0);
        //                }
        //                q_target_peptides[peptide.Sequence]++;
        //            }
        //            else
        //            {
        //                q_cumulative_decoy_peptides++;
        //                if (!q_decoy_peptides.ContainsKey(peptide.Sequence))
        //                {
        //                    q_decoy_peptides.Add(peptide.Sequence, 0);
        //                }
        //                q_decoy_peptides[peptide.Sequence]++;
        //            }

        //            p3++;

        //            if (p3 < peptides.Count)
        //            {
        //                PeptideHit next_peptide = peptides[p3];
        //                if (next_peptide.EValueScore == peptide.EValueScore)
        //                {
        //                    continue;
        //                }
        //            }

        //            int current_target_peptides = UseUniqueSequence ? q_target_peptides.Count : q_cumulative_target_peptides;
        //            int current_decoy_peptides = UseUniqueSequence ? q_decoy_peptides.Count : q_cumulative_decoy_peptides;
        //            double q_value = (double)current_decoy_peptides / current_target_peptides * 100.0;
        //            peptide.QValue = q_value;

        //            int p4 = p3 - 1 - 1;
        //            while (p4 >= 0)
        //            {
        //                if (double.IsNaN(peptides[p4].QValue))
        //                {
        //                    peptides[p4].QValue = q_value;
        //                    p4--;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        int p5 = peptides.Count - 1 - 1;
        //        while (p5 >= 0)
        //        {
        //            if (peptides[p5].QValue > peptides[p5 + 1].QValue)
        //            {
        //                peptides[p5].QValue = peptides[p5 + 1].QValue;
        //            }
        //            p5--;
        //        }

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.Write(best_low_res_evalue_score_threshold.ToString() + ',');
        //            _summaryStreamWriter.Write(best_target_peptides.ToString() + ',');
        //            _summaryStreamWriter.Write(best_decoy_peptides.ToString() + ',');
        //            _summaryStreamWriter.Write(best_low_res_false_discovery_rate.ToString() + ',');
        //            _summaryStreamWriter.Write(median_precursor_mass_error.ToString() + ',');
        //        }

        //        log.WriteLine("Preliminary E-Value Score Threshold: " + best_low_res_evalue_score_threshold.ToString());
        //        log.WriteLine("Preliminary Target Peptides: " + best_target_peptides.ToString());
        //        log.WriteLine("Preliminary Decoy Peptides: " + best_decoy_peptides.ToString());
        //        log.WriteLine("Preliminary FDR (%): " + best_low_res_false_discovery_rate.ToString());
        //        log.WriteLine("Systematic (Median) Precursor Mass Error (ppm): " + median_precursor_mass_error.ToString());
        //        log.WriteLine();

        //        string scans_filepath = Path.Combine(_scansFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_scans.csv");
        //        scans_output = new StreamWriter(scans_filepath);
        //        scans_output.WriteLine(_extendedHeaderLine);

        //        string scans_phospho_filepath = Path.Combine(_scansPhosphoFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_scans_phospho.csv");
        //        if (phosphopeptideOutputs)
        //        {
        //            scans_phospho_output = new StreamWriter(scans_phospho_filepath);
        //            scans_phospho_output.WriteLine(_extendedHeaderLine);
        //        }

        //        int scans_phospho = 0;
        //        foreach (PeptideHit peptide in scans_peptides.Values)
        //        {
        //            if (overallOutputs)
        //            {
        //                overall_scans_output.WriteLine(peptide.ExtendedLine);
        //            }
        //            scans_output.WriteLine(peptide.ExtendedLine);
        //            if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
        //            {
        //                scans_phospho++;
        //                if (overallOutputs)
        //                {
        //                    overall_scans_phospho_output.WriteLine(peptide.ExtendedLine);
        //                }
        //                scans_phospho_output.WriteLine(peptide.ExtendedLine);
        //            }
        //        }

        //        scans_output.Close();
        //        if (phosphopeptideOutputs)
        //        {
        //            scans_phospho_output.Close();
        //        }

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.Write(scans_peptides.Count.ToString() + ',');
        //        }
        //        log.WriteLine(scans_filepath);
        //        log.WriteLine(scans_peptides.Count.ToString() + " MS/MS scans resulted in at least one peptide hit");
        //        if (phosphopeptideOutputs)
        //        {
        //            log.WriteLine(scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
        //            if (overallOutputs)
        //            {
        //                _summaryStreamWriter.Write(scans_phospho.ToString() + ',');
        //            }
        //        }
        //        log.WriteLine();

        //        peptides.Sort(new AscendingPeptideHitQValueComparer(higherScoresAreBetter));
        //        int best_target = 0;
        //        int best_decoy = 0;
        //        double best_false_discovery_rate = double.NaN;
        //        double best_q_value = double.NegativeInfinity;
        //        double best_evalue_score = higherScoresAreBetter ? double.PositiveInfinity : double.NegativeInfinity;
        //        double best_max_precursor_mass_error = 0.0;

        //        double max_precursor_mass_error = precursorMassErrorIncrement;
        //        while (max_precursor_mass_error <= maximumPrecursorMassError)
        //        {
        //            List<PeptideHit> filtered_peptides = new List<PeptideHit>();
        //            foreach (PeptideHit peptide in peptides)
        //            {
        //                if (Math.Abs(peptide.AdjustedPrecursorMassError) <= max_precursor_mass_error)
        //                {
        //                    filtered_peptides.Add(peptide);
        //                }
        //            }
        //            int target_count = 0;
        //            int decoy_count = 0;
        //            Dictionary<string, int> target_unique_count = new Dictionary<string, int>();
        //            Dictionary<string, int> decoy_unique_count = new Dictionary<string, int>();
        //            int p2 = 0;
        //            int value = 0;
        //            while (p2 < filtered_peptides.Count)
        //            {
        //                PeptideHit peptide = filtered_peptides[p2];
        //                string sequence = peptide.Sequence;
        //                if (!peptide.IsDecoy)
        //                {
        //                    target_count++;
        //                    if (target_unique_count.TryGetValue(sequence, out value))
        //                    {
        //                        target_unique_count[sequence] = value + 1;
        //                    }
        //                    else
        //                    {
        //                        target_unique_count.Add(sequence, 1);
        //                    }
        //                }
        //                else
        //                {
        //                    decoy_count++;
        //                    if (decoy_unique_count.TryGetValue(sequence, out value))
        //                    {
        //                        decoy_unique_count[sequence] = value + 1;
        //                    }
        //                    else
        //                    {
        //                        decoy_unique_count.Add(sequence, 1);
        //                    }
        //                }

        //                p2++;

        //                if (p2 < filtered_peptides.Count)
        //                {
        //                    PeptideHit next_peptide = filtered_peptides[p2];
        //                    if (next_peptide.QValue == peptide.QValue && next_peptide.EValueScore == peptide.EValueScore)
        //                    {
        //                        continue;
        //                    }
        //                }

        //                int target = UseUniqueSequence ? target_unique_count.Count : target_count;
        //                int decoy = UseUniqueSequence ? decoy_unique_count.Count : decoy_count;
        //                double false_discovery_rate = (double)decoy / target * 100.0;
        //                if (false_discovery_rate <= maximumFalseDiscoveryRate)
        //                {
        //                    if (target > best_target || (target == best_target && false_discovery_rate < best_false_discovery_rate))
        //                    {
        //                        best_target = target;
        //                        best_decoy = decoy;
        //                        best_false_discovery_rate = false_discovery_rate;
        //                        best_q_value = peptide.QValue;
        //                        best_evalue_score = peptide.EValueScore;
        //                        best_max_precursor_mass_error = max_precursor_mass_error;
        //                    }
        //                }
        //            }

        //            max_precursor_mass_error += precursorMassErrorIncrement;
        //        }

        //        string target_filepath = Path.Combine(_targetDecoyFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target.csv");
        //        target_output = new StreamWriter(target_filepath);
        //        target_output.WriteLine(_extendedHeaderLine);
        //        string decoy_filepath = Path.Combine(_targetDecoyFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy.csv");
        //        decoy_output = new StreamWriter(decoy_filepath);
        //        decoy_output.WriteLine(_extendedHeaderLine);

        //        string target_phospho_filepath = Path.Combine(_targetDecoyPhosphoFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_phospho.csv");
        //        string decoy_phospho_filepath = Path.Combine(_targetDecoyPhosphoFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_phospho.csv");
        //        if (phosphopeptideOutputs)
        //        {
        //            target_phospho_output = new StreamWriter(target_phospho_filepath);
        //            target_phospho_output.WriteLine(_extendedHeaderLine);
        //            decoy_phospho_output = new StreamWriter(decoy_phospho_filepath);
        //            decoy_phospho_output.WriteLine(_extendedHeaderLine);
        //        }

        //        SortedDictionary<string, PeptideHit> target_unique = new SortedDictionary<string, PeptideHit>();
        //        SortedDictionary<string, PeptideHit> decoy_unique = new SortedDictionary<string, PeptideHit>();
        //        int redundant_target = 0;
        //        int redundant_decoy = 0;
        //        int redundant_target_phospho = 0;
        //        int redundant_decoy_phospho = 0;
        //        foreach (PeptideHit peptide in scans_peptides.Values)
        //        {
        //            if ((peptide.QValue < best_q_value || (peptide.QValue == best_q_value
        //                && ((!higherScoresAreBetter && peptide.EValueScore <= best_evalue_score)
        //                || (higherScoresAreBetter && peptide.EValueScore >= best_evalue_score))))
        //                && Math.Abs(peptide.AdjustedPrecursorMassError) <= best_max_precursor_mass_error)
        //            {
        //                if (!peptide.IsDecoy)
        //                {
        //                    redundant_target++;
        //                    if (overallOutputs)
        //                    {
        //                        overall_target_output.WriteLine(peptide.ExtendedLine);
        //                    }
        //                    target_output.WriteLine(peptide.ExtendedLine);
        //                    if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
        //                    {
        //                        redundant_target_phospho++;
        //                        if (overallOutputs)
        //                        {
        //                            overall_target_phospho_output.WriteLine(peptide.ExtendedLine);
        //                        }
        //                        target_phospho_output.WriteLine(peptide.ExtendedLine);
        //                    }

        //                    if (!target_unique.ContainsKey(peptide.Sequence))
        //                    {
        //                        target_unique.Add(peptide.Sequence, peptide);
        //                    }
        //                    else if (peptide.QValue < target_unique[peptide.Sequence].QValue
        //                        || (peptide.QValue == target_unique[peptide.Sequence].QValue
        //                        && ((!higherScoresAreBetter && peptide.EValueScore < target_unique[peptide.Sequence].EValueScore)
        //                        || (higherScoresAreBetter && peptide.EValueScore > target_unique[peptide.Sequence].EValueScore)))
        //                        || (peptide.QValue == target_unique[peptide.Sequence].QValue
        //                        && peptide.EValueScore == target_unique[peptide.Sequence].EValueScore
        //                        && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(target_unique[peptide.Sequence].AdjustedPrecursorMassError)))
        //                    {
        //                        target_unique[peptide.Sequence] = peptide;
        //                    }
        //                }
        //                else
        //                {
        //                    redundant_decoy++;
        //                    if (overallOutputs)
        //                    {
        //                        overall_decoy_output.WriteLine(peptide.ExtendedLine);
        //                    }
        //                    decoy_output.WriteLine(peptide.ExtendedLine);
        //                    if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
        //                    {
        //                        redundant_decoy_phospho++;
        //                        if (overallOutputs)
        //                        {
        //                            overall_decoy_phospho_output.WriteLine(peptide.ExtendedLine);
        //                        }
        //                        decoy_phospho_output.WriteLine(peptide.ExtendedLine);
        //                    }

        //                    if (!decoy_unique.ContainsKey(peptide.Sequence))
        //                    {
        //                        decoy_unique.Add(peptide.Sequence, peptide);
        //                    }
        //                    else if (peptide.QValue < decoy_unique[peptide.Sequence].QValue
        //                        || (peptide.QValue == decoy_unique[peptide.Sequence].QValue
        //                        && ((!higherScoresAreBetter && peptide.EValueScore < decoy_unique[peptide.Sequence].EValueScore)
        //                        || (higherScoresAreBetter && peptide.EValueScore > decoy_unique[peptide.Sequence].EValueScore)))
        //                        || (peptide.QValue == decoy_unique[peptide.Sequence].QValue
        //                        && peptide.EValueScore == decoy_unique[peptide.Sequence].EValueScore
        //                        && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(decoy_unique[peptide.Sequence].AdjustedPrecursorMassError)))
        //                    {
        //                        decoy_unique[peptide.Sequence] = peptide;
        //                    }
        //                }
        //            }
        //        }

        //        overall_target += redundant_target;
        //        overall_decoy += redundant_decoy;
        //        overall_target_phospho += redundant_target_phospho;
        //        overall_decoy_phospho += redundant_decoy_phospho;

        //        target_output.Close();
        //        decoy_output.Close();
        //        if (phosphopeptideOutputs)
        //        {
        //            target_phospho_output.Close();
        //            decoy_phospho_output.Close();
        //        }

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.Write(best_q_value.ToString() + ',');
        //            _summaryStreamWriter.Write(best_evalue_score.ToString() + ',');
        //            _summaryStreamWriter.Write(best_max_precursor_mass_error.ToString() + ',');
        //        }

        //        log.WriteLine("Q-Value Threshold (%): " + best_q_value.ToString());
        //        log.WriteLine("E-Value Score Threshold: " + best_evalue_score.ToString());
        //        log.WriteLine("Maximum Precursor Mass Error (ppm): ±" + best_max_precursor_mass_error.ToString());
        //        log.WriteLine();

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.Write(redundant_target.ToString() + ',');
        //            _summaryStreamWriter.Write(redundant_decoy.ToString() + ',');
        //        }

        //        log.WriteLine(target_filepath);
        //        log.WriteLine(redundant_target.ToString() + " target peptides after FDR optimization");
        //        log.WriteLine(decoy_filepath);
        //        log.WriteLine(redundant_decoy.ToString() + " decoy peptides after FDR optimization");

        //        if (phosphopeptideOutputs)
        //        {
        //            if (overallOutputs)
        //            {
        //                _summaryStreamWriter.Write(redundant_target_phospho.ToString() + ',');
        //                _summaryStreamWriter.Write(redundant_decoy_phospho.ToString() + ',');
        //            }

        //            log.WriteLine(target_phospho_filepath);
        //            log.WriteLine(redundant_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
        //            log.WriteLine(decoy_phospho_filepath);
        //            log.WriteLine(redundant_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
        //        }

        //        log.WriteLine();

        //        if (!UseUniqueSequence)
        //        {
        //            if (overallOutputs)
        //            {
        //                _summaryStreamWriter.Write(((double)redundant_decoy / redundant_target * 100.0).ToString() + ',');
        //            }

        //            log.WriteLine("FDR (%): " + ((double)redundant_decoy / redundant_target * 100.0).ToString());
        //            log.WriteLine();
        //        }

        //        string target_unique_filepath = Path.Combine(_uniqueFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_unique.csv");
        //        target_unique_output = new StreamWriter(target_unique_filepath);
        //        target_unique_output.WriteLine(_extendedHeaderLine);
        //        string decoy_unique_filepath = Path.Combine(_uniqueFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_unique.csv");
        //        decoy_unique_output = new StreamWriter(decoy_unique_filepath);
        //        decoy_unique_output.WriteLine(_extendedHeaderLine);

        //        string target_unique_phospho_filepath = Path.Combine(_uniquePhosphoFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_unique_phospho.csv");
        //        string decoy_unique_phospho_filepath = Path.Combine(_uniquePhosphoFolder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_unique_phospho.csv");
        //        if (phosphopeptideOutputs)
        //        {
        //            target_unique_phospho_output = new StreamWriter(target_unique_phospho_filepath);
        //            target_unique_phospho_output.WriteLine(_extendedHeaderLine);
        //            decoy_unique_phospho_output = new StreamWriter(decoy_unique_phospho_filepath);
        //            decoy_unique_phospho_output.WriteLine(_extendedHeaderLine);
        //        }

        //        int target_unique_phospho = 0;
        //        foreach (PeptideHit peptide in target_unique.Values)
        //        {
        //            if (overallOutputs)
        //            {
        //                overall_target_unique_output.WriteLine(peptide.ExtendedLine);
        //            }
        //            target_unique_output.WriteLine(peptide.ExtendedLine);
        //            if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
        //            {
        //                target_unique_phospho++;
        //                if (overallOutputs)
        //                {
        //                    overall_target_unique_phospho_output.WriteLine(peptide.ExtendedLine);
        //                }
        //                target_unique_phospho_output.WriteLine(peptide.ExtendedLine);
        //            }

        //            if (!overall_target_peptides.ContainsKey(peptide.Sequence))
        //            {
        //                overall_target_peptides.Add(peptide.Sequence, peptide);
        //            }
        //            else if (peptide.QValue < overall_target_peptides[peptide.Sequence].QValue
        //                || (peptide.QValue == overall_target_peptides[peptide.Sequence].QValue
        //                && ((!higherScoresAreBetter && peptide.EValueScore < overall_target_peptides[peptide.Sequence].EValueScore)
        //                || (higherScoresAreBetter && peptide.EValueScore > overall_target_peptides[peptide.Sequence].EValueScore)))
        //                || (peptide.QValue == overall_target_peptides[peptide.Sequence].QValue
        //                && peptide.EValueScore == overall_target_peptides[peptide.Sequence].EValueScore
        //                && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(overall_target_peptides[peptide.Sequence].AdjustedPrecursorMassError)))
        //            {
        //                overall_target_peptides[peptide.Sequence] = peptide;
        //            }
        //        }
        //        overall_target_unique += target_unique.Count;
        //        overall_target_unique_phospho += target_unique_phospho;

        //        target_unique_output.Close();
        //        if (phosphopeptideOutputs)
        //        {
        //            target_unique_phospho_output.Close();
        //        }

        //        int decoy_unique_phospho = 0;
        //        foreach (PeptideHit peptide in decoy_unique.Values)
        //        {
        //            if (overallOutputs)
        //            {
        //                overall_decoy_unique_output.WriteLine(peptide.ExtendedLine);
        //            }
        //            decoy_unique_output.WriteLine(peptide.ExtendedLine);
        //            if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
        //            {
        //                decoy_unique_phospho++;
        //                if (overallOutputs)
        //                {
        //                    overall_decoy_unique_phospho_output.WriteLine(peptide.ExtendedLine);
        //                }
        //                decoy_unique_phospho_output.WriteLine(peptide.ExtendedLine);
        //            }

        //            if (!overall_decoy_peptides.ContainsKey(peptide.Sequence))
        //            {
        //                overall_decoy_peptides.Add(peptide.Sequence, peptide);
        //            }
        //            else if (peptide.QValue < overall_decoy_peptides[peptide.Sequence].QValue
        //                || (peptide.QValue == overall_decoy_peptides[peptide.Sequence].QValue
        //                && ((!higherScoresAreBetter && peptide.EValueScore < overall_decoy_peptides[peptide.Sequence].EValueScore)
        //                || (higherScoresAreBetter && peptide.EValueScore > overall_decoy_peptides[peptide.Sequence].EValueScore)))
        //                || (peptide.QValue == overall_decoy_peptides[peptide.Sequence].QValue
        //                && peptide.EValueScore == overall_decoy_peptides[peptide.Sequence].EValueScore
        //                && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(overall_decoy_peptides[peptide.Sequence].AdjustedPrecursorMassError)))
        //            {
        //                overall_decoy_peptides[peptide.Sequence] = peptide;
        //            }
        //        }
        //        overall_decoy_unique += decoy_unique.Count;
        //        overall_decoy_unique_phospho += decoy_unique_phospho;

        //        decoy_unique_output.Close();
        //        if (phosphopeptideOutputs)
        //        {
        //            decoy_unique_phospho_output.Close();
        //        }

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.Write(target_unique.Count.ToString() + ',');
        //            _summaryStreamWriter.Write(decoy_unique.Count.ToString() + ',');
        //        }

        //        log.WriteLine(target_unique_filepath);
        //        log.WriteLine(target_unique.Count.ToString() + " unique target peptide sequences after FDR optimization");
        //        log.WriteLine(decoy_unique_filepath);
        //        log.WriteLine(decoy_unique.Count.ToString() + " unique decoy peptide sequences after FDR optimization");

        //        if (phosphopeptideOutputs)
        //        {
        //            if (overallOutputs)
        //            {
        //                _summaryStreamWriter.Write(target_unique_phospho.ToString() + ',');
        //                _summaryStreamWriter.Write(decoy_unique_phospho.ToString() + ',');
        //            }

        //            log.WriteLine(target_unique_phospho_filepath);
        //            log.WriteLine(target_unique_phospho.ToString() + " unique target phosphopeptide sequences after FDR optimization");
        //            log.WriteLine(decoy_unique_phospho_filepath);
        //            log.WriteLine(decoy_unique_phospho.ToString() + " unique decoy phosphopeptide sequences after FDR optimization");
        //        }

        //        if (UseUniqueSequence)
        //        {
        //            if (overallOutputs)
        //            {
        //                _summaryStreamWriter.Write(((double)decoy_unique.Count / target_unique.Count * 100.0).ToString() + ',');
        //            }

        //            log.WriteLine();
        //            log.WriteLine("FDR (%): " + ((double)decoy_unique.Count / target_unique.Count * 100.0).ToString());
        //        }

        //        log.Close();

        //        if (overallOutputs)
        //        {
        //            _summaryStreamWriter.WriteLine();
        //        }

        //        onFinishedFile(new FilepathEventArgs(csv_filepath));
        //    }

        //    raw.Close();
        //}

        private void WriteOverallOutputs()
        {
            _summaryStreamWriter.Write("SUM,");

            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");

            string overall_target_unique_unique_filepath = Path.Combine(_outputFolder, "target_unique_unique.csv");
            overall_target_unique_unique_output = new StreamWriter(overall_target_unique_unique_filepath);
            overall_target_unique_unique_output.WriteLine(_extendedHeaderLine);
            string overall_decoy_unique_unique_filepath = Path.Combine(_outputFolder, "decoy_unique_unique.csv");
            overall_decoy_unique_unique_output = new StreamWriter(overall_decoy_unique_unique_filepath);
            overall_decoy_unique_unique_output.WriteLine(_extendedHeaderLine);

            string overall_target_unique_unique_phospho_filepath = Path.Combine(_overallPhosphoOutputFolder, "target_unique_unique_phospho.csv");
            string overall_decoy_unique_unique_phospho_filepath = Path.Combine(_overallPhosphoOutputFolder, "decoy_unique_unique_phospho.csv");
            if (phosphopeptideOutputs)
            {
                overall_target_unique_unique_phospho_output = new StreamWriter(overall_target_unique_unique_phospho_filepath);
                overall_target_unique_unique_phospho_output.WriteLine(_extendedHeaderLine);
                overall_decoy_unique_unique_phospho_output = new StreamWriter(overall_decoy_unique_unique_phospho_filepath);
                overall_decoy_unique_unique_phospho_output.WriteLine(_extendedHeaderLine);
            }

            foreach (PeptideHit peptide in overall_target_peptides.Values)
            {
                overall_target_unique_unique_output.WriteLine(peptide.ExtendedLine);
                if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                {
                    overall_target_unique_unique_phospho++;
                    overall_target_unique_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                }
            }

            foreach (PeptideHit peptide in overall_decoy_peptides.Values)
            {
                overall_decoy_unique_unique_output.WriteLine(peptide.ExtendedLine);
                if (phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                {
                    overall_decoy_unique_unique_phospho++;
                    overall_decoy_unique_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                }
            }

            overall_scans_output.Close();
            overall_target_output.Close();
            overall_decoy_output.Close();
            overall_target_unique_output.Close();
            overall_decoy_unique_output.Close();
            overall_target_unique_unique_output.Close();
            overall_decoy_unique_unique_output.Close();
            if (phosphopeptideOutputs)
            {
                overall_scans_phospho_output.Close();
                overall_target_phospho_output.Close();
                overall_decoy_phospho_output.Close();
                overall_target_unique_phospho_output.Close();
                overall_decoy_unique_phospho_output.Close();
                overall_target_unique_unique_phospho_output.Close();
                overall_decoy_unique_unique_phospho_output.Close();
            }

            WriteToOverallLog(_overallScansFilepath);
            WriteToOverallLog(overall_scans.ToString() + " MS/MS scans resulted in at least one peptide hit");
            _summaryStreamWriter.Write(overall_scans.ToString() + ',');

            if (phosphopeptideOutputs)
            {
                WriteToOverallLog(overall_scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
                _summaryStreamWriter.Write(overall_scans_phospho.ToString() + ',');
            }

            WriteToOverallLog();

            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");

            WriteToOverallLog(_overallTargetFilepath);
            WriteToOverallLog(overall_target.ToString() + " target peptides after FDR optimization");
            _summaryStreamWriter.Write(overall_target.ToString() + ',');
            WriteToOverallLog(_overallDecoyFilepath);
            WriteToOverallLog(overall_decoy.ToString() + " decoy peptides after FDR optimization");
            _summaryStreamWriter.Write(overall_decoy.ToString() + ',');

            if (phosphopeptideOutputs)
            {
                WriteToOverallLog(_overallTargetPhosphoFilepath);
                WriteToOverallLog(overall_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
                _summaryStreamWriter.Write(overall_target_phospho.ToString() + ',');
                WriteToOverallLog(_overallDecoyPhosphoFilepath);
                WriteToOverallLog(overall_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
                _summaryStreamWriter.Write(overall_decoy_phospho.ToString() + ',');
            }

            WriteToOverallLog();

            if (!UseUniqueSequence)
            {
                WriteToOverallLog("FDR (%): " + ((double)overall_decoy / overall_target * 100.0).ToString());
                _summaryStreamWriter.Write(((double)overall_decoy / overall_target * 100.0).ToString() + ',');
                WriteToOverallLog();
            }

            WriteToOverallLog(_overallTargetUniqueFilepath);
            WriteToOverallLog(overall_target_unique.ToString() + " target unique peptide sequences after FDR optimization");
            _summaryStreamWriter.Write(overall_target_unique.ToString() + ',');
            WriteToOverallLog(_overallDecoyUniqueFilepath);
            WriteToOverallLog(overall_decoy_unique.ToString() + " decoy unique peptide sequences after FDR optimization");
            _summaryStreamWriter.Write(overall_decoy_unique.ToString() + ',');

            if (phosphopeptideOutputs)
            {
                WriteToOverallLog(_overallTargetUniquePhosphoFilepath);
                WriteToOverallLog(overall_target_unique_phospho.ToString() + " target unique phosphopeptide sequences after FDR optimization");
                _summaryStreamWriter.Write(overall_target_unique_phospho.ToString() + ',');
                WriteToOverallLog(_overallDecoyUniquePhosphoFilepath);
                WriteToOverallLog(overall_decoy_unique_phospho.ToString() + " decoy unique phosphopeptide sequences after FDR optimization");
                _summaryStreamWriter.Write(overall_decoy_unique_phospho.ToString() + ',');
            }

            WriteToOverallLog();

            if (UseUniqueSequence)
            {
                WriteToOverallLog("FDR (%): " + ((double)overall_decoy_unique / overall_target_unique * 100.0).ToString());
                _summaryStreamWriter.Write(((double)overall_decoy_unique / overall_target_unique * 100.0).ToString() + ',');
                WriteToOverallLog();
            }

            _summaryStreamWriter.WriteLine();

            _summaryStreamWriter.Write("OVERALL,");

            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            _summaryStreamWriter.Write("n/a,");
            if (!UseUniqueSequence)
            {
                _summaryStreamWriter.Write("n/a,");
            }

            if (phosphopeptideOutputs)
            {
                _summaryStreamWriter.Write("n/a,");
                _summaryStreamWriter.Write("n/a,");
            }

            WriteToOverallLog(overall_target_unique_unique_filepath);
            WriteToOverallLog(overall_target_peptides.Count.ToString() + " target unique unique peptide sequences after FDR optimization");
            _summaryStreamWriter.Write(overall_target_peptides.Count.ToString() + ',');
            WriteToOverallLog(overall_decoy_unique_unique_filepath);
            WriteToOverallLog(overall_decoy_peptides.Count.ToString() + " decoy unique unique peptide sequences after FDR optimization");
            _summaryStreamWriter.Write(overall_decoy_peptides.Count.ToString() + ',');

            if (phosphopeptideOutputs)
            {
                WriteToOverallLog(overall_target_unique_unique_phospho_filepath);
                WriteToOverallLog(overall_target_unique_unique_phospho.ToString() + " target unique unique phosphopeptide sequences after FDR optimization");
                _summaryStreamWriter.Write(overall_target_unique_unique_phospho.ToString() + ',');
                WriteToOverallLog(overall_decoy_unique_unique_phospho_filepath);
                WriteToOverallLog(overall_decoy_unique_unique_phospho.ToString() + " decoy unique unique phosphopeptide sequences after FDR optimization");
                _summaryStreamWriter.Write(overall_decoy_unique_unique_phospho.ToString() + ',');
            }

            if (UseUniqueSequence)
            {
                _summaryStreamWriter.Write("n/a");
            }

            _summaryStreamWriter.WriteLine();

            _overallLog.Close();

            _summaryStreamWriter.Close();
        }
        
        private void Cleanup()
        {
            Log("Finished");
            onFinished(new EventArgs());
            if (overall_scans_output != null)
            {
                overall_scans_output.Close();
            }
            if (overall_target_output != null)
            {
                overall_target_output.Close();
            }
            if (overall_decoy_output != null)
            {
                overall_decoy_output.Close();
            }
            if (overall_target_unique_output != null)
            {
                overall_target_unique_output.Close();
            }
            if (overall_decoy_unique_output != null)
            {
                overall_decoy_unique_output.Close();
            }
            if (overall_scans_phospho_output != null)
            {
                overall_scans_phospho_output.Close();
            }
            if (overall_target_phospho_output != null)
            {
                overall_target_phospho_output.Close();
            }
            if (overall_decoy_phospho_output != null)
            {
                overall_decoy_phospho_output.Close();
            }
            if (overall_target_unique_phospho_output != null)
            {
                overall_target_unique_phospho_output.Close();
            }
            if (overall_decoy_unique_phospho_output != null)
            {
                overall_decoy_unique_phospho_output.Close();
            }
            if (_overallLog != null)
            {
                _overallLog.Dispose();
            }
            if (log != null)
            {
                log.Close();
            }
            if (_summaryStreamWriter != null)
            {
                _summaryStreamWriter.Close();
            }
            if (csv != null)
            {
                csv.Close();
            }
            if (scans_output != null)
            {
                scans_output.Close();
            }
            if (target_output != null)
            {
                target_output.Close();
            }
            if (decoy_output != null)
            {
                decoy_output.Close();
            }
            if (target_phospho_output != null)
            {
                target_phospho_output.Close();
            }
            if (decoy_phospho_output != null)
            {
                decoy_phospho_output.Close();
            }
            if (target_unique_output != null)
            {
                target_unique_output.Close();
            }
            if (decoy_unique_output != null)
            {
                decoy_unique_output.Close();
            }
            if (target_unique_phospho_output != null)
            {
                target_unique_phospho_output.Close();
            }
            if (decoy_unique_phospho_output != null)
            {
                decoy_unique_phospho_output.Close();
            }
            if (overall_target_unique_unique_output != null)
            {
                overall_target_unique_unique_output.Close();
            }
            if (overall_decoy_unique_unique_output != null)
            {
                overall_decoy_unique_unique_output.Close();
            }
            if (overall_target_unique_unique_phospho_output != null)
            {
                overall_target_unique_unique_phospho_output.Close();
            }
        }

        private static double CalculateMedian(List<double> values)
        {
            values.Sort();
            int middle_index = values.Count / 2;
            if(values.Count % 2 == 0)
            {
                return (values[middle_index - 1] + values[middle_index]) / 2.0;
            }
            else
            {
                return values[middle_index];
            }
        }

        //private static int AscendingPeptideHitQValueThenEValueScoreComparison(PeptideHit left, PeptideHit right)
        //{
        //    int comparison = left.QValue.CompareTo(right.QValue);
        //    if(comparison != 0)
        //    {
        //        return comparison;
        //    }
        //    else
        //    {
        //        return left.EValueScore.CompareTo(right.EValueScore);
        //    }
        //}
    }
}