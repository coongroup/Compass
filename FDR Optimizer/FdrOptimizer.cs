using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using XRAWFILE2Lib;
using LumenWorks.Framework.IO.Csv;
using MSFileReaderLib;

namespace FdrOptimizer
{
    public class FdrOptimizer
    {
        private const double MAXIMUM_FDR_FOR_SYSTEMATIC_PRECURSOR_MASS_ERROR = 1.0;

        public event EventHandler Starting;

        protected virtual void onStarting(EventArgs e)
        {
            EventHandler handler = Starting;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> StartingFile;

        protected virtual void onStartingFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = StartingFile;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void onUpdateProgress(ProgressEventArgs e)
        {
            if (UpdateProgress != null)
            {
                UpdateProgress(this, e);
            }
        }

        public event EventHandler<ExceptionEventArgs> ThrowException;

        protected virtual void onThrowException(ExceptionEventArgs e)
        {
            EventHandler<ExceptionEventArgs> handler = ThrowException;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> FinishedFile;

        protected virtual void onFinishedFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = FinishedFile;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Finished;

        protected virtual void onFinished(EventArgs e)
        {
            EventHandler handler = Finished;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        private IList<string> csvFilepaths;
        private string rawFolder;
        private IEnumerable<Modification> fixedModifications;
        private double maximumPrecursorMassError;
        private double precursorMassErrorIncrement;
        private bool higherScoresAreBetter;
        private double maximumFalseDiscoveryRate;
        private bool unique;
        private bool overallOutputs;
        private bool phosphopeptideOutputs;
        private string outputFolder;

        public FdrOptimizer(IList<string> csvFilepaths, string rawFolder, 
            IEnumerable<Modification> fixedModifications, 
            double maximumPrecursorMassError, double precursorMassErrorIncrement, 
            bool higherScoresAreBetter, 
            double maximumFalseDiscoveryRate, 
            bool unique, 
            bool overallOutputs, bool phosphopeptideOutputs, string outputFolder)
        {
            this.csvFilepaths = csvFilepaths;
            this.rawFolder = rawFolder;
            this.fixedModifications = fixedModifications;
            this.maximumPrecursorMassError = maximumPrecursorMassError;
            this.precursorMassErrorIncrement = precursorMassErrorIncrement;
            this.higherScoresAreBetter = higherScoresAreBetter;
            this.maximumFalseDiscoveryRate = maximumFalseDiscoveryRate;
            this.unique = unique;
            this.overallOutputs = overallOutputs;
            this.phosphopeptideOutputs = phosphopeptideOutputs;
            this.outputFolder = outputFolder;
        }

        public void Optimize()
        {
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
            StreamWriter summary = null;
            MSFileReaderLib.IXRawfile5 raw = null;
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

            //try
            //{
                onStarting(new EventArgs());

                StringBuilder fixed_modifications_sb = new StringBuilder();
                foreach(Modification modification in fixedModifications)
                {
                    fixed_modifications_sb.Append(modification.Name);
                    fixed_modifications_sb.Append(", ");
                }
                if(fixed_modifications_sb.Length > 0)
                {
                    fixed_modifications_sb = fixed_modifications_sb.Remove(fixed_modifications_sb.Length - 2, 2);
                }
                string fixed_modifications = fixed_modifications_sb.ToString();

                Dictionary<string, List<string>> raw_csv_filepaths = new Dictionary<string, List<string>>();
                foreach(string csv_filepath in csvFilepaths)
                {
                    string truncated_filename = Path.GetFileNameWithoutExtension(csv_filepath);
                    string[] raw_filepaths = null;
                    do
                    {
                        if(rawFolder != null && rawFolder != string.Empty && Directory.Exists(rawFolder))
                        {
                            raw_filepaths = Directory.GetFiles(rawFolder, truncated_filename + ".raw", SearchOption.AllDirectories);
                        }
                        else
                        {
                            raw_filepaths = Directory.GetFiles(Directory.GetParent(csv_filepath).ToString(), truncated_filename + ".raw", SearchOption.AllDirectories);
                        }
                        truncated_filename = truncated_filename.Substring(0, truncated_filename.Length - 1);
                        if(truncated_filename.Length == 0)
                        {
                            throw new Exception("No corresponding .raw file found for " + csv_filepath);
                        }
                    } while(raw_filepaths.Length == 0);

                    if(!raw_csv_filepaths.ContainsKey(raw_filepaths[0]))
                    {
                        raw_csv_filepaths.Add(raw_filepaths[0], new List<string>());
                    }
                    raw_csv_filepaths[raw_filepaths[0]].Add(csv_filepath);
                }

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

                if(!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                string overall_output_folder = outputFolder;

                string overall_phospho_output_folder = Path.Combine(outputFolder, "phospho");
                if(overallOutputs && phosphopeptideOutputs)
                {
                    if(!Directory.Exists(overall_phospho_output_folder))
                    {
                        Directory.CreateDirectory(overall_phospho_output_folder);
                    }
                }

                string log_folder = Path.Combine(outputFolder, "log");
                if(!Directory.Exists(log_folder))
                {
                    Directory.CreateDirectory(log_folder);
                }

                string scans_folder = Path.Combine(outputFolder, "scans");
                if(!Directory.Exists(scans_folder))
                {
                    Directory.CreateDirectory(scans_folder);
                }

                string scans_phospho_folder = Path.Combine(scans_folder, "phospho");
                if(phosphopeptideOutputs)
                {
                    if(!Directory.Exists(scans_phospho_folder))
                    {
                        Directory.CreateDirectory(scans_phospho_folder);
                    }
                }

                string target_decoy_folder = Path.Combine(outputFolder, "target-decoy");
                if(!Directory.Exists(target_decoy_folder))
                {
                    Directory.CreateDirectory(target_decoy_folder);
                }

                string target_decoy_phospho_folder = Path.Combine(target_decoy_folder, "phospho");
                if(phosphopeptideOutputs)
                {
                    if(!Directory.Exists(target_decoy_phospho_folder))
                    {
                        Directory.CreateDirectory(target_decoy_phospho_folder);
                    }
                }

                string unique_folder = Path.Combine(outputFolder, "unique");
                if(!Directory.Exists(unique_folder))
                {
                    Directory.CreateDirectory(unique_folder);
                }

                string unique_phospho_folder = Path.Combine(unique_folder, "phospho");
                if(phosphopeptideOutputs)
                {
                    if(!Directory.Exists(unique_phospho_folder))
                    {
                        Directory.CreateDirectory(unique_phospho_folder);
                    }
                }

                string overall_scans_filepath = Path.Combine(overall_output_folder, "scans.csv");
                string overall_scans_phospho_filepath = Path.Combine(overall_phospho_output_folder, "scans_phospho.csv");

                string overall_target_filepath = Path.Combine(overall_output_folder, "target.csv");
                string overall_decoy_filepath = Path.Combine(overall_output_folder, "decoy.csv");
                string overall_target_phospho_filepath = Path.Combine(overall_phospho_output_folder, "target_phospho.csv");
                string overall_decoy_phospho_filepath = Path.Combine(overall_phospho_output_folder, "decoy_phospho.csv");

                string overall_target_unique_filepath = Path.Combine(overall_output_folder, "target_unique.csv");
                string overall_decoy_unique_filepath = Path.Combine(overall_output_folder, "decoy_unique.csv");
                string overall_target_unique_phospho_filepath = Path.Combine(overall_phospho_output_folder, "target_unique_phospho.csv");
                string overall_decoy_unique_phospho_filepath = Path.Combine(overall_phospho_output_folder, "decoy_unique_phospho.csv");

                if(overallOutputs)
                {
                    overall_log = new StreamWriter(Path.Combine(overall_output_folder, "FDR_Optimizer_log.txt"));
                    overall_log.AutoFlush = true;

                    overall_log.WriteLine("FDR Optimizer PARAMETERS");
                    overall_log.WriteLine("Fixed Modifications: " + fixed_modifications);
                    overall_log.WriteLine("Maximum Precursor Mass Error (ppm): ±" + maximumPrecursorMassError.ToString());
                    overall_log.WriteLine("Precursor Mass Error Increment (ppm): " + precursorMassErrorIncrement.ToString());
                    overall_log.WriteLine("Higher Scores are Better: " + higherScoresAreBetter.ToString());
                    overall_log.WriteLine("Maximum False Discovery Rate (%): " + maximumFalseDiscoveryRate.ToString());
                    overall_log.WriteLine("FDR Calculation and Optimization Based on Unique Peptide Sequences: " + unique.ToString());
                    overall_log.WriteLine();

                    overall_scans_output = new StreamWriter(overall_scans_filepath);
                    overall_target_output = new StreamWriter(overall_target_filepath);
                    overall_decoy_output = new StreamWriter(overall_decoy_filepath);
                    overall_target_unique_output = new StreamWriter(overall_target_unique_filepath);
                    overall_decoy_unique_output = new StreamWriter(overall_decoy_unique_filepath);

                    if(phosphopeptideOutputs)
                    {
                        overall_scans_phospho_output = new StreamWriter(overall_scans_phospho_filepath);
                        overall_target_phospho_output = new StreamWriter(overall_target_phospho_filepath);
                        overall_decoy_phospho_output = new StreamWriter(overall_decoy_phospho_filepath);
                        overall_target_unique_phospho_output = new StreamWriter(overall_target_unique_phospho_filepath);
                        overall_decoy_unique_phospho_output = new StreamWriter(overall_decoy_unique_phospho_filepath);
                    }

                    summary = new StreamWriter(Path.Combine(overall_output_folder, "summary.csv"));
                    summary.AutoFlush = true;
                    summary.WriteLine("CSV Filepath,Raw Filepath,Preliminary E-Value Score Threshold,Preliminary Target Peptides,Preliminary Decoy Peptides,Preliminary FDR (%),Systematic (Median) Precursor Mass Error (ppm),Scans," + (phosphopeptideOutputs ? "Phosphopeptide Scans," : null) + "Q-Value Threshold (%),E-Value Score Threshold,Maximum Precursor Mass Error (ppm),Target Peptides,Decoy Peptides," + (phosphopeptideOutputs ? "Target Phosphopeptides,Decoy Phosphopeptides," : null) + (unique ? null : "FDR (%),") + "Unique Target Peptides,Unique Decoy Peptides," + (phosphopeptideOutputs ? "Unique Target Phosphopeptides,Unique Decoy Phosphopeptides," : null) + (unique ? "FDR (%)" : null));
                }

                string extended_header_line = null;
                bool overall_header_written = false;

                SortedDictionary<string, PeptideHit> overall_target_peptides = new SortedDictionary<string, PeptideHit>();
                SortedDictionary<string, PeptideHit> overall_decoy_peptides = new SortedDictionary<string, PeptideHit>();

                foreach(KeyValuePair<string, List<string>> raw_csv_filepath in raw_csv_filepaths)
                {
                    string raw_filepath = raw_csv_filepath.Key;

                    raw = (MSFileReaderLib.IXRawfile5)new MSFileReader_XRawfile();
                    raw.Open(raw_filepath);
                    raw.SetCurrentController(0, 1);

                    int first_scan_number = 1;
                    raw.GetFirstSpectrumNumber(ref first_scan_number);

                    foreach(string csv_filepath in raw_csv_filepath.Value)
                    {
                        onStartingFile(new FilepathEventArgs(csv_filepath));

                        onUpdateProgress(new ProgressEventArgs(0));

                        FileInfo file_info = new FileInfo(csv_filepath);

                        string log_filepath = Path.Combine(log_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_log.txt");
                        log = new StreamWriter(log_filepath);
                        log.AutoFlush = true;

                        log.WriteLine("FDR Optimizer PARAMETERS");
                        log.WriteLine("Fixed Modifications: " + fixed_modifications);
                        log.WriteLine("Maximum Precursor Mass Error (ppm): ±" + maximumPrecursorMassError.ToString());
                        log.WriteLine("Precursor Mass Error Increment (ppm): " + precursorMassErrorIncrement.ToString());
                        log.WriteLine("Higher Scores are Better: " + higherScoresAreBetter.ToString());
                        log.WriteLine("Maximum False Discovery Rate (%): " + maximumFalseDiscoveryRate.ToString());
                        log.WriteLine("FDR Calculation and Optimization Based on Unique Peptide Sequences: " + unique.ToString());
                        log.WriteLine();

                        if(overallOutputs)
                        {
                            overall_log.WriteLine(raw_filepath);
                            overall_log.WriteLine();
                        }
                        log.WriteLine(raw_filepath);
                        log.WriteLine();

                        SortedDictionary<int, PeptideHit> scans_peptides = new SortedDictionary<int, PeptideHit>();

                        if(overallOutputs)
                        {
                            overall_log.WriteLine(csv_filepath);

                            summary.Write(csv_filepath + ',');
                            summary.Write(raw_filepath + ',');
                        }
                        log.WriteLine(csv_filepath);

                        // DJB addition
                        StreamReader csv_file = new StreamReader(csv_filepath);
                        using (CsvReader reader = new CsvReader(csv_file, true))
                        {
                            string header_line = string.Join(",", reader.GetFieldHeaders());                        
                            extended_header_line = header_line + ", Precursor Isolation m/z, Precursor Isolation Mass (Da), Precursor Theoretical Neutral Mass (Da), Precursor Experimental Neutral Mass (Da), Precursor Mass Error (ppm), Adjusted Precursor Mass Error (ppm), Q-Value (%)";
                            if (overallOutputs && !overall_header_written)
                            {
                                overall_scans_output.WriteLine(extended_header_line);
                                overall_target_output.WriteLine(extended_header_line);
                                overall_decoy_output.WriteLine(extended_header_line);
                                overall_target_unique_output.WriteLine(extended_header_line);
                                overall_decoy_unique_output.WriteLine(extended_header_line);
                                if (phosphopeptideOutputs)
                                {
                                    overall_scans_phospho_output.WriteLine(extended_header_line);
                                    overall_target_phospho_output.WriteLine(extended_header_line);
                                    overall_decoy_phospho_output.WriteLine(extended_header_line);
                                    overall_target_unique_phospho_output.WriteLine(extended_header_line);
                                    overall_decoy_unique_phospho_output.WriteLine(extended_header_line);
                                }

                                overall_header_written = true;
                            }
                            int counter = 0;
                            string[] data = new string[reader.FieldCount];
                            PeptideHit storedHit = null;
                            int msn = 0;
                            int lastMS1 = -1;
                            int lastscan = -1;
                            double header_mass = 0;
                            double[,] labels = null;
                            List<double> ms1Peaks = null;
                            while (reader.ReadNextRecord())
                            {                          
                                int scan_number = int.Parse(reader["Spectrum number"]);

                                // Check to see if there was an MS1 between this and the last record read
                                if (scan_number > lastscan + 1)
                                {
                                    // Preprocess the MS1 scan
                                    int ms1_scan = scan_number - 1;
                                    while (ms1_scan > lastMS1)
                                    {
                                        raw.GetMSOrderForScanNum(ms1_scan, ref msn);
                                        if (msn == 1)
                                        {
                                            break;
                                        }
                                        ms1_scan--;
                                    }
                                    if (lastMS1 != ms1_scan)
                                    {
                                        lastMS1 = ms1_scan;                                        
                                        object labels_obj = null;
                                        object flags_obj = null;
                                        raw.GetLabelData(ref labels_obj, ref flags_obj, ref lastMS1);
                                        labels = (double[,])labels_obj;
                                        ms1Peaks = new List<double>(labels.GetUpperBound(1) + 1);
                                        int max = labels.GetUpperBound(1);
                                        for (int i = labels.GetLowerBound(1); i <= max; i++)
                                        {
                                            ms1Peaks.Add(labels[(int)RawLabelDataColumn.MZ, i]);
                                        }
                                    }
                                }
                                // Save this scan number
                                lastscan = scan_number; 
                             
                                // Read in the e-value
                                double evalue_score = double.Parse(reader["E-value"]);

                                // Read in the defline and see if it is a decoy peptide
                                string defline = reader["Defline"];
                                bool decoy = defline.StartsWith("DECOY") || defline.StartsWith("REVERSED"); 

                                // Have we already processed a peptide for this scan number?
                                bool containsSNAlready = false;
                                if (containsSNAlready = scans_peptides.TryGetValue(scan_number, out storedHit)) {    
                                    // We have proceessed a peptide for this scan number before, retrieve that result
                                    double storedEvalue = storedHit.EValueScore;
                                    bool storedDecoy =  storedHit.Decoy;

                                    // If the current evalue equals the stored value, and the stored value is a decoy, keep the decoy
                                    if(evalue_score.Equals(storedEvalue) && storedDecoy) {                                      
                                        continue; // If scores are equivalent, keep the decoy on the stack (more conservative)
                                    }

                                    if (higherScoresAreBetter)
                                    {
                                        // Higher scores are better, so if the current score is lower the the stored score, skip it
                                        if (evalue_score < storedEvalue) continue;
                                    }
                                    else
                                    {
                                        // Lower scores are better, so if the current score is greater the the stored score, skip it
                                        if (evalue_score > storedEvalue) continue;
                                    } 
                                }

                                string sequence = reader["Peptide"];                                                   
                                string dynamic_modifications = reader["Mods"];
                                int charge = int.Parse(reader["Charge"]);
                                Peptide peptide = new Peptide(sequence, fixedModifications, dynamic_modifications);

                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {                                    
                                    sb.Append(reader[i].Replace(',',';'));
                                    sb.Append(',');
                                }
                                sb.Remove(sb.Length - 1, 1);                                
                                string line = sb.ToString();
                                raw.GetPrecursorMassForScanNum(scan_number, 2, ref header_mass);
                                PeptideHit hit = new PeptideHit(line, sequence, dynamic_modifications, evalue_score, decoy, peptide, ms1Peaks, header_mass, charge);
                                                               
                                if (containsSNAlready)
                                {
                                    // We already processed this scan number before, so replace this hit
                                    scans_peptides[scan_number] = hit;
                                } 
                                else 
                                {
                                    // We haven't process this scan number before, so add the hit
                                    scans_peptides.Add(scan_number, hit);
                                }                                   
 
                                if (dynamic_modifications.Contains("phosphorylation"))
                                {
                                    overall_scans_phospho++;
                                }
                           
                                counter++;
                                if (counter > 100)
                                {
                                    double progress = (double)csv_file.BaseStream.Position / csv_file.BaseStream.Length;
                                    onUpdateProgress(new ProgressEventArgs((int)(progress * 100.0)));                                   
                                    counter = 0;
                                }
                            }
                        }                     
                           
                        onUpdateProgress(new ProgressEventArgs(0));
                        

                        overall_scans += scans_peptides.Count;

                        if(overallOutputs)
                        {
                            overall_log.WriteLine();
                        }
                        log.WriteLine();

                        List<PeptideHit> peptides = new List<PeptideHit>(scans_peptides.Values);
                        peptides.Sort(new AscendingPeptideHitEValueScoreComparer(higherScoresAreBetter));
                        int cumulative_target_peptides = 0;
                        int cumulative_decoy_peptides = 0;
                        double best_low_res_evalue_score_threshold = higherScoresAreBetter ? double.PositiveInfinity : double.NegativeInfinity;
                        double best_low_res_false_discovery_rate = double.NaN;
                        int best_target_peptides = 0;
                        int best_decoy_peptides = 0;
                        int p = 0;
                        while(p < peptides.Count)
                        {
                            PeptideHit peptide = peptides[p];
                            if(!peptide.Decoy)
                            {
                                cumulative_target_peptides++;
                            }
                            else
                            {
                                cumulative_decoy_peptides++;
                            }

                            p++;

                            if(p < peptides.Count)
                            {
                                PeptideHit next_peptide = peptides[p];
                                if(next_peptide.EValueScore == peptide.EValueScore)
                                {
                                    continue;
                                }
                            }

                            double false_discovery_rate = (double)cumulative_decoy_peptides / cumulative_target_peptides * 100.0;
                            if(false_discovery_rate <= MAXIMUM_FDR_FOR_SYSTEMATIC_PRECURSOR_MASS_ERROR)
                            {
                                if(cumulative_target_peptides > best_target_peptides
                                    || (cumulative_target_peptides == best_target_peptides && false_discovery_rate < best_low_res_false_discovery_rate))
                                {
                                    best_low_res_evalue_score_threshold = peptide.EValueScore;
                                    best_low_res_false_discovery_rate = false_discovery_rate;
                                    best_target_peptides = cumulative_target_peptides;
                                    best_decoy_peptides = cumulative_decoy_peptides;
                                }
                            }
                        }

                        List<double> precursor_mass_errors = new List<double>(cumulative_target_peptides);
                        foreach(PeptideHit peptide in peptides)
                        {
                            if((!higherScoresAreBetter && peptide.EValueScore > best_low_res_evalue_score_threshold)
                                || (higherScoresAreBetter && peptide.EValueScore < best_low_res_evalue_score_threshold))
                            {
                                break;
                            }

                            if(!peptide.Decoy)
                            {
                                precursor_mass_errors.Add(peptide.PrecursorMassError);
                            }
                        }

                        double median_precursor_mass_error = 0.0;
                        if(precursor_mass_errors.Count > 0)
                        {
                            median_precursor_mass_error = CalculateMedian(precursor_mass_errors);
                        }
                        foreach(PeptideHit peptide in scans_peptides.Values)
                        {
                            peptide.AdjustedPrecursorMassError = peptide.PrecursorMassError - median_precursor_mass_error;
                        }

                        int q_cumulative_target_peptides = 0;
                        int q_cumulative_decoy_peptides = 0;
                        Dictionary<string, int> q_target_peptides = new Dictionary<string, int>();
                        Dictionary<string, int> q_decoy_peptides = new Dictionary<string, int>();
                        int p3 = 0;
                        while(p3 < peptides.Count)
                        {
                            PeptideHit peptide = peptides[p3];
                            if(!peptide.Decoy)
                            {
                                q_cumulative_target_peptides++;
                                if(!q_target_peptides.ContainsKey(peptide.Sequence))
                                {
                                    q_target_peptides.Add(peptide.Sequence, 0);
                                }
                                q_target_peptides[peptide.Sequence]++;
                            }
                            else
                            {
                                q_cumulative_decoy_peptides++;
                                if(!q_decoy_peptides.ContainsKey(peptide.Sequence))
                                {
                                    q_decoy_peptides.Add(peptide.Sequence, 0);
                                }
                                q_decoy_peptides[peptide.Sequence]++;
                            }

                            p3++;

                            if(p3 < peptides.Count)
                            {
                                PeptideHit next_peptide = peptides[p3];
                                if(next_peptide.EValueScore == peptide.EValueScore)
                                {
                                    continue;
                                }
                            }

                            int current_target_peptides = unique ? q_target_peptides.Count : q_cumulative_target_peptides;
                            int current_decoy_peptides = unique ? q_decoy_peptides.Count : q_cumulative_decoy_peptides;
                            double q_value = (double)current_decoy_peptides / current_target_peptides * 100.0;
                            peptide.QValue = q_value;

                            int p4 = p3 - 1 - 1;
                            while(p4 >= 0)
                            {
                                if(double.IsNaN(peptides[p4].QValue))
                                {
                                    peptides[p4].QValue = q_value;
                                    p4--;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        int p5 = peptides.Count - 1 - 1;
                        while(p5 >= 0)
                        {
                            if(peptides[p5].QValue > peptides[p5 + 1].QValue)
                            {
                                peptides[p5].QValue = peptides[p5 + 1].QValue;
                            }
                            p5--;
                        }

                        if(overallOutputs)
                        {
                            summary.Write(best_low_res_evalue_score_threshold.ToString() + ',');
                            summary.Write(best_target_peptides.ToString() + ',');
                            summary.Write(best_decoy_peptides.ToString() + ',');
                            summary.Write(best_low_res_false_discovery_rate.ToString() + ',');
                            summary.Write(median_precursor_mass_error.ToString() + ',');
                        }

                        log.WriteLine("Preliminary E-Value Score Threshold: " + best_low_res_evalue_score_threshold.ToString());
                        log.WriteLine("Preliminary Target Peptides: " + best_target_peptides.ToString());
                        log.WriteLine("Preliminary Decoy Peptides: " + best_decoy_peptides.ToString());
                        log.WriteLine("Preliminary FDR (%): " + best_low_res_false_discovery_rate.ToString());
                        log.WriteLine("Systematic (Median) Precursor Mass Error (ppm): " + median_precursor_mass_error.ToString());
                        log.WriteLine();

                        string scans_filepath = Path.Combine(scans_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_scans.csv");
                        scans_output = new StreamWriter(scans_filepath);
                        scans_output.WriteLine(extended_header_line);

                        string scans_phospho_filepath = Path.Combine(scans_phospho_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_scans_phospho.csv");
                        if(phosphopeptideOutputs)
                        {
                            scans_phospho_output = new StreamWriter(scans_phospho_filepath);
                            scans_phospho_output.WriteLine(extended_header_line);
                        }

                        int scans_phospho = 0;
                        foreach(PeptideHit peptide in scans_peptides.Values)
                        {
                            if(overallOutputs)
                            {
                                overall_scans_output.WriteLine(peptide.ExtendedLine);
                            }
                            scans_output.WriteLine(peptide.ExtendedLine);
                            if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                            {
                                scans_phospho++;
                                if(overallOutputs)
                                {
                                    overall_scans_phospho_output.WriteLine(peptide.ExtendedLine);
                                }
                                scans_phospho_output.WriteLine(peptide.ExtendedLine);
                            }
                        }

                        scans_output.Close();
                        if(phosphopeptideOutputs)
                        {
                            scans_phospho_output.Close();
                        }

                        if(overallOutputs)
                        {
                            summary.Write(scans_peptides.Count.ToString() + ',');
                        }
                        log.WriteLine(scans_filepath);
                        log.WriteLine(scans_peptides.Count.ToString() + " MS/MS scans resulted in at least one peptide hit");
                        if(phosphopeptideOutputs)
                        {
                            log.WriteLine(scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
                            if(overallOutputs)
                            {
                                summary.Write(scans_phospho.ToString() + ',');
                            }
                        }
                        log.WriteLine();

                        peptides.Sort(new AscendingPeptideHitQValueComparer(higherScoresAreBetter));
                        int best_target = 0;
                        int best_decoy = 0;
                        double best_false_discovery_rate = double.NaN;
                        double best_q_value = double.NegativeInfinity;
                        double best_evalue_score = higherScoresAreBetter ? double.PositiveInfinity : double.NegativeInfinity;
                        double best_max_precursor_mass_error = 0.0;

                        double max_precursor_mass_error = precursorMassErrorIncrement;
                        while(max_precursor_mass_error <= maximumPrecursorMassError)
                        {
                            List<PeptideHit> filtered_peptides = new List<PeptideHit>();
                            foreach(PeptideHit peptide in peptides)
                            {
                                if(Math.Abs(peptide.AdjustedPrecursorMassError) <= max_precursor_mass_error)
                                {
                                    filtered_peptides.Add(peptide);
                                }
                            }
                            int target_count = 0;
                            int decoy_count = 0;
                            Dictionary<string, int> target_unique_count = new Dictionary<string, int>();
                            Dictionary<string, int> decoy_unique_count = new Dictionary<string, int>();
                            int p2 = 0;
                            int value = 0;
                            while(p2 < filtered_peptides.Count)
                            {
                                PeptideHit peptide = filtered_peptides[p2];
                                string sequence = peptide.Sequence;
                                if(!peptide.Decoy)
                                {
                                    target_count++;
                                    if (target_unique_count.TryGetValue(sequence, out value))
                                    {
                                        target_unique_count[sequence] = value + 1;
                                    }
                                    else
                                    {
                                        target_unique_count.Add(sequence, 1);
                                    }
                                }
                                else
                                {                                    
                                    decoy_count++;
                                    if (decoy_unique_count.TryGetValue(sequence, out value))
                                    {
                                        decoy_unique_count[sequence] = value + 1;
                                    }
                                    else
                                    {
                                        decoy_unique_count.Add(sequence, 1);
                                    }
                                }

                                p2++;

                                if(p2 < filtered_peptides.Count)
                                {
                                    PeptideHit next_peptide = filtered_peptides[p2];
                                    if(next_peptide.QValue == peptide.QValue && next_peptide.EValueScore == peptide.EValueScore)
                                    {
                                        continue;
                                    }
                                }

                                int target = unique ? target_unique_count.Count : target_count;
                                int decoy = unique ? decoy_unique_count.Count : decoy_count;
                                double false_discovery_rate = (double)decoy / target * 100.0;
                                if(false_discovery_rate <= maximumFalseDiscoveryRate)
                                {
                                    if(target > best_target || (target == best_target && false_discovery_rate < best_false_discovery_rate))
                                    {
                                        best_target = target;
                                        best_decoy = decoy;
                                        best_false_discovery_rate = false_discovery_rate;
                                        best_q_value = peptide.QValue;
                                        best_evalue_score = peptide.EValueScore;
                                        best_max_precursor_mass_error = max_precursor_mass_error;
                                    }
                                }
                            }

                            max_precursor_mass_error += precursorMassErrorIncrement;
                        }

                        string target_filepath = Path.Combine(target_decoy_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target.csv");
                        target_output = new StreamWriter(target_filepath);
                        target_output.WriteLine(extended_header_line);
                        string decoy_filepath = Path.Combine(target_decoy_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy.csv");
                        decoy_output = new StreamWriter(decoy_filepath);
                        decoy_output.WriteLine(extended_header_line);

                        string target_phospho_filepath = Path.Combine(target_decoy_phospho_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_phospho.csv");
                        string decoy_phospho_filepath = Path.Combine(target_decoy_phospho_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_phospho.csv");
                        if(phosphopeptideOutputs)
                        {
                            target_phospho_output = new StreamWriter(target_phospho_filepath);
                            target_phospho_output.WriteLine(extended_header_line);
                            decoy_phospho_output = new StreamWriter(decoy_phospho_filepath);
                            decoy_phospho_output.WriteLine(extended_header_line);
                        }

                        SortedDictionary<string, PeptideHit> target_unique = new SortedDictionary<string, PeptideHit>();
                        SortedDictionary<string, PeptideHit> decoy_unique = new SortedDictionary<string, PeptideHit>();
                        int redundant_target = 0;
                        int redundant_decoy = 0;
                        int redundant_target_phospho = 0;
                        int redundant_decoy_phospho = 0;
                        foreach(PeptideHit peptide in scans_peptides.Values)
                        {
                            if((peptide.QValue < best_q_value || (peptide.QValue == best_q_value 
                                && ((!higherScoresAreBetter && peptide.EValueScore <= best_evalue_score) 
                                || (higherScoresAreBetter && peptide.EValueScore >= best_evalue_score)))) 
                                && Math.Abs(peptide.AdjustedPrecursorMassError) <= best_max_precursor_mass_error)
                            {
                                if(!peptide.Decoy)
                                {
                                    redundant_target++;
                                    if(overallOutputs)
                                    {
                                        overall_target_output.WriteLine(peptide.ExtendedLine);
                                    }
                                    target_output.WriteLine(peptide.ExtendedLine);
                                    if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                                    {
                                        redundant_target_phospho++;
                                        if(overallOutputs)
                                        {
                                            overall_target_phospho_output.WriteLine(peptide.ExtendedLine);
                                        }
                                        target_phospho_output.WriteLine(peptide.ExtendedLine);
                                    }

                                    if(!target_unique.ContainsKey(peptide.Sequence))
                                    {
                                        target_unique.Add(peptide.Sequence, peptide);
                                    }
                                    else if(peptide.QValue < target_unique[peptide.Sequence].QValue 
                                        || (peptide.QValue == target_unique[peptide.Sequence].QValue
                                        && ((!higherScoresAreBetter && peptide.EValueScore < target_unique[peptide.Sequence].EValueScore)
                                        || (higherScoresAreBetter && peptide.EValueScore > target_unique[peptide.Sequence].EValueScore)))
                                        || (peptide.QValue == target_unique[peptide.Sequence].QValue 
                                        && peptide.EValueScore == target_unique[peptide.Sequence].EValueScore 
                                        && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(target_unique[peptide.Sequence].AdjustedPrecursorMassError)))
                                    {
                                        target_unique[peptide.Sequence] = peptide;
                                    }
                                }
                                else
                                {
                                    redundant_decoy++;
                                    if(overallOutputs)
                                    {
                                        overall_decoy_output.WriteLine(peptide.ExtendedLine);
                                    }
                                    decoy_output.WriteLine(peptide.ExtendedLine);
                                    if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                                    {
                                        redundant_decoy_phospho++;
                                        if(overallOutputs)
                                        {
                                            overall_decoy_phospho_output.WriteLine(peptide.ExtendedLine);
                                        }
                                        decoy_phospho_output.WriteLine(peptide.ExtendedLine);
                                    }

                                    if(!decoy_unique.ContainsKey(peptide.Sequence))
                                    {
                                        decoy_unique.Add(peptide.Sequence, peptide);
                                    }
                                    else if(peptide.QValue < decoy_unique[peptide.Sequence].QValue
                                        || (peptide.QValue == decoy_unique[peptide.Sequence].QValue
                                        && ((!higherScoresAreBetter && peptide.EValueScore < decoy_unique[peptide.Sequence].EValueScore) 
                                        || (higherScoresAreBetter && peptide.EValueScore > decoy_unique[peptide.Sequence].EValueScore)))
                                        || (peptide.QValue == decoy_unique[peptide.Sequence].QValue
                                        && peptide.EValueScore == decoy_unique[peptide.Sequence].EValueScore
                                        && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(decoy_unique[peptide.Sequence].AdjustedPrecursorMassError)))
                                    {
                                        decoy_unique[peptide.Sequence] = peptide;
                                    }
                                }
                            }
                        }

                        overall_target += redundant_target;
                        overall_decoy += redundant_decoy;
                        overall_target_phospho += redundant_target_phospho;
                        overall_decoy_phospho += redundant_decoy_phospho;

                        target_output.Close();
                        decoy_output.Close();
                        if(phosphopeptideOutputs)
                        {
                            target_phospho_output.Close();
                            decoy_phospho_output.Close();
                        }

                        if(overallOutputs)
                        {
                            summary.Write(best_q_value.ToString() + ',');
                            summary.Write(best_evalue_score.ToString() + ',');
                            summary.Write(best_max_precursor_mass_error.ToString() + ',');
                        }

                        log.WriteLine("Q-Value Threshold (%): " + best_q_value.ToString());
                        log.WriteLine("E-Value Score Threshold: " + best_evalue_score.ToString());
                        log.WriteLine("Maximum Precursor Mass Error (ppm): ±" + best_max_precursor_mass_error.ToString());
                        log.WriteLine();

                        if(overallOutputs)
                        {
                            summary.Write(redundant_target.ToString() + ',');
                            summary.Write(redundant_decoy.ToString() + ',');
                        }

                        log.WriteLine(target_filepath);
                        log.WriteLine(redundant_target.ToString() + " target peptides after FDR optimization");
                        log.WriteLine(decoy_filepath);
                        log.WriteLine(redundant_decoy.ToString() + " decoy peptides after FDR optimization");
                        
                        if(phosphopeptideOutputs)
                        {
                            if(overallOutputs)
                            {
                                summary.Write(redundant_target_phospho.ToString() + ',');
                                summary.Write(redundant_decoy_phospho.ToString() + ',');
                            }

                            log.WriteLine(target_phospho_filepath);
                            log.WriteLine(redundant_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
                            log.WriteLine(decoy_phospho_filepath);
                            log.WriteLine(redundant_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
                        }

                        log.WriteLine();

                        if(!unique)
                        {
                            if(overallOutputs)
                            {
                                summary.Write(((double)redundant_decoy / redundant_target * 100.0).ToString() + ',');
                            }

                            log.WriteLine("FDR (%): " + ((double)redundant_decoy / redundant_target * 100.0).ToString());
                            log.WriteLine();
                        }

                        string target_unique_filepath = Path.Combine(unique_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_unique.csv");
                        target_unique_output = new StreamWriter(target_unique_filepath);
                        target_unique_output.WriteLine(extended_header_line);
                        string decoy_unique_filepath = Path.Combine(unique_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_unique.csv");
                        decoy_unique_output = new StreamWriter(decoy_unique_filepath);
                        decoy_unique_output.WriteLine(extended_header_line);

                        string target_unique_phospho_filepath = Path.Combine(unique_phospho_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_target_unique_phospho.csv");
                        string decoy_unique_phospho_filepath = Path.Combine(unique_phospho_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_decoy_unique_phospho.csv");
                        if(phosphopeptideOutputs)
                        {
                            target_unique_phospho_output = new StreamWriter(target_unique_phospho_filepath);
                            target_unique_phospho_output.WriteLine(extended_header_line);
                            decoy_unique_phospho_output = new StreamWriter(decoy_unique_phospho_filepath);
                            decoy_unique_phospho_output.WriteLine(extended_header_line);
                        }

                        int target_unique_phospho = 0;
                        foreach(PeptideHit peptide in target_unique.Values)
                        {
                            if(overallOutputs)
                            {
                                overall_target_unique_output.WriteLine(peptide.ExtendedLine);
                            }
                            target_unique_output.WriteLine(peptide.ExtendedLine);
                            if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                            {
                                target_unique_phospho++;
                                if(overallOutputs)
                                {
                                    overall_target_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                                }
                                target_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                            }

                            if(!overall_target_peptides.ContainsKey(peptide.Sequence))
                            {
                                overall_target_peptides.Add(peptide.Sequence, peptide);
                            }
                            else if(peptide.QValue < overall_target_peptides[peptide.Sequence].QValue
                                || (peptide.QValue == overall_target_peptides[peptide.Sequence].QValue
                                && ((!higherScoresAreBetter && peptide.EValueScore < overall_target_peptides[peptide.Sequence].EValueScore)
                                || (higherScoresAreBetter && peptide.EValueScore > overall_target_peptides[peptide.Sequence].EValueScore)))
                                || (peptide.QValue == overall_target_peptides[peptide.Sequence].QValue
                                && peptide.EValueScore == overall_target_peptides[peptide.Sequence].EValueScore
                                && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(overall_target_peptides[peptide.Sequence].AdjustedPrecursorMassError)))
                            {
                                overall_target_peptides[peptide.Sequence] = peptide;
                            }
                        }
                        overall_target_unique += target_unique.Count;
                        overall_target_unique_phospho += target_unique_phospho;

                        target_unique_output.Close();
                        if(phosphopeptideOutputs)
                        {
                            target_unique_phospho_output.Close();
                        }

                        int decoy_unique_phospho = 0;
                        foreach(PeptideHit peptide in decoy_unique.Values)
                        {
                            if(overallOutputs)
                            {
                                overall_decoy_unique_output.WriteLine(peptide.ExtendedLine);
                            }
                            decoy_unique_output.WriteLine(peptide.ExtendedLine);
                            if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                            {
                                decoy_unique_phospho++;
                                if(overallOutputs)
                                {
                                    overall_decoy_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                                }
                                decoy_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                            }

                            if(!overall_decoy_peptides.ContainsKey(peptide.Sequence))
                            {
                                overall_decoy_peptides.Add(peptide.Sequence, peptide);
                            }
                            else if(peptide.QValue < overall_decoy_peptides[peptide.Sequence].QValue
                                || (peptide.QValue == overall_decoy_peptides[peptide.Sequence].QValue
                                && ((!higherScoresAreBetter && peptide.EValueScore < overall_decoy_peptides[peptide.Sequence].EValueScore)
                                || (higherScoresAreBetter && peptide.EValueScore > overall_decoy_peptides[peptide.Sequence].EValueScore)))
                                || (peptide.QValue == overall_decoy_peptides[peptide.Sequence].QValue
                                && peptide.EValueScore == overall_decoy_peptides[peptide.Sequence].EValueScore
                                && Math.Abs(peptide.AdjustedPrecursorMassError) < Math.Abs(overall_decoy_peptides[peptide.Sequence].AdjustedPrecursorMassError)))
                            {
                                overall_decoy_peptides[peptide.Sequence] = peptide;
                            }
                        }
                        overall_decoy_unique += decoy_unique.Count;
                        overall_decoy_unique_phospho += decoy_unique_phospho;

                        decoy_unique_output.Close();
                        if(phosphopeptideOutputs)
                        {
                            decoy_unique_phospho_output.Close();
                        }

                        if(overallOutputs)
                        {
                            summary.Write(target_unique.Count.ToString() + ',');
                            summary.Write(decoy_unique.Count.ToString() + ',');
                        }

                        log.WriteLine(target_unique_filepath);
                        log.WriteLine(target_unique.Count.ToString() + " unique target peptide sequences after FDR optimization");
                        log.WriteLine(decoy_unique_filepath);
                        log.WriteLine(decoy_unique.Count.ToString() + " unique decoy peptide sequences after FDR optimization");

                        if(phosphopeptideOutputs)
                        {
                            if(overallOutputs)
                            {
                                summary.Write(target_unique_phospho.ToString() + ',');
                                summary.Write(decoy_unique_phospho.ToString() + ',');
                            }

                            log.WriteLine(target_unique_phospho_filepath);
                            log.WriteLine(target_unique_phospho.ToString() + " unique target phosphopeptide sequences after FDR optimization");
                            log.WriteLine(decoy_unique_phospho_filepath);
                            log.WriteLine(decoy_unique_phospho.ToString() + " unique decoy phosphopeptide sequences after FDR optimization");
                        }

                        if(unique)
                        {
                            if(overallOutputs)
                            {
                                summary.Write(((double)decoy_unique.Count / target_unique.Count * 100.0).ToString() + ',');
                            }

                            log.WriteLine();
                            log.WriteLine("FDR (%): " + ((double)decoy_unique.Count / target_unique.Count * 100.0).ToString());
                        }

                        log.Close();

                        if(overallOutputs)
                        {
                            summary.WriteLine();
                        }

                        onFinishedFile(new FilepathEventArgs(csv_filepath));
                    }

                    raw.Close();
                }

                if(overallOutputs)
                {
                    summary.Write("SUM,");

                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");

                    string overall_target_unique_unique_filepath = Path.Combine(overall_output_folder, "target_unique_unique.csv");
                    overall_target_unique_unique_output = new StreamWriter(overall_target_unique_unique_filepath);
                    overall_target_unique_unique_output.WriteLine(extended_header_line);
                    string overall_decoy_unique_unique_filepath = Path.Combine(overall_output_folder, "decoy_unique_unique.csv");
                    overall_decoy_unique_unique_output = new StreamWriter(overall_decoy_unique_unique_filepath);
                    overall_decoy_unique_unique_output.WriteLine(extended_header_line);

                    string overall_target_unique_unique_phospho_filepath = Path.Combine(overall_phospho_output_folder, "target_unique_unique_phospho.csv");
                    string overall_decoy_unique_unique_phospho_filepath = Path.Combine(overall_phospho_output_folder, "decoy_unique_unique_phospho.csv");
                    if(phosphopeptideOutputs)
                    {
                        overall_target_unique_unique_phospho_output = new StreamWriter(overall_target_unique_unique_phospho_filepath);
                        overall_target_unique_unique_phospho_output.WriteLine(extended_header_line);
                        overall_decoy_unique_unique_phospho_output = new StreamWriter(overall_decoy_unique_unique_phospho_filepath);
                        overall_decoy_unique_unique_phospho_output.WriteLine(extended_header_line);
                    }

                    foreach(PeptideHit peptide in overall_target_peptides.Values)
                    {
                        overall_target_unique_unique_output.WriteLine(peptide.ExtendedLine);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                        {
                            overall_target_unique_unique_phospho++;
                            overall_target_unique_unique_phospho_output.WriteLine(peptide.ExtendedLine);
                        }
                    }

                    foreach(PeptideHit peptide in overall_decoy_peptides.Values)
                    {
                        overall_decoy_unique_unique_output.WriteLine(peptide.ExtendedLine);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
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
                    if(phosphopeptideOutputs)
                    {
                        overall_scans_phospho_output.Close();
                        overall_target_phospho_output.Close();
                        overall_decoy_phospho_output.Close();
                        overall_target_unique_phospho_output.Close();
                        overall_decoy_unique_phospho_output.Close();
                        overall_target_unique_unique_phospho_output.Close();
                        overall_decoy_unique_unique_phospho_output.Close();
                    }

                    overall_log.WriteLine(overall_scans_filepath);
                    overall_log.WriteLine(overall_scans.ToString() + " MS/MS scans resulted in at least one peptide hit");
                    summary.Write(overall_scans.ToString() + ',');

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
                        summary.Write(overall_scans_phospho.ToString() + ',');
                    }

                    overall_log.WriteLine();

                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");

                    overall_log.WriteLine(overall_target_filepath);
                    overall_log.WriteLine(overall_target.ToString() + " target peptides after FDR optimization");
                    summary.Write(overall_target.ToString() + ',');
                    overall_log.WriteLine(overall_decoy_filepath);
                    overall_log.WriteLine(overall_decoy.ToString() + " decoy peptides after FDR optimization");
                    summary.Write(overall_decoy.ToString() + ',');

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_phospho_filepath);
                        overall_log.WriteLine(overall_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
                        summary.Write(overall_target_phospho.ToString() + ',');
                        overall_log.WriteLine(overall_decoy_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
                        summary.Write(overall_decoy_phospho.ToString() + ',');
                    }

                    overall_log.WriteLine();

                    if(!unique)
                    {
                        overall_log.WriteLine("FDR (%): " + ((double)overall_decoy / overall_target * 100.0).ToString());
                        summary.Write(((double)overall_decoy / overall_target * 100.0).ToString() + ',');
                        overall_log.WriteLine();
                    }

                    overall_log.WriteLine(overall_target_unique_filepath);
                    overall_log.WriteLine(overall_target_unique.ToString() + " target unique peptide sequences after FDR optimization");
                    summary.Write(overall_target_unique.ToString() + ',');
                    overall_log.WriteLine(overall_decoy_unique_filepath);
                    overall_log.WriteLine(overall_decoy_unique.ToString() + " decoy unique peptide sequences after FDR optimization");
                    summary.Write(overall_decoy_unique.ToString() + ',');

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_unique_phospho_filepath);
                        overall_log.WriteLine(overall_target_unique_phospho.ToString() + " target unique phosphopeptide sequences after FDR optimization");
                        summary.Write(overall_target_unique_phospho.ToString() + ',');
                        overall_log.WriteLine(overall_decoy_unique_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_unique_phospho.ToString() + " decoy unique phosphopeptide sequences after FDR optimization");
                        summary.Write(overall_decoy_unique_phospho.ToString() + ',');
                    }

                    overall_log.WriteLine();

                    if(unique)
                    {
                        overall_log.WriteLine("FDR (%): " + ((double)overall_decoy_unique / overall_target_unique * 100.0).ToString());
                        summary.Write(((double)overall_decoy_unique / overall_target_unique * 100.0).ToString() + ',');
                        overall_log.WriteLine();
                    }

                    summary.WriteLine();

                    summary.Write("OVERALL,");

                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    summary.Write("n/a,");
                    if(!unique)
                    {
                        summary.Write("n/a,");
                    }

                    if(phosphopeptideOutputs)
                    {
                        summary.Write("n/a,");
                        summary.Write("n/a,");
                    }

                    overall_log.WriteLine(overall_target_unique_unique_filepath);
                    overall_log.WriteLine(overall_target_peptides.Count.ToString() + " target unique unique peptide sequences after FDR optimization");
                    summary.Write(overall_target_peptides.Count.ToString() + ',');
                    overall_log.WriteLine(overall_decoy_unique_unique_filepath);
                    overall_log.WriteLine(overall_decoy_peptides.Count.ToString() + " decoy unique unique peptide sequences after FDR optimization");
                    summary.Write(overall_decoy_peptides.Count.ToString() + ',');

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_unique_unique_phospho_filepath);
                        overall_log.WriteLine(overall_target_unique_unique_phospho.ToString() + " target unique unique phosphopeptide sequences after FDR optimization");
                        summary.Write(overall_target_unique_unique_phospho.ToString() + ',');
                        overall_log.WriteLine(overall_decoy_unique_unique_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_unique_unique_phospho.ToString() + " decoy unique unique phosphopeptide sequences after FDR optimization");
                        summary.Write(overall_decoy_unique_unique_phospho.ToString() + ',');
                    }

                    if(unique)
                    {
                        summary.Write("n/a");
                    }

                    summary.WriteLine();

                    overall_log.Close();

                    summary.Close();
                }

                onFinished(new EventArgs());
            //}
            //catch(Exception ex)
            //{
            //    onThrowException(new ExceptionEventArgs(ex));
            //}
            //finally
            //{
                if(overall_scans_output != null)
                {
                    overall_scans_output.Close();
                }
                if(overall_target_output != null)
                {
                    overall_target_output.Close();
                }
                if(overall_decoy_output != null)
                {
                    overall_decoy_output.Close();
                }
                if(overall_target_unique_output != null)
                {
                    overall_target_unique_output.Close();
                }
                if(overall_decoy_unique_output != null)
                {
                    overall_decoy_unique_output.Close();
                }
                if(overall_scans_phospho_output != null)
                {
                    overall_scans_phospho_output.Close();
                }
                if(overall_target_phospho_output != null)
                {
                    overall_target_phospho_output.Close();
                }
                if(overall_decoy_phospho_output != null)
                {
                    overall_decoy_phospho_output.Close();
                }
                if(overall_target_unique_phospho_output != null)
                {
                    overall_target_unique_phospho_output.Close();
                }
                if(overall_decoy_unique_phospho_output != null)
                {
                    overall_decoy_unique_phospho_output.Close();
                }
                if(overall_log != null)
                {
                    overall_log.Close();
                }
                if(log != null)
                {
                    log.Close();
                }
                if(summary != null)
                {
                    summary.Close();
                }
                if(raw != null)
                {
                    raw.Close();
                }
                if(csv != null)
                {
                    csv.Close();
                }
                if(scans_output != null)
                {
                    scans_output.Close();
                }
                if(target_output != null)
                {
                    target_output.Close();
                }
                if(decoy_output != null)
                {
                    decoy_output.Close();
                }
                if(target_phospho_output != null)
                {
                    target_phospho_output.Close();
                }
                if(decoy_phospho_output != null)
                {
                    decoy_phospho_output.Close();
                }
                if(target_unique_output != null)
                {
                    target_unique_output.Close();
                }
                if(decoy_unique_output != null)
                {
                    decoy_unique_output.Close();
                }
                if(target_unique_phospho_output != null)
                {
                    target_unique_phospho_output.Close();
                }
                if(decoy_unique_phospho_output != null)
                {
                    decoy_unique_phospho_output.Close();
                }
                if(overall_target_unique_unique_output != null)
                {
                    overall_target_unique_unique_output.Close();
                }
                if(overall_decoy_unique_unique_output != null)
                {
                    overall_decoy_unique_unique_output.Close();
                }
                if(overall_target_unique_unique_phospho_output != null)
                {
                    overall_target_unique_unique_phospho_output.Close();
                }
            //}
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

        private static int AscendingPeptideHitQValueThenEValueScoreComparison(PeptideHit left, PeptideHit right)
        {
            int comparison = left.QValue.CompareTo(right.QValue);
            if(comparison != 0)
            {
                return comparison;
            }
            else
            {
                return left.EValueScore.CompareTo(right.EValueScore);
            }
        }
    }
}