using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BatchLowResFdrOptimizer
{
    public class BatchLowResFdrOptimizer
    {
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
            EventHandler<ProgressEventArgs> handler = UpdateProgress;

            if(handler != null)
            {
                handler(this, e);
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
        private bool higherScoresAreBetter;
        private double maximumFalseDiscoveryRate;
        private bool unique;
        private bool overallOutputs;
        private bool phosphopeptideOutputs;
        private string outputFolder;

        public BatchLowResFdrOptimizer(IList<string> csvFilepaths, 
            bool higherScoresAreBetter,
            double maximumFalseDiscoveryRate,
            bool unique,
            bool overallOutputs, bool phosphopeptideOutputs, string outputFolder)
        {
            this.csvFilepaths = csvFilepaths;
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

            try
            {
                onStarting(new EventArgs());

                onUpdateProgress(new ProgressEventArgs(0));

                long total_bytes = 0;
                foreach(string csv_filepath in csvFilepaths)
                {
                    total_bytes += new FileInfo(csv_filepath).Length;
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
                    overall_log = new StreamWriter(Path.Combine(overall_output_folder, "Batch_Low-Res_FDR_Optimizer_log.txt"));
                    overall_log.AutoFlush = true;

                    overall_log.WriteLine("Batch Low-Res FDR Optimizer PARAMETERS");
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
                }

                Dictionary<string, List<string>> summary_info = new Dictionary<string, List<string>>();

                string extended_header_line = null;
                bool overall_header_written = false;

                Dictionary<string, SortedDictionary<int, PeptideHit>> all_scans_peptides = new Dictionary<string, SortedDictionary<int, PeptideHit>>();

                long bytes_read = 0;
                foreach(string csv_filepath in csvFilepaths)
                {
                    onStartingFile(new FilepathEventArgs(csv_filepath));

                    string log_filepath = Path.Combine(log_folder, Path.GetFileNameWithoutExtension(csv_filepath) + "_log.txt");
                    log = new StreamWriter(log_filepath);
                    log.AutoFlush = true;

                    log.WriteLine("Batch Low-Res FDR Optimizer PARAMETERS");
                    log.WriteLine("Higher Scores are Better: " + higherScoresAreBetter.ToString());
                    log.WriteLine("Maximum False Discovery Rate (%): " + maximumFalseDiscoveryRate.ToString());
                    log.WriteLine("FDR Calculation and Optimization Based on Unique Peptide Sequences: " + unique.ToString());
                    log.WriteLine();

                    SortedDictionary<int, FullPeptideHit> scans_full_peptides = new SortedDictionary<int, FullPeptideHit>();
                    SortedDictionary<int, PeptideHit> scans_peptides = new SortedDictionary<int, PeptideHit>();

                    if(overallOutputs)
                    {
                        overall_log.WriteLine(csv_filepath);
                    }
                    log.WriteLine(csv_filepath);
                    summary_info.Add(csv_filepath, new List<string>());

                    csv = new StreamReader(csv_filepath);

                    string header_line = csv.ReadLine();
                    bytes_read += header_line.Length + 2;
                    extended_header_line = header_line + ", Q-Value (%)";
                    if(overallOutputs && !overall_header_written)
                    {
                        overall_scans_output.WriteLine(extended_header_line);
                        overall_target_output.WriteLine(extended_header_line);
                        overall_decoy_output.WriteLine(extended_header_line);
                        overall_target_unique_output.WriteLine(extended_header_line);
                        overall_decoy_unique_output.WriteLine(extended_header_line);
                        if(phosphopeptideOutputs)
                        {
                            overall_scans_phospho_output.WriteLine(extended_header_line);
                            overall_target_phospho_output.WriteLine(extended_header_line);
                            overall_decoy_phospho_output.WriteLine(extended_header_line);
                            overall_target_unique_phospho_output.WriteLine(extended_header_line);
                            overall_decoy_unique_phospho_output.WriteLine(extended_header_line);
                        }

                        overall_header_written = true;
                    }

                    while(csv.Peek() != -1)
                    {
                        string line = csv.ReadLine();
                        bytes_read += line.Length + 2;

                        string[] fields = Regex.Split(line, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))");  // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621

                        int scan_number = int.Parse(fields[0]);
                        string sequence = fields[2];
                        double evalue_score = double.Parse(fields[3]);
                        bool decoy = fields[9].Contains("DECOY") || fields[9].Contains("REVERSED");
                        string dynamic_modifications = fields[10];
                        int charge = int.Parse(fields[11]);

                        if(!scans_full_peptides.ContainsKey(scan_number))
                        {
                            scans_full_peptides.Add(scan_number, new FullPeptideHit(line, sequence, dynamic_modifications, evalue_score, decoy));
                            scans_peptides.Add(scan_number, new PeptideHit(sequence, dynamic_modifications, evalue_score, decoy));
                            if(dynamic_modifications.Contains("phosphorylation"))
                            {
                                overall_scans_phospho++;
                            }
                        }
                        else if((!higherScoresAreBetter && evalue_score < scans_full_peptides[scan_number].EValueScore
                            || (evalue_score == scans_full_peptides[scan_number].EValueScore && scans_full_peptides[scan_number].Decoy && !decoy))
                            || (higherScoresAreBetter && evalue_score > scans_full_peptides[scan_number].EValueScore
                            || (evalue_score == scans_full_peptides[scan_number].EValueScore && scans_full_peptides[scan_number].Decoy && !decoy)))
                        {
                            scans_full_peptides[scan_number] = new FullPeptideHit(line, dynamic_modifications, sequence, evalue_score, decoy);
                            scans_peptides[scan_number] = new PeptideHit(sequence, dynamic_modifications, evalue_score, decoy);
                        }

                        double progress = (double)bytes_read / total_bytes;
                        onUpdateProgress(new ProgressEventArgs((int)(progress * 100.0)));
                    }

                    csv.Close();

                    overall_scans += scans_full_peptides.Count;
                    all_scans_peptides.Add(csv_filepath, scans_peptides);

                    if(overallOutputs)
                    {
                        overall_log.WriteLine();
                    }
                    log.WriteLine();

                    List<PeptideHit> peptides = new List<PeptideHit>(scans_peptides.Values);
                    peptides.Sort(new AscendingPeptideHitEValueScoreComparer(higherScoresAreBetter));
                    List<FullPeptideHit> full_peptides = new List<FullPeptideHit>(scans_full_peptides.Values);
                    full_peptides.Sort(new AscendingFullPeptideHitEValueScoreComparer(higherScoresAreBetter));
                    int q_cumulative_target_peptides = 0;
                    int q_cumulative_decoy_peptides = 0;
                    Dictionary<string, int> q_target_peptides = new Dictionary<string, int>();
                    Dictionary<string, int> q_decoy_peptides = new Dictionary<string, int>();
                    int p3 = 0;
                    while(p3 < peptides.Count)
                    {
                        PeptideHit peptide = peptides[p3];
                        PeptideHit full_peptide = full_peptides[p3];
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
                        full_peptide.QValue = q_value;

                        int p4 = p3 - 1 - 1;
                        while(p4 >= 0)
                        {
                            if(double.IsNaN(peptides[p4].QValue))
                            {
                                peptides[p4].QValue = q_value;
                                full_peptides[p4].QValue = q_value;
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
                            full_peptides[p5].QValue = full_peptides[p5 + 1].QValue;
                        }
                        p5--;
                    }

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
                    foreach(FullPeptideHit peptide in scans_full_peptides.Values)
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

                    log.WriteLine(scans_filepath);
                    log.WriteLine(scans_full_peptides.Count.ToString() + " MS/MS scans resulted in at least one peptide hit");
                    summary_info[csv_filepath].Add(scans_full_peptides.Count.ToString());
                    if(phosphopeptideOutputs)
                    {
                        log.WriteLine(scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
                        summary_info[csv_filepath].Add(scans_phospho.ToString());
                    }
                    log.WriteLine();

                    log.Close();
                }

                List<PeptideHit> all_peptides = new List<PeptideHit>();
                foreach(SortedDictionary<int, PeptideHit> kvp in all_scans_peptides.Values)
                {
                    all_peptides.AddRange(kvp.Values);
                }
                all_peptides.Sort(new AscendingPeptideHitQValueComparer(higherScoresAreBetter));
                int best_target = 0;
                int best_decoy = 0;
                double best_false_discovery_rate = double.NaN;
                double best_q_value = double.NegativeInfinity;
                double best_evalue_score = higherScoresAreBetter ? double.PositiveInfinity : double.NegativeInfinity;

                List<PeptideHit> all_filtered_peptides = new List<PeptideHit>(all_peptides);
                int target_count = 0;
                int decoy_count = 0;
                Dictionary<string, int> target_unique_count = new Dictionary<string, int>();
                Dictionary<string, int> decoy_unique_count = new Dictionary<string, int>();
                int p2 = 0;
                while(p2 < all_filtered_peptides.Count)
                {
                    PeptideHit peptide = all_filtered_peptides[p2];

                    if(!peptide.Decoy)
                    {
                        target_count++;
                        if(!target_unique_count.ContainsKey(peptide.Sequence))
                        {
                            target_unique_count.Add(peptide.Sequence, 0);
                        }
                        target_unique_count[peptide.Sequence]++;
                    }
                    else
                    {
                        decoy_count++;
                        if(!decoy_unique_count.ContainsKey(peptide.Sequence))
                        {
                            decoy_unique_count.Add(peptide.Sequence, 0);
                        }
                        decoy_unique_count[peptide.Sequence]++;
                    }

                    p2++;

                    if(p2 < all_filtered_peptides.Count)
                    {
                        PeptideHit next_peptide = all_filtered_peptides[p2];
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
                        }
                    }
                }

                SortedDictionary<string, FullPeptideHit> overall_target_peptides = new SortedDictionary<string, FullPeptideHit>();
                SortedDictionary<string, FullPeptideHit> overall_decoy_peptides = new SortedDictionary<string, FullPeptideHit>();

                foreach(string csv_filepath in csvFilepaths)
                {
                    string log_filepath = Path.Combine(Path.Combine(outputFolder, "log"), Path.GetFileNameWithoutExtension(csv_filepath) + "_log.txt");
                    log = new StreamWriter(log_filepath, true);
                    log.AutoFlush = true;

                    StreamReader scans_csv = new StreamReader(Path.Combine(Path.Combine(outputFolder, "scans"), Path.GetFileNameWithoutExtension(csv_filepath) + "_scans.csv"));

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

                    SortedDictionary<string, FullPeptideHit> target_unique = new SortedDictionary<string, FullPeptideHit>();
                    SortedDictionary<string, FullPeptideHit> decoy_unique = new SortedDictionary<string, FullPeptideHit>();
                    int redundant_target = 0;
                    int redundant_decoy = 0;
                    int redundant_target_phospho = 0;
                    int redundant_decoy_phospho = 0;

                    scans_csv.ReadLine();

                    while(scans_csv.Peek() != -1)
                    {
                        string csv_line = scans_csv.ReadLine();

                        string[] csv_fields = Regex.Split(csv_line, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))");  // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621

                        string sequence = csv_fields[2];
                        double evalue_score = double.Parse(csv_fields[3]);
                        bool decoy = csv_fields[9].Contains("DECOY") || csv_fields[9].Contains("REVERSED");
                        string dynamic_modifications = csv_fields[10];
                        double q_value = double.Parse(csv_fields[15]);
                        FullPeptideHit peptide = new FullPeptideHit(csv_line, sequence, dynamic_modifications, evalue_score, decoy, q_value);

                        if((peptide.QValue < best_q_value || (peptide.QValue == best_q_value
                            && ((!higherScoresAreBetter && peptide.EValueScore <= best_evalue_score)
                            || (higherScoresAreBetter && peptide.EValueScore >= best_evalue_score)))))
                        {
                            if(!peptide.Decoy)
                            {
                                redundant_target++;
                                if(overallOutputs)
                                {
                                    overall_target_output.WriteLine(peptide.Line);
                                }
                                target_output.WriteLine(peptide.Line);
                                if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                                {
                                    redundant_target_phospho++;
                                    if(overallOutputs)
                                    {
                                        overall_target_phospho_output.WriteLine(peptide.Line);
                                    }
                                    target_phospho_output.WriteLine(peptide.Line);
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
                                    && peptide.EValueScore == target_unique[peptide.Sequence].EValueScore))
                                {
                                    target_unique[peptide.Sequence] = peptide;
                                }
                            }
                            else
                            {
                                redundant_decoy++;
                                if(overallOutputs)
                                {
                                    overall_decoy_output.WriteLine(peptide.Line);
                                }
                                decoy_output.WriteLine(peptide.Line);
                                if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                                {
                                    redundant_decoy_phospho++;
                                    if(overallOutputs)
                                    {
                                        overall_decoy_phospho_output.WriteLine(peptide.Line);
                                    }
                                    decoy_phospho_output.WriteLine(peptide.Line);
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
                                    && peptide.EValueScore == decoy_unique[peptide.Sequence].EValueScore))
                                {
                                    decoy_unique[peptide.Sequence] = peptide;
                                }
                            }
                        }
                    }

                    scans_csv.Close();

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

                    log.WriteLine("Q-Value Threshold (%): " + best_q_value.ToString());
                    summary_info[csv_filepath].Add(best_q_value.ToString());
                    log.WriteLine("E-Value Score Threshold: " + best_evalue_score.ToString());
                    summary_info[csv_filepath].Add(best_evalue_score.ToString());
                    log.WriteLine();

                    log.WriteLine(target_filepath);
                    log.WriteLine(redundant_target.ToString() + " target peptides after FDR optimization");
                    summary_info[csv_filepath].Add(redundant_target.ToString());
                    log.WriteLine(decoy_filepath);
                    log.WriteLine(redundant_decoy.ToString() + " decoy peptides after FDR optimization");
                    summary_info[csv_filepath].Add(redundant_decoy.ToString());

                    if(phosphopeptideOutputs)
                    {
                        log.WriteLine(target_phospho_filepath);
                        log.WriteLine(redundant_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
                        summary_info[csv_filepath].Add(redundant_target_phospho.ToString());
                        log.WriteLine(decoy_phospho_filepath);
                        log.WriteLine(redundant_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
                        summary_info[csv_filepath].Add(redundant_decoy_phospho.ToString());
                    }

                    log.WriteLine();

                    if(!unique)
                    {
                        log.WriteLine("FDR (%): " + ((double)redundant_decoy / redundant_target * 100.0).ToString());
                        summary_info[csv_filepath].Add(((double)redundant_decoy / redundant_target * 100.0).ToString());
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
                    foreach(FullPeptideHit peptide in target_unique.Values)
                    {
                        if(overallOutputs)
                        {
                            overall_target_unique_output.WriteLine(peptide.Line);
                        }
                        target_unique_output.WriteLine(peptide.Line);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                        {
                            target_unique_phospho++;
                            if(overallOutputs)
                            {
                                overall_target_unique_phospho_output.WriteLine(peptide.Line);
                            }
                            target_unique_phospho_output.WriteLine(peptide.Line);
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
                            && peptide.EValueScore == overall_target_peptides[peptide.Sequence].EValueScore))
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
                    foreach(FullPeptideHit peptide in decoy_unique.Values)
                    {
                        if(overallOutputs)
                        {
                            overall_decoy_unique_output.WriteLine(peptide.ExtendedLine);
                        }
                        decoy_unique_output.WriteLine(peptide.Line);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                        {
                            decoy_unique_phospho++;
                            if(overallOutputs)
                            {
                                overall_decoy_unique_phospho_output.WriteLine(peptide.Line);
                            }
                            decoy_unique_phospho_output.WriteLine(peptide.Line);
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
                            && peptide.EValueScore == overall_decoy_peptides[peptide.Sequence].EValueScore))
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

                    log.WriteLine(target_unique_filepath);
                    log.WriteLine(target_unique.Count.ToString() + " unique target peptide sequences after FDR optimization");
                    summary_info[csv_filepath].Add(target_unique.Count.ToString());
                    log.WriteLine(decoy_unique_filepath);
                    log.WriteLine(decoy_unique.Count.ToString() + " unique decoy peptide sequences after FDR optimization");
                    summary_info[csv_filepath].Add(decoy_unique.Count.ToString());

                    if(phosphopeptideOutputs)
                    {
                        log.WriteLine(target_unique_phospho_filepath);
                        log.WriteLine(target_unique_phospho.ToString() + " unique target phoshopeptide sequences after FDR optimization");
                        summary_info[csv_filepath].Add(target_unique_phospho.ToString());
                        log.WriteLine(decoy_unique_phospho_filepath);
                        log.WriteLine(decoy_unique_phospho.ToString() + " unique decoy phoshopeptide sequences after FDR optimization");
                        summary_info[csv_filepath].Add(decoy_unique_phospho.ToString());
                    }

                    if(unique)
                    {
                        log.WriteLine();
                        log.WriteLine("FDR (%): " + ((double)decoy_unique.Count / target_unique.Count * 100.0).ToString());
                        summary_info[csv_filepath].Add(((double)decoy_unique.Count / target_unique.Count * 100.0).ToString());
                    }

                    log.Close();

                    onFinishedFile(new FilepathEventArgs(csv_filepath));
                }

                if(overallOutputs)
                {
                    summary_info.Add("SUM", new List<string>());

                    summary_info.Add("OVERALL", new List<string>());

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

                    foreach(FullPeptideHit peptide in overall_target_peptides.Values)
                    {
                        overall_target_unique_unique_output.WriteLine(peptide.Line);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                        {
                            overall_target_unique_unique_phospho++;
                            overall_target_unique_unique_phospho_output.WriteLine(peptide.Line);
                        }
                    }

                    foreach(FullPeptideHit peptide in overall_decoy_peptides.Values)
                    {
                        overall_decoy_unique_unique_output.WriteLine(peptide.Line);
                        if(phosphopeptideOutputs && peptide.DynamicModifications.Contains("phosphorylation"))
                        {
                            overall_decoy_unique_unique_phospho++;
                            overall_decoy_unique_unique_phospho_output.WriteLine(peptide.Line);
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
                    summary_info["SUM"].Add(overall_scans.ToString());
                    summary_info["OVERALL"].Add("n/a");

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_scans_phospho.ToString() + " MS/MS scans resulted in at least one phosphopeptide hit");
                        summary_info["SUM"].Add(overall_scans_phospho.ToString());
                        summary_info["OVERALL"].Add("n/a");
                    }

                    overall_log.WriteLine();

                    overall_log.WriteLine("Q-Value Threshold (%): " + best_q_value.ToString());
                    summary_info["SUM"].Add(best_q_value.ToString());
                    summary_info["OVERALL"].Add(best_q_value.ToString());
                    overall_log.WriteLine("E-Value Score Threshold: " + best_evalue_score.ToString());
                    summary_info["SUM"].Add(best_evalue_score.ToString());
                    summary_info["OVERALL"].Add(best_evalue_score.ToString());
                    overall_log.WriteLine();

                    overall_log.WriteLine(overall_target_filepath);
                    overall_log.WriteLine(overall_target.ToString() + " target peptides after FDR optimization");
                    summary_info["SUM"].Add(overall_target.ToString());
                    summary_info["OVERALL"].Add("n/a");
                    overall_log.WriteLine(overall_decoy_filepath);
                    overall_log.WriteLine(overall_decoy.ToString() + " decoy peptides after FDR optimization");
                    summary_info["SUM"].Add(overall_decoy.ToString());
                    summary_info["OVERALL"].Add("n/a");

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_phospho_filepath);
                        overall_log.WriteLine(overall_target_phospho.ToString() + " target phosphopeptides after FDR optimization");
                        summary_info["SUM"].Add(overall_target_phospho.ToString());
                        summary_info["OVERALL"].Add("n/a");
                        overall_log.WriteLine(overall_decoy_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_phospho.ToString() + " decoy phosphopeptides after FDR optimization");
                        summary_info["SUM"].Add(overall_decoy_phospho.ToString());
                        summary_info["OVERALL"].Add("n/a");
                    }

                    overall_log.WriteLine();

                    if(!unique)
                    {
                        overall_log.WriteLine("FDR (%): " + ((double)overall_decoy / overall_target * 100.0).ToString());
                        summary_info["SUM"].Add(((double)overall_decoy / overall_target * 100.0).ToString());
                        summary_info["OVERALL"].Add("n/a");
                        overall_log.WriteLine();
                    }

                    overall_log.WriteLine(overall_target_unique_filepath);
                    overall_log.WriteLine(overall_target_unique.ToString() + " target unique peptide sequences after FDR optimization");
                    summary_info["SUM"].Add(overall_target_unique.ToString());
                    overall_log.WriteLine(overall_decoy_unique_filepath);
                    overall_log.WriteLine(overall_decoy_unique.ToString() + " decoy unique peptide sequences after FDR optimization");
                    summary_info["SUM"].Add(overall_decoy_unique.ToString());

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_unique_phospho_filepath);
                        overall_log.WriteLine(overall_target_unique_phospho.ToString() + " target unique phosphopeptide sequences after FDR optimization");
                        summary_info["SUM"].Add(overall_target_unique_phospho.ToString());
                        overall_log.WriteLine(overall_decoy_unique_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_unique_phospho.ToString() + " decoy unique phosphopeptide sequences after FDR optimization");
                        summary_info["SUM"].Add(overall_decoy_unique_phospho.ToString());
                    }

                    overall_log.WriteLine();

                    overall_log.WriteLine(overall_target_unique_unique_filepath);
                    overall_log.WriteLine(overall_target_peptides.Count.ToString() + " target unique unique peptide sequences after FDR optimization");
                    summary_info["OVERALL"].Add(overall_target_peptides.Count.ToString());
                    overall_log.WriteLine(overall_decoy_unique_unique_filepath);
                    overall_log.WriteLine(overall_decoy_peptides.Count.ToString() + " decoy unique unique peptide sequences after FDR optimization");
                    summary_info["OVERALL"].Add(overall_decoy_peptides.Count.ToString());

                    if(phosphopeptideOutputs)
                    {
                        overall_log.WriteLine(overall_target_unique_unique_phospho_filepath);
                        overall_log.WriteLine(overall_target_unique_unique_phospho.ToString() + " target unique unique phosphopeptide sequences after FDR optimization");
                        summary_info["OVERALL"].Add(overall_target_unique_unique_phospho.ToString());
                        overall_log.WriteLine(overall_decoy_unique_unique_phospho_filepath);
                        overall_log.WriteLine(overall_decoy_unique_unique_phospho.ToString() + " decoy unique unique phosphopeptide sequences after FDR optimization");
                        summary_info["OVERALL"].Add(overall_decoy_unique_unique_phospho.ToString());
                    }

                    if(unique)
                    {
                        overall_log.WriteLine();
                        overall_log.WriteLine("FDR (%): " + ((double)overall_decoy_peptides.Count / overall_target_peptides.Count * 100.0).ToString());
                        summary_info["SUM"].Add("n/a");
                        summary_info["OVERALL"].Add(((double)overall_decoy_peptides.Count / overall_target_peptides.Count * 100.0).ToString());
                    }

                    overall_log.Close();

                    summary = new StreamWriter(Path.Combine(overall_output_folder, "summary.csv"));
                    summary.WriteLine("CSV Filepath,Scans," + (phosphopeptideOutputs ? "Phosphopeptide Scans," : null) + "Q-Value Threshold (%),E-Value Score Threshold,Target Peptides,Decoy Peptides," + (phosphopeptideOutputs ? "Target Phosphopeptides,Decoy Phosphopeptides," : null) + (unique ? null : "FDR (%),") + "Unique Target Peptides,Unique Decoy Peptides," + (phosphopeptideOutputs ? "Unique Target Phosphopeptides,Unique Decoy Phosphopeptides," : null) + (unique ? "FDR (%)" : null));
                    foreach(KeyValuePair<string, List<string>> kvp in summary_info)
                    {
                        summary.Write(kvp.Key + ',');
                        foreach(string item in kvp.Value)
                        {
                            summary.Write(item + ',');
                        }
                        summary.WriteLine();
                    }
                    summary.Close();
                }

                onFinished(new EventArgs());
            }
            catch(Exception ex)
            {
                onThrowException(new ExceptionEventArgs(ex));
            }
            finally
            {
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
            }
        }
    }
}