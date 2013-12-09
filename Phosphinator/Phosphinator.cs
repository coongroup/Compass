using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LumenWorks.Framework.IO.Csv;
using Net.Kniaz.LMA;
using MSFileReaderLib;

namespace Phosphinator
{
    public class Phosphinator
    {
        private const double PROTON_MASS = 1.00727638;

        private static readonly Converter<double, double> SET_DOUBLE_VALUE_TO_NAN = new Converter<double, double>(delegate(double x) { return double.NaN; });
        private static readonly Converter<double, double> SET_DOUBLE_VALUE_TO_ZERO = new Converter<double, double>(delegate(double x) { return 0.0; });

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

        public event EventHandler FinishedFile;

        protected virtual void onFinishedFile(EventArgs e)
        {
            EventHandler handler = FinishedFile;

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
        private double intensityThreshold;
        private IntensityThresholdType intensityThresholdType;
        private double mzTolerance;
        private double ambiguityScoreThreshold;
        private bool eliminatePrecursorInterference;
        private double precursorInterferenceThreshold;
        private bool motifXOutput;
        private string motifXFastaProteinDatabaseFilepath;
        private int motifXWindowSize;
        private string outputFolder;

        public Phosphinator(IList<string> csvFilepaths,
            string rawFolder,
            IEnumerable<Modification> fixedModifications,
            double intensityThreshold, IntensityThresholdType intensityThresholdType,
            double mzTolerance,
            double ambiguityScoreThreshold,
            bool eliminatePrecursorInterference, double precursorInterferenceThreshold,
            bool motifXOutput, string motifXFastaProteinDatabaseFilepath, int motifXWindowSize,
            string outputFolder)
        {
            this.csvFilepaths = csvFilepaths;
            this.rawFolder = rawFolder;
            this.fixedModifications = fixedModifications;
            this.intensityThreshold = intensityThreshold;
            this.intensityThresholdType = intensityThresholdType;
            this.mzTolerance = mzTolerance;
            this.ambiguityScoreThreshold = ambiguityScoreThreshold;
            this.eliminatePrecursorInterference = eliminatePrecursorInterference;
            this.precursorInterferenceThreshold = precursorInterferenceThreshold;
            this.motifXOutput = motifXOutput;
            this.motifXFastaProteinDatabaseFilepath = motifXFastaProteinDatabaseFilepath;
            this.motifXWindowSize = motifXWindowSize;
            this.outputFolder = outputFolder;
        }

        public void Phosphinate()
        {
            StreamWriter log = null;
            IXRawfile2 raw = null;
            StreamReader csv = null;
            StreamWriter non_phospho_output = null;
            StreamWriter localized_phospho_output = null;
            StreamWriter unlocalized_phospho_output = null;
            StreamWriter motifX = null;

            try
            {
                onStarting(new EventArgs());

                onUpdateProgress(new ProgressEventArgs(0));

                StringBuilder fixed_modifications_sb = new StringBuilder();
                foreach (Modification modification in fixedModifications)
                {
                    fixed_modifications_sb.Append(modification.Name + ", ");
                }
                if (fixed_modifications_sb.Length > 0)
                {
                    fixed_modifications_sb = fixed_modifications_sb.Remove(fixed_modifications_sb.Length - 2, 2);
                }
                string fixed_modifications = fixed_modifications_sb.ToString();

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                log = new StreamWriter(Path.Combine(outputFolder, "Phosphinator_log.txt"));
                log.AutoFlush = true;

                log.WriteLine("Phosphinator PARAMETERS");
                log.WriteLine("Fixed Modifications: " + fixed_modifications);
                log.WriteLine("Fragment Intensity Threshold: " + intensityThreshold.ToString() + " (" + intensityThresholdType.ToString() + ')');
                log.WriteLine("Fragment m/z Tolerance (Th): " + mzTolerance.ToString());
                log.WriteLine("Ambiguity Score Threshold: " + ambiguityScoreThreshold.ToString());
                log.WriteLine("Eliminate Precursor Interference: " + eliminatePrecursorInterference.ToString());
                if (eliminatePrecursorInterference)
                {
                    log.WriteLine("Precursor Interference Threshold: " + precursorInterferenceThreshold.ToString());
                }
                if (motifXOutput)
                {
                    log.WriteLine("Motif-X Fasta Protein Database Filepath: " + motifXFastaProteinDatabaseFilepath);
                    log.WriteLine("Motif-X Window Size: " + motifXWindowSize.ToString());
                }
                log.WriteLine();

                ProteinSiteCounter identified_sites_by_protein = new ProteinSiteCounter();
                ProteinSiteCounter localized_sites_by_protein = new ProteinSiteCounter();
                ProteinSiteCounter unlocalized_sites_by_protein = new ProteinSiteCounter();

                Dictionary<string, Dictionary<KeyValuePair<int, string>, List<string>>> localized =
                    new Dictionary<string, Dictionary<KeyValuePair<int, string>, List<string>>>();
                Dictionary<string, Dictionary<KeyValuePair<int, string>, List<string>>> unlocalized =
                    new Dictionary<string, Dictionary<KeyValuePair<int, string>, List<string>>>();

                non_phospho_output = new StreamWriter(Path.Combine(outputFolder, "non_phospho.csv"));
                localized_phospho_output = new StreamWriter(Path.Combine(outputFolder, "localized_phospho.csv"));
                unlocalized_phospho_output = new StreamWriter(Path.Combine(outputFolder, "unlocalized_phospho.csv"));

                ProteinDictionary proteins = null;
                Dictionary<string, int> motifs = null;
                if (motifXOutput)
                {
                    proteins = new ProteinDictionary(motifXFastaProteinDatabaseFilepath);
                    motifs = new Dictionary<string, int>();
                    motifX = new StreamWriter(Path.Combine(outputFolder, "motif-x.txt"));
                }

                raw = (IXRawfile2) new MSFileReader_XRawfile();

                string header_line = null;
                string[] headers = null;
                bool quant = false;

                foreach (string csv_filepath in csvFilepaths)
                {
                    onStartingFile(new FilepathEventArgs(csv_filepath));

                    csv = new StreamReader(csv_filepath);

                    using (CsvReader reader = new CsvReader(csv, true))
                    {
                        headers = reader.GetFieldHeaders();

                        header_line = string.Join(",", headers);
                        quant = headers.Contains("Channels Detected");
                        string[] lineData = new string[headers.Length];
                        //header_line = csv.ReadLine();
                        //quant = header_line.Contains("TQ_");

                        non_phospho_output.WriteLine(header_line);
                        localized_phospho_output.WriteLine(header_line +
                                                           ", Number of Theoretical Fragments, Identified Phosphoisoform, Identified Phosphoisoform Number of Matching Fragments, Best Phosphoisoforms, Best Phosphoisoform, Best Phosphoisoform Number of Matching Fragments, Second-Best Phosphoisoform, Second-Best Phosphoisoform Number of Matching Fragments, Identified Phosphoisoform Correct?, Preliminary Localization of All Phosphorylations?, Peptide Phosphorylation Sites, Probability of Spurious Fragment Match, Number of Theoretical Site-Determining Fragment Ions, Number of Matching Site-Determining Fragment Ions, Matching Site-Determining Fragment Ions, Probability Values, Ambiguity Scores, Phosphorylation Sites Localized?, All Phosphorylation Sites Localized?");
                        unlocalized_phospho_output.WriteLine(header_line +
                                                             ", Number of Theoretical Fragments, Identified Phosphoisoform, Identified Phosphoisoform Number of Matching Fragments, Best Phosphoisoforms, Best Phosphoisoform, Best Phosphoisoform Number of Matching Fragments, Second-Best Phosphoisoform, Second-Best Phosphoisoform Number of Matching Fragments, Identified Phosphoisoform Correct?, Preliminary Localization of All Phosphorylations?, Peptide Phosphorylation Sites, Probability of Spurious Fragment Match, Number of Theoretical Site-Determining Fragment Ions, Number of Matching Site-Determining Fragment Ions, Matching Site-Determining Fragment Ions, Probability Values, Ambiguity Scores, Phosphorylation Sites Localized?, All Phosphorylation Sites Localized?");

                        while (reader.ReadNextRecord())
                        {
                            //string line = csv.ReadLine();



                            //string[] fields = Regex.Split(line,
                            //    @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))");
                            //    // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621

                            string sequence = reader["Peptide"];

                            string dynamic_modifications = reader["Mods"];
                            if (!dynamic_modifications.Contains("phosphorylation"))
                            {
                                //non_phospho_output.WriteLine(line);
                            }
                            else
                            {
                                Peptide identified_phosphopeptide = new Peptide(sequence, fixedModifications,
                                    dynamic_modifications);

                                int start_residue = int.Parse(reader["Start"]);
                                int stop_residue = int.Parse(reader["Stop"]);
                                string protein_description = reader["Defline"].Trim('"');

                                StringBuilder sb = new StringBuilder();
                                reader.CopyCurrentRecordTo(lineData);
                                foreach (string datum in lineData)
                                {
                                    if (datum.Contains(','))
                                        sb.Append("\"" + datum + "\"");
                                    else
                                        sb.Append(datum);
                                    sb.Append(',');
                                }
                                sb.Remove(sb.Length - 1, 1);
                                string line = sb.ToString();
                                if (!identified_sites_by_protein.ContainsKey(protein_description))
                                {
                                    identified_sites_by_protein.Add(protein_description, new Dictionary<string, int>());
                                }
                                foreach (KeyValuePair<int, string> kvp in identified_phosphopeptide.DynamicModifications)
                                {
                                    if (kvp.Value.Contains("phosphorylation"))
                                    {
                                        string site = sequence[kvp.Key - 1] + (start_residue + kvp.Key).ToString();

                                        if (!identified_sites_by_protein[protein_description].ContainsKey(site))
                                        {
                                            identified_sites_by_protein[protein_description].Add(site, 0);
                                        }
                                        identified_sites_by_protein[protein_description][site]++;
                                    }
                                }

                                int scan_number = int.Parse(reader["Spectrum number"]);
                                string filenameID = reader["Filename/id"];
                                FragmentType[] fragment_types = null;
                                if (filenameID.Contains(".ETD.") || filenameID.Contains(".ECD."))
                                {
                                    fragment_types = new FragmentType[] {FragmentType.c, FragmentType.zdot};
                                }
                                else
                                {
                                    fragment_types = new FragmentType[] {FragmentType.b, FragmentType.y};
                                }
                                string raw_filename = filenameID.Substring(0, filenameID.IndexOf('.')) + ".raw";
                                int charge = int.Parse(reader["Charge"]);

                                string current_raw_filename = null;
                                raw.GetFileName(ref current_raw_filename);

                                if (current_raw_filename == null ||
                                    !raw_filename.Equals(Path.GetFileName(current_raw_filename),
                                        StringComparison.InvariantCultureIgnoreCase))
                                {
                                    raw.Close();
                                    string[] raw_filepaths = null;
                                    if (!string.IsNullOrEmpty(rawFolder) && Directory.Exists(rawFolder))
                                    {
                                        raw_filepaths = Directory.GetFiles(rawFolder, raw_filename,
                                            SearchOption.AllDirectories);
                                    }
                                    else
                                    {
                                        raw_filepaths = Directory.GetFiles(Path.GetDirectoryName(csv_filepath),
                                            raw_filename, SearchOption.AllDirectories);
                                    }
                                    if (raw_filepaths.Length == 0)
                                    {
                                        throw new FileNotFoundException("No corresponding .raw file found for " +
                                                                        csv_filepath);
                                    }
                                    if (raw_filepaths.Length > 1)
                                    {
                                        throw new Exception("Multiple corresponding .raw files found for " +
                                                            csv_filepath);
                                    }
                                    raw.Open(raw_filepaths[0]);
                                    raw.SetCurrentController(0, 1);
                                }

                                string scan_filter = null;
                                raw.GetFilterForScanNum(scan_number, ref scan_filter);

                                string low_mz_scan_filter = scan_filter.Substring(scan_filter.IndexOf('[') + 1);
                                double low_mz =
                                    double.Parse(low_mz_scan_filter.Substring(0, low_mz_scan_filter.IndexOf('-')));
                                string high_mz_scan_filter = scan_filter.Substring(scan_filter.LastIndexOf('-') + 1);
                                double high_mz =
                                    double.Parse(high_mz_scan_filter.Substring(0, high_mz_scan_filter.IndexOf(']')));

                                double[,] spectrum = null;
                                if (scan_filter.Contains("FTMS"))
                                {
                                    object labels_obj = null;
                                    object flags_obj = null;
                                    raw.GetLabelData(ref labels_obj, ref flags_obj, ref scan_number);
                                    spectrum = (double[,]) labels_obj;
                                }
                                else
                                {
                                    double centroid_width = double.NaN;
                                    object spectrum_obj = null;
                                    object flags = null;
                                    int size = -1;
                                    raw.GetMassListFromScanNum(ref scan_number, null, 0, -1, 0, 1, ref centroid_width,
                                        ref spectrum_obj, ref flags, ref size);
                                    spectrum = (double[,]) spectrum_obj;
                                }

                                double base_peak_mz = double.NaN;
                                double base_peak_intensity = double.NaN;
                                for (int i = spectrum.GetLowerBound(1); i <= spectrum.GetUpperBound(1); i++)
                                {
                                    if (double.IsNaN(base_peak_mz) ||
                                        spectrum[(int) RawLabelDataColumn.Intensity, i] > base_peak_intensity)
                                    {
                                        base_peak_mz = spectrum[(int) RawLabelDataColumn.MZ, i];
                                        base_peak_intensity = spectrum[(int) RawLabelDataColumn.Intensity, i];
                                    }
                                }

                                double intensity_threshold = intensityThreshold;
                                if (intensityThresholdType == IntensityThresholdType.Relative)
                                {
                                    intensity_threshold = (intensityThreshold/100.0)*base_peak_intensity;
                                }

                                double[] parameters = new double[4];
                                if (!scan_filter.Contains("FTMS") &&
                                    intensityThresholdType == IntensityThresholdType.SignalToNoiseRatio)
                                {
                                    List<double> relative_intensities = new List<double>();
                                    for (int i = spectrum.GetLowerBound(1); i <= spectrum.GetUpperBound(1); i++)
                                    {
                                        relative_intensities.Add(spectrum[(int) RawLabelDataColumn.Intensity, i]/
                                                                 base_peak_intensity);
                                    }

                                    double bin_width = 0.001;
                                    int bins = 101;
                                    double[][] relative_intensity_histogram = new double[2][];
                                    relative_intensity_histogram[0] = new double[bins];
                                    relative_intensity_histogram[1] = new double[bins];
                                    for (int i = relative_intensity_histogram[0].GetLowerBound(0);
                                        i <= relative_intensity_histogram[0].GetUpperBound(0);
                                        i++)
                                    {
                                        relative_intensity_histogram[0][i] = i*bin_width;
                                    }
                                    foreach (double relative_intensity in relative_intensities)
                                    {
                                        int bin_number = (int) Math.Floor(relative_intensity/bin_width);
                                        if (bin_number < bins)
                                        {
                                            relative_intensity_histogram[1][bin_number]++;
                                        }
                                    }

                                    parameters[0] = 0.0;
                                    parameters[1] = 100.0;
                                    parameters[2] = 0.0;
                                    parameters[3] = 0.001;

                                    double[] weights = new double[relative_intensity_histogram[1].Length];
                                    for (int i = weights.GetLowerBound(0); i <= weights.GetUpperBound(0); i++)
                                    {
                                        weights[i] = 1.0;
                                    }
                                    LMA lma = new LMA(new GaussianFunctionWithPartials(), parameters,
                                        relative_intensity_histogram, weights, new DotNetMatrix.GeneralMatrix(4, 4),
                                        0.001, 5000);
                                    lma.Fit();
                                }

                                List<Peptide> peptides = GetAlternativePhosphoisoformPeptides(
                                    identified_phosphopeptide, fixedModifications);
                                List<PhosphopeptideStatistics> all_phosphopeptide_stats =
                                    new List<PhosphopeptideStatistics>(peptides.Count);
                                PhosphopeptideStatistics identified_phosphoisoform = null;
                                List<double> ms2_mz_peaks = new List<double>(spectrum.GetLength(1));
                                for (int i = spectrum.GetLowerBound(1); i <= spectrum.GetUpperBound(1); i++)
                                {
                                    double signal_to_noise = scan_filter.Contains("FTMS")
                                        ? (spectrum[(int) RawLabelDataColumn.Intensity, i] -
                                           spectrum[(int) RawLabelDataColumn.NoiseBaseline, i])/
                                          spectrum[(int) RawLabelDataColumn.NoiseLevel, i]
                                        : ((spectrum[(int) RawLabelDataColumn.Intensity, i]/base_peak_intensity) -
                                           parameters[2])/parameters[3];
                                    if ((intensityThresholdType == IntensityThresholdType.SignalToNoiseRatio
                                         && signal_to_noise >= intensity_threshold)
                                        || (intensityThresholdType != IntensityThresholdType.SignalToNoiseRatio
                                            && spectrum[(int) RawLabelDataColumn.Intensity, i] >= intensity_threshold))
                                    {
                                        ms2_mz_peaks.Add(spectrum[(int) RawLabelDataColumn.MZ, i]);
                                    }
                                }

                                double mz_range = high_mz - low_mz;

                                Dictionary<double, bool> searched_fragment_mzs = new Dictionary<double, bool>();
                                foreach (Peptide peptide in peptides)
                                {
                                    PhosphopeptideStatistics phosphopeptide_stats = new PhosphopeptideStatistics(peptide);
                                    if (peptide.Sequence == identified_phosphopeptide.Sequence)
                                    {
                                        identified_phosphoisoform = phosphopeptide_stats;
                                    }

                                    FragmentDictionary fragments = peptide.CalculateFragments(fragment_types);
                                    foreach (KeyValuePair<string, Fragment> kvp in fragments)
                                    {
                                        phosphopeptide_stats.Fragments.Add(kvp.Key, new Dictionary<int, bool>());

                                        for (int fragment_charge = 1;
                                            fragment_charge <= (charge >= 3 ? 2 : 1);
                                            fragment_charge++)
                                        {
                                            if (fragment_charge > 1 &&
                                                fragment_charge >
                                                (double) kvp.Value.Number/peptide.Sequence.Length*charge)
                                            {
                                                break;
                                            }

                                            double mz = MZFromMassAndCharge(kvp.Value.Mass, fragment_charge);

                                            if (mz < low_mz || mz > high_mz)
                                            {
                                                continue;
                                            }

                                            if (!searched_fragment_mzs.ContainsKey(mz))
                                            {
                                                bool found = false;

                                                foreach (double ms2_mz_peak in ms2_mz_peaks)
                                                {
                                                    if (Math.Abs(ms2_mz_peak - mz) <= mzTolerance)
                                                    {
                                                        found = true;
                                                        break;
                                                    }
                                                    else if (ms2_mz_peak > mz + mzTolerance)
                                                    {
                                                        break;
                                                    }
                                                }

                                                searched_fragment_mzs.Add(mz, found);
                                            }

                                            phosphopeptide_stats.Fragments[kvp.Key].Add(fragment_charge,
                                                searched_fragment_mzs[mz]);
                                        }
                                    }

                                    all_phosphopeptide_stats.Add(phosphopeptide_stats);
                                }

                                all_phosphopeptide_stats.Sort(ComparePhosphopeptidesByDescendingMatchingFragments);

                                PhosphopeptideStatistics best_phosphoisoform = all_phosphopeptide_stats[0];
                                PhosphopeptideStatistics second_best_phosphoisoform = all_phosphopeptide_stats.Count > 1
                                    ? all_phosphopeptide_stats[1]
                                    : null;

                                List<string> best_sequences = new List<string>();
                                foreach (PhosphopeptideStatistics phosphopeptide_stats in all_phosphopeptide_stats)
                                {
                                    if (phosphopeptide_stats.NumberOfMatchingFragments ==
                                        best_phosphoisoform.NumberOfMatchingFragments)
                                    {
                                        best_sequences.Add(phosphopeptide_stats.Peptide.Sequence);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                bool preliminary_localization = second_best_phosphoisoform == null
                                                                ||
                                                                best_phosphoisoform.NumberOfMatchingFragments >
                                                                second_best_phosphoisoform.NumberOfMatchingFragments;

                                bool all_sites_localized = preliminary_localization;

                                Dictionary<string, bool> peptide_sites = new Dictionary<string, bool>();
                                Dictionary<string, bool> protein_sites = new Dictionary<string, bool>();
                                string best_sequence = best_phosphoisoform.Peptide.Sequence;
                                for (int i = 0; i < best_sequence.Length; i++)
                                {
                                    if (char.IsLower(best_sequence[i]))
                                    {
                                        if (preliminary_localization && second_best_phosphoisoform == null)
                                        {
                                            peptide_sites.Add(best_sequence[i] + (i + 1).ToString(), true);
                                            protein_sites.Add(best_sequence[i] + (start_residue + i).ToString(), true);

                                            if (!localized_sites_by_protein.ContainsKey(protein_description))
                                            {
                                                localized_sites_by_protein.Add(protein_description,
                                                    new Dictionary<string, int>());
                                            }
                                            string site = best_sequence[i] + (start_residue + i).ToString();
                                            if (!localized_sites_by_protein[protein_description].ContainsKey(site))
                                            {
                                                localized_sites_by_protein[protein_description].Add(site, 0);
                                            }
                                            localized_sites_by_protein[protein_description][site]++;

                                            if (motifXOutput)
                                            {
                                                ExtractMotifs(motifs, proteins, protein_description, best_sequence,
                                                    start_residue, i);
                                            }
                                        }
                                        else
                                        {
                                            peptide_sites.Add(best_sequence[i] + (i + 1).ToString(), false);
                                            protein_sites.Add(best_sequence[i] + (start_residue + i).ToString(), false);
                                        }
                                    }
                                }

                                double probability_of_success = double.NaN;
                                List<string> theoretical_site_determining_fragment_ions = new List<string>();
                                List<string> matching_site_determining_fragment_ions = new List<string>();
                                List<string> left_site_determining_fragments = new List<string>();
                                List<string> right_site_determining_fragments = new List<string>();
                                List<string> site_determining_fragments = new List<string>();
                                List<string> p_values = new List<string>();
                                List<string> a_scores = new List<string>();
                                List<string> sites_localized = new List<string>();

                                if (preliminary_localization && second_best_phosphoisoform != null)
                                {
                                    probability_of_success = (ms2_mz_peaks.Count*2*mzTolerance)/mz_range;

                                    for (int i = 0; i < best_sequence.Length; i++)
                                    {
                                        if (char.IsLower(best_sequence[i]))
                                        {
                                            int first_phosphorylatable_residue = i - 1;
                                            while (first_phosphorylatable_residue >= 0)
                                            {
                                                if (best_sequence[first_phosphorylatable_residue] == 'S'
                                                    || best_sequence[first_phosphorylatable_residue] == 'T'
                                                    || best_sequence[first_phosphorylatable_residue] == 'Y')
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    first_phosphorylatable_residue--;
                                                }
                                            }

                                            int? num_left_theoretical_site_determining_fragment_ions = null;
                                            int? num_left_matching_site_determining_fragment_ions = null;
                                            double left_p_value = double.NaN;
                                            double left_a_score = double.NaN;
                                            if (first_phosphorylatable_residue >= 0)
                                            {
                                                num_left_theoretical_site_determining_fragment_ions = 0;
                                                num_left_matching_site_determining_fragment_ions = 0;

                                                for (int j = first_phosphorylatable_residue + 1; j <= i; j++)
                                                {
                                                    string n_terminal_fragment = fragment_types[0].ToString() +
                                                                                 j.ToString();
                                                    if (best_phosphoisoform.Fragments.ContainsKey(n_terminal_fragment))
                                                    {
                                                        foreach (
                                                            KeyValuePair<int, bool> kvp in
                                                                best_phosphoisoform.Fragments[n_terminal_fragment])
                                                        {
                                                            num_left_theoretical_site_determining_fragment_ions++;
                                                            if (kvp.Value)
                                                            {
                                                                num_left_matching_site_determining_fragment_ions++;
                                                                string n_terminal_fragment_string =
                                                                    n_terminal_fragment + "(+" + kvp.Key.ToString() +
                                                                    ')';
                                                                if (
                                                                    !left_site_determining_fragments.Contains(
                                                                        n_terminal_fragment_string))
                                                                {
                                                                    left_site_determining_fragments.Add(
                                                                        n_terminal_fragment_string);
                                                                }
                                                            }
                                                        }
                                                    }

                                                    string c_terminal_fragment = fragment_types[1].ToString() +
                                                                                 (best_sequence.Length - j).ToString();
                                                    if (best_phosphoisoform.Fragments.ContainsKey(c_terminal_fragment))
                                                    {
                                                        foreach (
                                                            KeyValuePair<int, bool> kvp in
                                                                best_phosphoisoform.Fragments[c_terminal_fragment])
                                                        {
                                                            num_left_theoretical_site_determining_fragment_ions++;
                                                            if (kvp.Value)
                                                            {
                                                                num_left_matching_site_determining_fragment_ions++;
                                                                string c_terminal_fragment_string =
                                                                    c_terminal_fragment + "(+" + kvp.Key.ToString() +
                                                                    ')';
                                                                if (
                                                                    !left_site_determining_fragments.Contains(
                                                                        c_terminal_fragment_string))
                                                                {
                                                                    left_site_determining_fragments.Add(
                                                                        c_terminal_fragment_string);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                left_p_value =
                                                    alglib.binomialdistr.binomialcdistribution(
                                                        num_left_matching_site_determining_fragment_ions.Value - 1,
                                                        num_left_theoretical_site_determining_fragment_ions.Value,
                                                        probability_of_success);
                                                left_a_score = -10*Math.Log10(left_p_value);
                                            }

                                            int last_phosphorylatable_residue = i + 1;
                                            while (last_phosphorylatable_residue < best_sequence.Length)
                                            {
                                                if (best_sequence[last_phosphorylatable_residue] == 'S'
                                                    || best_sequence[last_phosphorylatable_residue] == 'T'
                                                    || best_sequence[last_phosphorylatable_residue] == 'Y')
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    last_phosphorylatable_residue++;
                                                }
                                            }

                                            int? num_right_theoretical_site_determining_fragment_ions = null;
                                            int? num_right_matching_site_determining_fragment_ions = null;
                                            double right_p_value = double.NaN;
                                            double right_a_score = double.NaN;
                                            if (last_phosphorylatable_residue < best_sequence.Length)
                                            {
                                                num_right_theoretical_site_determining_fragment_ions = 0;
                                                num_right_matching_site_determining_fragment_ions = 0;

                                                for (int j = last_phosphorylatable_residue; j > i; j--)
                                                {
                                                    string n_terminal_fragment = fragment_types[0].ToString() +
                                                                                 j.ToString();
                                                    if (best_phosphoisoform.Fragments.ContainsKey(n_terminal_fragment))
                                                    {
                                                        foreach (
                                                            KeyValuePair<int, bool> kvp in
                                                                best_phosphoisoform.Fragments[n_terminal_fragment])
                                                        {
                                                            num_right_theoretical_site_determining_fragment_ions++;
                                                            if (kvp.Value)
                                                            {
                                                                num_right_matching_site_determining_fragment_ions++;
                                                                string n_terminal_fragment_string =
                                                                    n_terminal_fragment + "(+" + kvp.Key.ToString() +
                                                                    ')';
                                                                if (
                                                                    !right_site_determining_fragments.Contains(
                                                                        n_terminal_fragment_string))
                                                                {
                                                                    right_site_determining_fragments.Add(
                                                                        n_terminal_fragment_string);
                                                                }
                                                            }
                                                        }
                                                    }

                                                    string c_terminal_fragment = fragment_types[1].ToString() +
                                                                                 (best_sequence.Length - j).ToString();
                                                    if (best_phosphoisoform.Fragments.ContainsKey(c_terminal_fragment))
                                                    {
                                                        foreach (
                                                            KeyValuePair<int, bool> kvp in
                                                                best_phosphoisoform.Fragments[c_terminal_fragment])
                                                        {
                                                            num_right_theoretical_site_determining_fragment_ions++;
                                                            if (kvp.Value)
                                                            {
                                                                num_right_matching_site_determining_fragment_ions++;
                                                                string c_terminal_fragment_string =
                                                                    c_terminal_fragment + "(+" + kvp.Key.ToString() +
                                                                    ')';
                                                                if (
                                                                    !right_site_determining_fragments.Contains(
                                                                        c_terminal_fragment_string))
                                                                {
                                                                    right_site_determining_fragments.Add(
                                                                        c_terminal_fragment_string);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                right_p_value =
                                                    alglib.binomialdistr.binomialcdistribution(
                                                        num_right_matching_site_determining_fragment_ions.Value - 1,
                                                        num_right_theoretical_site_determining_fragment_ions.Value,
                                                        probability_of_success);
                                                right_a_score = -10*Math.Log10(right_p_value);
                                            }

                                            theoretical_site_determining_fragment_ions.Add(
                                                (num_left_theoretical_site_determining_fragment_ions.HasValue
                                                    ? num_left_theoretical_site_determining_fragment_ions.ToString()
                                                    : "n/a") + " | " +
                                                (num_right_theoretical_site_determining_fragment_ions.HasValue
                                                    ? num_right_theoretical_site_determining_fragment_ions.ToString()
                                                    : "n/a"));
                                            matching_site_determining_fragment_ions.Add(
                                                (num_left_matching_site_determining_fragment_ions.HasValue
                                                    ? num_left_matching_site_determining_fragment_ions.ToString()
                                                    : "n/a") + " | " +
                                                (num_right_matching_site_determining_fragment_ions.HasValue
                                                    ? num_right_matching_site_determining_fragment_ions.ToString()
                                                    : "n/a"));
                                            site_determining_fragments.Add((left_site_determining_fragments.Count > 0
                                                ? string.Join(",", left_site_determining_fragments.ToArray())
                                                : "n/a") + " | " +
                                                                           (right_site_determining_fragments.Count > 0
                                                                               ? string.Join(",",
                                                                                   right_site_determining_fragments
                                                                                       .ToArray())
                                                                               : "n/a"));
                                            p_values.Add((double.IsNaN(left_p_value) ? "n/a" : left_p_value.ToString()) +
                                                         " | " +
                                                         (double.IsNaN(right_p_value) ? "n/a" : right_p_value.ToString()));
                                            a_scores.Add((double.IsNaN(left_a_score) ? "n/a" : left_a_score.ToString()) +
                                                         " | " +
                                                         (double.IsNaN(right_a_score) ? "n/a" : right_a_score.ToString()));
                                            bool site_localized = (double.IsNaN(left_a_score) ||
                                                                   left_a_score >= ambiguityScoreThreshold) &&
                                                                  (double.IsNaN(right_a_score) ||
                                                                   right_a_score >= ambiguityScoreThreshold);
                                            sites_localized.Add(site_localized.ToString().ToUpper());
                                            if (site_localized)
                                            {
                                                peptide_sites[best_sequence[i] + (i + 1).ToString()] = true;
                                                protein_sites[best_sequence[i] + (start_residue + i).ToString()] = true;

                                                if (!localized_sites_by_protein.ContainsKey(protein_description))
                                                {
                                                    localized_sites_by_protein.Add(protein_description,
                                                        new Dictionary<string, int>());
                                                }
                                                string site = best_sequence[i] + (start_residue + i).ToString();
                                                if (!localized_sites_by_protein[protein_description].ContainsKey(site))
                                                {
                                                    localized_sites_by_protein[protein_description].Add(site, 0);
                                                }
                                                localized_sites_by_protein[protein_description][site]++;

                                                if (motifXOutput)
                                                {
                                                    ExtractMotifs(motifs, proteins, protein_description, best_sequence,
                                                        start_residue, i);
                                                }
                                            }
                                            if (!site_localized)
                                            {
                                                all_sites_localized = false;
                                            }
                                        }
                                    }
                                }

                                int phosphorylations = 0;
                                foreach (
                                    string dynamic_modification in
                                        best_phosphoisoform.Peptide.DynamicModifications.Values)
                                {
                                    if (dynamic_modification.Contains("phosphorylation"))
                                    {
                                        phosphorylations++;
                                    }
                                }
                                string isoform = null;
                                if (all_sites_localized)
                                {
                                    foreach (KeyValuePair<string, bool> kvp in protein_sites)
                                    {
                                        isoform += kvp.Key + ',';
                                    }
                                    isoform = isoform.Substring(0, isoform.Length - 1);
                                    KeyValuePair<int, string> isoform_kvp =
                                        new KeyValuePair<int, string>(phosphorylations, isoform);

                                    if (!localized.ContainsKey(protein_description))
                                    {
                                        localized.Add(protein_description,
                                            new Dictionary<KeyValuePair<int, string>, List<string>>());
                                    }
                                    if (!localized[protein_description].ContainsKey(isoform_kvp))
                                    {
                                        localized[protein_description].Add(isoform_kvp, new List<string>());
                                    }
                                    localized[protein_description][isoform_kvp].Add(line);
                                }
                                else
                                {
                                    if (preliminary_localization)
                                    {
                                        foreach (KeyValuePair<string, bool> kvp in protein_sites)
                                        {
                                            isoform += kvp.Key;
                                            if (!kvp.Value)
                                            {
                                                isoform += '?';
                                            }
                                            isoform += ',';
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < best_sequence.Length; i++)
                                        {
                                            bool phospho = false;
                                            for (int j = 0; j < all_phosphopeptide_stats.Count; j++)
                                            {
                                                if (all_phosphopeptide_stats[j].NumberOfMatchingFragments <
                                                    best_phosphoisoform.NumberOfMatchingFragments)
                                                {
                                                    break;
                                                }

                                                if (char.IsLower(all_phosphopeptide_stats[j].Peptide.Sequence[i]))
                                                {
                                                    phospho = true;
                                                }
                                            }
                                            if (phospho)
                                            {
                                                isoform += char.ToLower(best_sequence[i]) +
                                                           (start_residue + i).ToString() + "?,";
                                            }
                                        }
                                    }
                                    isoform = isoform.Substring(0, isoform.Length - 1);
                                    KeyValuePair<int, string> isoform_kvp =
                                        new KeyValuePair<int, string>(phosphorylations, isoform);

                                    if (!unlocalized.ContainsKey(protein_description))
                                    {
                                        unlocalized.Add(protein_description,
                                            new Dictionary<KeyValuePair<int, string>, List<string>>());
                                    }
                                    if (!unlocalized[protein_description].ContainsKey(isoform_kvp))
                                    {
                                        unlocalized[protein_description].Add(isoform_kvp, new List<string>());
                                    }
                                    unlocalized[protein_description][isoform_kvp].Add(line);
                                }

                                StreamWriter output = all_sites_localized
                                    ? localized_phospho_output
                                    : unlocalized_phospho_output;

                                output.Write(line + ',');

                                output.Write(identified_phosphoisoform.NumberOfTotalFragments.ToString() + ',');

                                output.Write(identified_phosphoisoform.Peptide.Sequence + ',');
                                output.Write(identified_phosphoisoform.NumberOfMatchingFragments.ToString() + ',');

                                for (int s = 0; s < best_sequences.Count; s++)
                                {
                                    output.Write(best_sequences[s]);
                                    if (s < best_sequences.Count - 1)
                                    {
                                        output.Write('/');
                                    }
                                }
                                output.Write(',');

                                output.Write(best_phosphoisoform.Peptide.Sequence + ',');
                                output.Write(best_phosphoisoform.NumberOfMatchingFragments.ToString() + ',');

                                if (second_best_phosphoisoform != null)
                                {
                                    output.Write(second_best_phosphoisoform.Peptide.Sequence + ',');
                                    output.Write(second_best_phosphoisoform.NumberOfMatchingFragments.ToString() + ',');
                                }
                                else
                                {
                                    output.Write("n/a,n/a,");
                                }

                                output.Write(
                                    (identified_phosphoisoform.NumberOfMatchingFragments ==
                                     best_phosphoisoform.NumberOfMatchingFragments).ToString() + ',');

                                output.Write(preliminary_localization.ToString() + ',');

                                string[] peptide_sites_array = new string[peptide_sites.Count];
                                peptide_sites.Keys.CopyTo(peptide_sites_array, 0);
                                string peptide_sites_array_string = string.Join("; ", peptide_sites_array);
                                AppendFieldToCsv(peptide_sites_array_string, output);
                                output.Write((!double.IsNaN(probability_of_success)
                                    ? probability_of_success.ToString()
                                    : string.Empty) + ',');
                                string theoretical_site_determining_fragment_ions_string = string.Join("; ",
                                    theoretical_site_determining_fragment_ions.ToArray());
                                AppendFieldToCsv(theoretical_site_determining_fragment_ions_string, output);
                                string matching_site_determining_fragment_ions_string = string.Join("; ",
                                    matching_site_determining_fragment_ions.ToArray());
                                AppendFieldToCsv(matching_site_determining_fragment_ions_string, output);
                                string site_determining_fragments_string = string.Join("; ",
                                    site_determining_fragments.ToArray());
                                AppendFieldToCsv(site_determining_fragments_string, output);
                                string p_values_string = string.Join("; ", p_values.ToArray());
                                AppendFieldToCsv(p_values_string, output);
                                string a_scores_string = string.Join("; ", a_scores.ToArray());
                                AppendFieldToCsv(a_scores_string, output);
                                string sites_localized_string = string.Join("; ", sites_localized.ToArray());
                                AppendFieldToCsv(sites_localized_string, output);

                                output.Write(all_sites_localized.ToString().ToUpper());

                                output.WriteLine();
                            }

                            double progress = (double) csv.BaseStream.Position/csv.BaseStream.Length;
                            onUpdateProgress(new ProgressEventArgs((int) Math.Round(progress*100.0)));
                        }

                    }
                    csv.Close();

                    onFinishedFile(new EventArgs());
                }

                raw.Close();

                non_phospho_output.Close();
                localized_phospho_output.Close();
                unlocalized_phospho_output.Close();

                log.WriteLine("Identified Phosphoproteins: " + identified_sites_by_protein.Proteins.ToString());
                log.WriteLine("Identified Phosphosites: " + identified_sites_by_protein.Sites.ToString());

                log.WriteLine();

                log.WriteLine("Localized Phosphoproteins: " + localized_sites_by_protein.Proteins.ToString());
                log.WriteLine("Localized Phosphosites: " + localized_sites_by_protein.Sites.ToString());

                log.WriteLine();

                int localized_phosphoisoforms = 0;
                foreach (KeyValuePair<string, Dictionary<KeyValuePair<int, string>, List<string>>> kvp in localized)
                {
                    foreach (KeyValuePair<KeyValuePair<int, string>, List<string>> kvp2 in kvp.Value)
                    {
                        localized_phosphoisoforms++;
                    }
                }

                int unlocalized_phosphoisoforms = 0;
                foreach (KeyValuePair<string, Dictionary<KeyValuePair<int, string>, List<string>>> kvp in unlocalized)
                {
                    foreach (KeyValuePair<KeyValuePair<int, string>, List<string>> kvp2 in kvp.Value)
                    {
                        unlocalized_phosphoisoforms++;
                    }
                }

                log.WriteLine("Localized Phosphoisoforms: " + localized_phosphoisoforms.ToString());
                log.WriteLine("Unlocalized Phosphoisoforms: " + unlocalized_phosphoisoforms.ToString());

                log.Close();

                using (StreamWriter protein_sites = new StreamWriter(Path.Combine(outputFolder, "localized_protein_phosphosites.csv")))
                {
                    protein_sites.WriteLine("Protein Description, Number of Localized Phosphosites");
                    protein_sites.WriteLine(", Localized Phosphosite");

                    foreach (KeyValuePair<string, Dictionary<string, int>> kvp in localized_sites_by_protein)
                    {
                        protein_sites.WriteLine((kvp.Key.Contains(",") ? '"' + kvp.Key + '"' : kvp.Key) + ',' + kvp.Value.Count.ToString());
                        foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                        {
                            protein_sites.WriteLine(',' + kvp2.Key);
                        }
                    }
                }

                using (StreamWriter full_localized_output = new StreamWriter(Path.Combine(outputFolder, "full_localized_phosphoisoforms.csv")))
                {
                    //int interference_index = -1;
                    int first_quant_index = -1;
                    int last_quant_index = -1;
                    if (!quant)
                    {
                        full_localized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites, PSMs Identified, Peptides Identified");
                    }
                    else
                    {
                        full_localized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites, PSMs Identified, PSMs Quantified, Peptides Identified, Peptides Quantified,");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (headers[i].EndsWith("NL)"))
                            {
                                if (first_quant_index < 0)
                                {
                                    first_quant_index = i;
                                }
                            }
                            if (first_quant_index >= 0)
                                full_localized_output.Write(' ' + headers[i] + ',');
                            if (headers[i].Equals("Channels Detected"))
                            {
                                last_quant_index = i;
                            }
                        }
                        full_localized_output.Write(" Phosphoisoform Quantified?");
                    }
                    full_localized_output.WriteLine();
                    full_localized_output.WriteLine(", " + header_line);

                    using (StreamWriter reduced_localized_output = new StreamWriter(Path.Combine(outputFolder, "reduced_localized_phosphoisoforms.csv")))
                    {
                        if (!quant)
                        {
                            reduced_localized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides , PSMs Identified, Peptides Identified");
                        }
                        else
                        {
                            reduced_localized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides , PSMs Identified, PSMs Quantified, Peptides Identified, Peptides Quantified,");
                            for (int i = first_quant_index; i <= last_quant_index; i++)
                            {
                                reduced_localized_output.Write(' ' + headers[i] + ',');
                            }
                            reduced_localized_output.Write(" Phosphoisoform Quantified?");
                        }
                        reduced_localized_output.WriteLine();

                        foreach (KeyValuePair<string, Dictionary<KeyValuePair<int, string>, List<string>>> kvp in localized)
                        {
                            foreach (KeyValuePair<KeyValuePair<int, string>, List<string>> kvp2 in kvp.Value)
                            {
                                full_localized_output.Write((kvp.Key.Contains(",") ? '"' + kvp.Key + '"' : kvp.Key) + ',');
                                reduced_localized_output.Write((kvp.Key.Contains(",") ? '"' + kvp.Key + '"' : kvp.Key) + ',');

                                full_localized_output.Write((kvp2.Key.Value.Contains(",") ? '"' + kvp2.Key.Value + '"' : kvp2.Key.Value) + ',');
                                reduced_localized_output.Write((kvp2.Key.Value.Contains(",") ? '"' + kvp2.Key.Value + '"' : kvp2.Key.Value) + ',');

                                full_localized_output.Write(kvp2.Key.Key.ToString() + ',');
                                reduced_localized_output.Write(kvp2.Key.Key.ToString() + ',');

                                double[] isoform_quantitation = new double[last_quant_index - first_quant_index + 1];
                                isoform_quantitation = Array.ConvertAll<double, double>(isoform_quantitation, SET_DOUBLE_VALUE_TO_NAN);
                                int spectra_identified = 0;
                                int spectra_quantified = 0;
                                Dictionary<string, int> unique_peptides_identified = new Dictionary<string, int>();
                                Dictionary<string, int> unique_peptides_quantified = new Dictionary<string, int>();
                                StringBuilder peptides = new StringBuilder();
                                foreach (string line in kvp2.Value)
                                {
                                    string[] fields = Regex.Split(line, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))"); // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621
                                    spectra_identified++;
                                    string peptide_sequence = fields[2];
                                    peptides.Append(peptide_sequence + " ");
                                    if (!unique_peptides_identified.ContainsKey(peptide_sequence))
                                    {
                                        unique_peptides_identified.Add(peptide_sequence, 0);
                                    }
                                    unique_peptides_identified[peptide_sequence]++;
                                    if (quant)
                                    {

                                        spectra_quantified++;
                                        if (double.IsNaN(isoform_quantitation[0]))
                                        {
                                            isoform_quantitation = Array.ConvertAll<double, double>(isoform_quantitation, SET_DOUBLE_VALUE_TO_ZERO);
                                        }
                                        if (!unique_peptides_quantified.ContainsKey(peptide_sequence))
                                        {
                                            unique_peptides_quantified.Add(peptide_sequence, 0);
                                        }
                                        unique_peptides_quantified[peptide_sequence]++;
                                        for (int i = first_quant_index; i <= last_quant_index; i++)
                                        {
                                            double val = 0;
                                            double.TryParse(fields[i], out val);
                                            isoform_quantitation[i - first_quant_index] += val;
                                        }

                                    }
                                }

                                full_localized_output.Write(peptides.ToString() + ',');
                                reduced_localized_output.Write(peptides.ToString() + ',');

                                full_localized_output.Write(spectra_identified.ToString() + ',');
                                reduced_localized_output.Write(spectra_identified.ToString() + ',');
                                if (quant)
                                {
                                    full_localized_output.Write(spectra_quantified.ToString() + ',');
                                    reduced_localized_output.Write(spectra_quantified.ToString() + ',');
                                }
                                full_localized_output.Write(unique_peptides_identified.Count.ToString() + ',');
                                reduced_localized_output.Write(unique_peptides_identified.Count.ToString() + ',');
                                if (quant)
                                {
                                    full_localized_output.Write(unique_peptides_quantified.Count.ToString() + ',');
                                    reduced_localized_output.Write(unique_peptides_quantified.Count.ToString() + ',');
                                    for (int i = isoform_quantitation.GetLowerBound(0); i <= isoform_quantitation.GetUpperBound(0); i++)
                                    {
                                        full_localized_output.Write(isoform_quantitation[i].ToString() + ',');
                                        reduced_localized_output.Write(isoform_quantitation[i].ToString() + ',');
                                    }
                                    full_localized_output.Write((spectra_quantified > 0).ToString());
                                    reduced_localized_output.Write((spectra_quantified > 0).ToString());
                                }
                                full_localized_output.WriteLine();
                                reduced_localized_output.WriteLine();

                                foreach (string line in kvp2.Value)
                                {
                                    full_localized_output.WriteLine(',' + line);
                                }
                            }
                        }
                    }
                }

                using (StreamWriter full_unlocalized_output = new StreamWriter(Path.Combine(outputFolder, "full_unlocalized_phosphoisoforms.csv")))
                {
                    int first_quant_index = -1;
                    int last_quant_index = -1;
                    if (!quant)
                    {
                        full_unlocalized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides, PSMs Identified, Peptides Identified");
                    }
                    else
                    {
                        full_unlocalized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides, PSMs Identified, PSMs Quantified, Peptides Identified, Peptides Quantified,");
                        for (int i = 0; i < headers.Length; i++)
                        {
                            if (headers[i].EndsWith("NL)"))
                            {
                                if (first_quant_index < 0)
                                {
                                    first_quant_index = i;
                                }
                            }
                            if (first_quant_index >= 0)
                                full_unlocalized_output.Write(' ' + headers[i] + ',');
                            if (headers[i].Equals("Channels Detected"))
                            {
                                last_quant_index = i;
                            }
                        }
                        full_unlocalized_output.Write(" Phosphoisoform Quantified?");
                    }
                    full_unlocalized_output.WriteLine();
                    full_unlocalized_output.WriteLine(", " + header_line);

                    using (StreamWriter reduced_unlocalized_output = new StreamWriter(Path.Combine(outputFolder, "reduced_unlocalized_phosphoisoforms.csv")))
                    {
                        if (!quant)
                        {
                            reduced_unlocalized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides, PSMs Identified, Peptides Identified");
                        }
                        else
                        {
                            reduced_unlocalized_output.Write("Protein Description, Phosphoisoform, Phosphoisoform Sites,Peptides, PSMs Identified, PSMs Quantified, Peptides Identified, Peptides Quantified,");
                            for (int i = first_quant_index; i <= last_quant_index; i++)
                            {
                                reduced_unlocalized_output.Write(' ' + headers[i] + ',');
                            }
                            reduced_unlocalized_output.Write(" Phosphoisoform Quantified?");
                        }
                        reduced_unlocalized_output.WriteLine();



                        foreach (KeyValuePair<string, Dictionary<KeyValuePair<int, string>, List<string>>> kvp in unlocalized)
                        {
                            foreach (KeyValuePair<KeyValuePair<int, string>, List<string>> kvp2 in kvp.Value)
                            {
                                full_unlocalized_output.Write((kvp.Key.Contains(",") ? '"' + kvp.Key + '"' : kvp.Key) + ',');
                                reduced_unlocalized_output.Write((kvp.Key.Contains(",") ? '"' + kvp.Key + '"' : kvp.Key) + ',');

                                full_unlocalized_output.Write((kvp2.Key.Value.Contains(",") ? '"' + kvp2.Key.Value + '"' : kvp2.Key.Value) + ',');
                                reduced_unlocalized_output.Write((kvp2.Key.Value.Contains(",") ? '"' + kvp2.Key.Value + '"' : kvp2.Key.Value) + ',');

                                full_unlocalized_output.Write(kvp2.Key.Key.ToString() + ',');
                                reduced_unlocalized_output.Write(kvp2.Key.Key.ToString() + ',');

                                double[] isoform_quantitation = new double[last_quant_index - first_quant_index + 1];
                                isoform_quantitation = Array.ConvertAll<double, double>(isoform_quantitation, SET_DOUBLE_VALUE_TO_NAN);
                                int spectra_identified = 0;
                                int spectra_quantified = 0;
                                Dictionary<string, int> unique_peptides_identified = new Dictionary<string, int>();
                                Dictionary<string, int> unique_peptides_quantified = new Dictionary<string, int>();
                                StringBuilder peptides = new StringBuilder();
                                foreach (string line in kvp2.Value)
                                {
                                    string[] fields = Regex.Split(line, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))"); // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621
                                    spectra_identified++;
                                    string peptide_sequence = fields[2];
                                    peptides.Append(peptide_sequence + " ");
                                    if (!unique_peptides_identified.ContainsKey(peptide_sequence))
                                    {
                                        unique_peptides_identified.Add(peptide_sequence, 0);
                                    }
                                    unique_peptides_identified[peptide_sequence]++;
                                    if (quant)
                                    {
                                        spectra_quantified++;
                                        if (double.IsNaN(isoform_quantitation[0]))
                                        {
                                            isoform_quantitation = Array.ConvertAll<double, double>(isoform_quantitation, SET_DOUBLE_VALUE_TO_ZERO);
                                        }
                                        if (!unique_peptides_quantified.ContainsKey(peptide_sequence))
                                        {
                                            unique_peptides_quantified.Add(peptide_sequence, 0);
                                        }
                                        unique_peptides_quantified[peptide_sequence]++;
                                        for (int i = first_quant_index; i <= last_quant_index; i++)
                                        {
                                            double val = 0;
                                            double.TryParse(fields[i], out val);
                                            isoform_quantitation[i - first_quant_index] += val;
                                        }

                                    }
                                }

                                full_unlocalized_output.Write(peptides.ToString() + ',');
                                reduced_unlocalized_output.Write(peptides.ToString() + ',');

                                full_unlocalized_output.Write(spectra_identified.ToString() + ',');
                                reduced_unlocalized_output.Write(spectra_identified.ToString() + ',');
                                if (quant)
                                {
                                    full_unlocalized_output.Write(spectra_quantified.ToString() + ',');
                                    reduced_unlocalized_output.Write(spectra_quantified.ToString() + ',');
                                }
                                full_unlocalized_output.Write(unique_peptides_identified.Count.ToString() + ',');
                                reduced_unlocalized_output.Write(unique_peptides_identified.Count.ToString() + ',');
                                if (quant)
                                {
                                    full_unlocalized_output.Write(unique_peptides_quantified.Count.ToString() + ',');
                                    reduced_unlocalized_output.Write(unique_peptides_quantified.Count.ToString() + ',');
                                    for (int i = isoform_quantitation.GetLowerBound(0); i <= isoform_quantitation.GetUpperBound(0); i++)
                                    {
                                        full_unlocalized_output.Write(isoform_quantitation[i].ToString() + ',');
                                        reduced_unlocalized_output.Write(isoform_quantitation[i].ToString() + ',');
                                    }
                                    full_unlocalized_output.Write((spectra_quantified > 0).ToString());
                                    reduced_unlocalized_output.Write((spectra_quantified > 0).ToString());
                                }
                                full_unlocalized_output.WriteLine();
                                reduced_unlocalized_output.WriteLine();

                                foreach (string line in kvp2.Value)
                                {
                                    full_unlocalized_output.WriteLine(',' + line);
                                }
                            }
                        }
                    }
                }

                if (motifXOutput)
                {
                    foreach (string motif in motifs.Keys)
                    {
                        motifX.WriteLine(motif);
                    }
                    motifX.Close();

                    using (StreamWriter motif_fasta = new StreamWriter(Path.Combine(outputFolder, "motif-x.fasta")))
                    {
                        foreach (KeyValuePair<string, string> kvp in proteins)
                        {
                            if (!kvp.Key.Contains("DECOY") && !kvp.Key.Contains("REVERSED"))
                            {
                                motif_fasta.WriteLine('>' + kvp.Key);
                                motif_fasta.WriteLine(kvp.Value);
                            }
                        }
                    }
                }

                onFinished(new EventArgs());
                //}
                //catch(Exception ex)
                //{
                //    onThrowException(new ExceptionEventArgs(ex));
                //}
                //finally
                //{
                if (log != null)
                {
                    log.Close();
                }
                if (raw != null)
                {
                    raw.Close();
                }
                if (csv != null)
                {
                    csv.Close();
                }
                if (non_phospho_output != null)
                {
                    non_phospho_output.Close();
                }
                if (localized_phospho_output != null)
                {
                    localized_phospho_output.Close();
                }
                if (unlocalized_phospho_output != null)
                {
                    unlocalized_phospho_output.Close();
                }
                if (motifX != null)
                {
                    motifX.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static List<Peptide> GetAlternativePhosphoisoformPeptides(Peptide peptide, IEnumerable<Modification> fixedModifications)
        {
            int phosphorylations = 0;
            foreach(string dynamic_modification in peptide.DynamicModifications.Values)
            {
                if(dynamic_modification.Contains("phosphorylation"))
                {
                    phosphorylations++;
                }
            }

            List<int> modification_sites = new List<int>();
            for(int c = 0; c < peptide.Sequence.Length; c++)
            {
                if(char.ToUpper(peptide.Sequence[c]) == 'S' || char.ToUpper(peptide.Sequence[c]) == 'T' || char.ToUpper(peptide.Sequence[c]) == 'Y')
                {
                    modification_sites.Add(c);
                }
            }

            List<Peptide> peptides = new List<Peptide>();

            if(modification_sites.Count == 0)
            {
                peptides.Add(peptide);
                return peptides;
            }

            string base_sequence = peptide.Sequence.ToUpper();
            long forms = (long)Math.Pow(2, modification_sites.Count);
            for(long f = forms - 1; f >= 0; f--)
            {
                bool skip_form = false;
                string form_binary = Convert.ToString(f, 2);
                form_binary = form_binary.PadLeft(modification_sites.Count, '0');
                if(Count(form_binary, '1') == phosphorylations)
                {
                    Dictionary<int, string> dynamic_modifications = new Dictionary<int, string>();
                    foreach(KeyValuePair<int, string> kvp in peptide.DynamicModifications)
                    {
                        if(!kvp.Value.Contains("phosphorylation"))
                        {
                            dynamic_modifications.Add(kvp.Key, kvp.Value);
                        }
                    }
                    for(int m = 0; m < form_binary.Length; m++)
                    {
                        if(form_binary[m] == '1')
                        {
                            if(dynamic_modifications.ContainsKey(modification_sites[m] + 1))
                            {
                                skip_form = true;
                                break;
                            }

                            dynamic_modifications.Add(modification_sites[m] + 1, "phosphorylation of " + base_sequence[modification_sites[m]] + ':' + (modification_sites[m] + 1).ToString());
                        }
                    }
                    if(skip_form)
                    {
                        continue;
                    }

                    string[] dynamic_modifications_array = new string[dynamic_modifications.Count];
                    dynamic_modifications.Values.CopyTo(dynamic_modifications_array, 0);
                    Peptide new_peptide = new Peptide(base_sequence, fixedModifications, string.Join(" ,", dynamic_modifications_array));
                    peptides.Add(new_peptide);
                }
            }

            return peptides;
        }

        private static int Count(string s, char c)
        {
            int count = 0;

            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] == c)
                {
                    count++;
                }
            }

            return count;
        }

        private static double MZFromMassAndCharge(double mass, int charge)
        {
            return (mass + charge * PROTON_MASS) / Math.Abs(charge);
        }

        private static double MassFromMZAndCharge(double mz, int charge)
        {
            return mz * Math.Abs(charge) - charge * PROTON_MASS;
        }

        private static int ComparePhosphopeptidesByDescendingMatchingFragments(PhosphopeptideStatistics left, PhosphopeptideStatistics right)
        {
            return -(left.NumberOfMatchingFragments.CompareTo(right.NumberOfMatchingFragments));
        }

        private void AppendFieldToCsv(string field, StreamWriter csv)
        {
            if(field.Length > 0)
            {
                if(field.Contains(","))
                {
                    if(field.Contains("/"))
                    {
                        csv.Write("\" " + field + "\",");
                    }
                    else
                    {
                        csv.Write('"' + field + "\",");
                    }
                }
                else
                {
                    csv.Write(field + ',');
                }
            }
            else
            {
                csv.Write(',');
            }
        }

        private void ExtractMotifs(Dictionary<string, int> motifs, ProteinDictionary proteins, string proteinDescription, string peptideSequence, int startResidueNumber, int phosphositeIndex)
        {
            string protein_sequence = proteins[proteinDescription];
            int half_window_size = (motifXWindowSize - 1) / 2;
            int absolute_start_residue = (startResidueNumber - 1) + phosphositeIndex;
            int first_half_start_residue = absolute_start_residue - half_window_size;
            if(first_half_start_residue < 0)
            {
                first_half_start_residue = 0;
            }
            int first_half_stop_residue = absolute_start_residue - 1;
            if(first_half_stop_residue < 0)
            {
                first_half_stop_residue = 0;
            }
            string phosphosequence = null;
            phosphosequence = protein_sequence.Substring(first_half_start_residue, first_half_stop_residue - first_half_start_residue + 1);
            phosphosequence += peptideSequence[phosphositeIndex];
            int last_half_start_residue = absolute_start_residue + 1;
            if(last_half_start_residue > protein_sequence.Length - 1)
            {
                last_half_start_residue = protein_sequence.Length - 1;
            }
            int last_half_stop_residue = absolute_start_residue + 1 + half_window_size - 1;
            if(last_half_stop_residue > protein_sequence.Length - 1)
            {
                last_half_stop_residue = protein_sequence.Length - 1;
            }
            phosphosequence += protein_sequence.Substring(last_half_start_residue, last_half_stop_residue - last_half_start_residue + 1);
            if(!motifs.ContainsKey(phosphosequence))
            {
                motifs.Add(phosphosequence, 0);
            }
            motifs[phosphosequence]++;
        }
    }

    public class GaussianFunction : LMAFunction
    {
        public override double GetY(double x, double[] a)
        {
            return a[0] + a[1] * Math.Exp(-0.5 * Math.Pow(x - a[2], 2) / Math.Pow(a[3], 2));
        }
    }

    public class GaussianFunctionWithPartials : GaussianFunction, ILMAFunction
    {
        public new double GetY(double x, double[] a)
        {
            return base.GetY(x, a);
        }

        public new double GetPartialDerivative(double x, double[] a, int parameterIndex)
        {
            switch(parameterIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return Math.Exp(-0.5 * Math.Pow(x - a[2], 2) / Math.Pow(a[3], 2));
                case 2:
                    return a[1] * Math.Exp(-0.5 * Math.Pow(x - a[2], 2) / Math.Pow(a[3], 2)) * (x - a[2]) / Math.Pow(a[3], 2);
                case 3:
                    return a[1] * Math.Exp(-0.5 * Math.Pow(x - a[2], 2) / Math.Pow(a[3], 2)) * Math.Pow(x - a[2], 2) / Math.Pow(a[3], 3);
                default:
                    return double.NaN;
            }
        }
    }
}