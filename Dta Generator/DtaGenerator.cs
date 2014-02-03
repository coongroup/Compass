//#define Aaron_Experiment

using MSFileReaderLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coon.Compass.DtaGenerator
{
    public class DtaGenerator
    {
        private const double PROTON_MASS = 1.00727638;

        private const double PEAK_IDENTIFICATION_MASS_TOLERANCE = 0.01;

        // precursor cleaning constants
        private const double LOW_PRECURSOR_CLEANING_WINDOW_MZ = 5.0;
        private const double HIGH_PRECURSOR_CLEANING_WINDOW_MZ = 5.0;

        // ETD pre-processing constants
        private const double LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA = 60.0;

        // negative ETD pre-processing constants
        private const double NETD_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA = 50.0;
        private const double NETD_HIGH_NEUTRAL_LOSS_CLEANING_WINDOW_DA = 5.0;
        private const double NETD_ADDUCT_CLEANING_WINDOW_DA = 202.0;
        private const double NETD_ADDUCT_LOW_CLEANING_WINDOW_MZ = 5.0;
        private const double NETD_ADDUCT_HIGH_CLEANING_WINDOW_MZ = 5.0;
        private const double NETD_SINGLY_CHARGED_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_MZ = 50.0;

        // TMT duplex cleaning constants
        private const double TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        // TMT duplex CAD cleaning constants
        private const double MINIMUM_TMT_DUPLEX_CAD_REPORTER_MZ = 126.0;
        private const double MAXIMUM_TMT_DUPLEX_CAD_REPORTER_MZ = 127.0;
        private const double TMT_DUPLEX_CAD_TAG_MZ = 226.0;
        private const double TMT_DUPLEX_CAD_TAG_LOSS_DA = 225.0;

        // TMT duplex ETD cleaning constants
        private const double MINIMUM_TMT_DUPLEX_ETD_REPORTER_MZ = 112.0;
        private const double MAXIMUM_TMT_DUPLEX_ETD_REPORTER_MZ = 114.0;
        private const double TMT_DUPLEX_ETD_TAG_MZ = 226.0;
        private const double TMT_DUPLEX_ETD_REPORTER_LOSS_DA = 113.0;
        private const double TMT_DUPLEX_ETD_TAG_LOSS_DA = 225.0;

        // iTRAQ 4-plex cleaning constants
        private const double ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        // iTRAQ 4-plex CAD cleaning constants
        private const double MINIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ = 114.0;
        private const double MAXIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ = 117.0;
        private const double ITRAQ_4PLEX_CAD_TAG_MZ = 145.0;
        private const double ITRAQ_4PLEX_CAD_TAG_LOSS_DA = 145.0;

        // iTRAQ 4-plex ETD cleaning constants
        private const double MINIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ = 101.0;
        private const double MAXIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ = 104.0;
        private const double ITRAQ_4PLEX_ETD_TAG_MZ = 162.0;
        private const double ITRAQ_4PLEX_ETD_REPORTER_LOSS_DA = 102.0;
        private const double ITRAQ_4PLEX_ETD_TAG_LOSS_DA = 161.0;

        // TMT 6-plex cleaning constants
        private const double TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        // TMT 6-plex CAD cleaning constants
        private const double MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ = 126.0;
        private const double MAXIMUM_TMT_6PLEX_CAD_REPORTER_MZ = 131.0;
        private const double TMT_6PLEX_CAD_TAG_MZ = 230.0;
        private const double TMT_6PLEX_CAD_TAG_LOSS_DA = 229.0;

        // TMT 6-plex ETD cleaning constants
        private const double MINIMUM_TMT_6PLEX_ETD_REPORTER_MZ = 114.0;
        private const double MAXIMUM_TMT_6PLEX_ETD_REPORTER_MZ = 119.0;
        private const double TMT_6PLEX_ETD_TAG_MZ = 230.0;
        private const double TMT_6PLEX_ETD_REPORTER_LOSS_DA = 114.5;
        private const double TMT_6PLEX_ETD_TAG_LOSS_DA = 229.0;

        // iTRAQ 8-plex cleaning constants
        private const double ITRAQ_8PLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double ITRAQ_8PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        // iTRAQ 8-plex cleaning constants (CAD only)
        private const double MINIMUM_ITRAQ_8PLEX_CAD_REPORTER_MZ = 113.0;
        private const double MAXIMUM_ITRAQ_8PLEX_CAD_REPORTER_MZ = 121.0;
        private const double ITRAQ_8PLEX_CAD_TAG_MZ = 304.2;
        private const double ITRAQ_8PLEX_CAD_TAG_LOSS_DA = 304.2;
        private readonly bool cleanItraq4Plex;
        private readonly bool cleanItraq8Plex;
        private readonly bool cleanPrecursor;
        private readonly bool cleanTmt6Plex;
        private readonly bool cleanTmtDuplex;
        private readonly bool enableEtdPreProcessing;
        private readonly bool groupByActivationEnergyTime;
        private readonly bool mascotMgfOutput;
        private readonly int maximumAssumedPrecursorChargeState;
        private readonly int minimumAssumedPrecursorChargeState;
        private readonly List<double> neutralLosses;
        private readonly bool omssaTxtOutput;
        private readonly string outputFolder;
        private readonly IList<string> rawFilepaths;
        private readonly bool sequestDtaOutput;

        public static bool IncludeLog = true;

        public DtaGenerator(IList<string> rawFilepaths,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            bool cleanPrecursor, bool enableEtdPreProcessing,
            bool cleanTmtDuplex, bool cleanItraq4Plex, bool cleanTmt6Plex, bool cleanItraq8Plex,
            bool groupByActivationEnergyTime,
            bool sequestDtaOutput, bool omssaTxtOutput, bool mascotMgfOutput,
            string outputFolder,
            List<double> neutralLosses,
            bool includeLog = true)
        {
            this.rawFilepaths = rawFilepaths;
            this.minimumAssumedPrecursorChargeState = minimumAssumedPrecursorChargeState;
            this.maximumAssumedPrecursorChargeState = maximumAssumedPrecursorChargeState;
            this.cleanPrecursor = cleanPrecursor;
            this.enableEtdPreProcessing = enableEtdPreProcessing;
            this.cleanTmtDuplex = cleanTmtDuplex;
            this.cleanItraq4Plex = cleanItraq4Plex;
            this.cleanTmt6Plex = cleanTmt6Plex;
            this.cleanItraq8Plex = cleanItraq8Plex;
            this.groupByActivationEnergyTime = groupByActivationEnergyTime;
            this.sequestDtaOutput = sequestDtaOutput;
            this.omssaTxtOutput = omssaTxtOutput;
            this.mascotMgfOutput = mascotMgfOutput;
            this.outputFolder = outputFolder;
            this.neutralLosses = neutralLosses;
            IncludeLog = includeLog;
        }

        public event EventHandler Starting;

        protected virtual void OnStarting(EventArgs e)
        {
            EventHandler handler = Starting;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> StartingFile;

        protected virtual void onStartingFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = StartingFile;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void onUpdateProgress(ProgressEventArgs e)
        {
            EventHandler<ProgressEventArgs> handler = UpdateProgress;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<ExceptionEventArgs> ThrowException;

        protected virtual void onThrowException(ExceptionEventArgs e)
        {
            EventHandler<ExceptionEventArgs> handler = ThrowException;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<FilepathEventArgs> FinishedFile;

        protected virtual void onFinishedFile(FilepathEventArgs e)
        {
            EventHandler<FilepathEventArgs> handler = FinishedFile;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler Finished;

        protected virtual void onFinished(EventArgs e)
        {
            EventHandler handler = Finished;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void GenerateDtas()
        {
            StreamWriter overall_log = null;
            IXRawfile5 raw = null;
            StreamWriter log = null;
            StreamWriter dta = null;
            Dictionary<string, StreamWriter> txt_outputs = null;
            Dictionary<string, StreamWriter> mgf_outputs = null;
            string log_folder = "";

            try
            {
                OnStarting(new EventArgs());

                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                bool neutralLossesIncluded = (neutralLosses != null && neutralLosses.Count > 0);

                if (IncludeLog)
                {

                    overall_log = new StreamWriter(Path.Combine(outputFolder, "DTA_Generator_log.txt"));
                    overall_log.AutoFlush = true;

                    overall_log.WriteLine("DTA Generator PARAMETERS");
                    overall_log.WriteLine("Assumed Precursor Charge State Range: " + minimumAssumedPrecursorChargeState +
                                          '-' + maximumAssumedPrecursorChargeState);
                    overall_log.WriteLine("Clean Precursor: " + cleanPrecursor);
                    overall_log.WriteLine("Enable ETD Pre-Processing: " + enableEtdPreProcessing);
                    overall_log.WriteLine("Clean TMT Duplex: " + cleanTmtDuplex);
                    overall_log.WriteLine("Clean iTRAQ 4-Plex: " + cleanItraq4Plex);
                    overall_log.WriteLine("Clean TMT 6-Plex: " + cleanTmt6Plex);
                    overall_log.WriteLine("Clean iTRAQ 8-Plex: " + cleanItraq8Plex);


                    if (neutralLossesIncluded)
                    {
                        overall_log.WriteLine("Neutral Losses: true");
                    }

                    overall_log.WriteLine();
                    foreach (string raw_filepath in rawFilepaths)
                    {
                        overall_log.WriteLine(raw_filepath);
                    }
                    overall_log.Close();

                    log_folder = Path.Combine(outputFolder, "log");
                    if (!Directory.Exists(log_folder))
                    {
                        Directory.CreateDirectory(log_folder);
                    }
                }

                for (int file_index = 0; file_index < rawFilepaths.Count; file_index++)
                {
                    string filepath = rawFilepaths[file_index];
                    onStartingFile(new FilepathEventArgs(filepath));

                    txt_outputs = new Dictionary<string, StreamWriter>();
                    mgf_outputs = new Dictionary<string, StreamWriter>();
                    var spectrum_counts = new SortedDictionary<string, int>();
                    var dta_counts = new SortedDictionary<string, int>();
                    var retention_times = new SortedDictionary<int, double>();
                    var scan_filter_mzs = new SortedDictionary<int, double>();
                    var precursor_mzs = new SortedDictionary<int, double>();
                    var precursor_intensities = new SortedDictionary<int, double>();
                    var precursor_denormalized_intensities = new SortedDictionary<int, double>();
                    var precursor_charge_states = new SortedDictionary<int, int>();
                    var precursor_fragmentation_methods = new SortedDictionary<int, string>();
                    var elapsed_scan_times = new SortedDictionary<int, double>();
                    var ion_injection_times = new SortedDictionary<int, double>();
                    var precursor_sns = new SortedDictionary<int, double?>();
                    var precursor_peak_depths = new SortedDictionary<int, int>();

                    //raw = (IXRawfile2)new XRawfile();
                    raw = (IXRawfile5)new MSFileReader_XRawfile();
                    raw.Open(filepath);
                    raw.SetCurrentController(0, 1);

                    if (IncludeLog)
                    {

                        log =
                            new StreamWriter(Path.Combine(log_folder,
                                Path.GetFileNameWithoutExtension(filepath) + "_log.txt"));
                        log.AutoFlush = true;

                        log.WriteLine("DTA Generator PARAMETERS");
                        log.WriteLine("Assumed Precursor Charge State Range: " + minimumAssumedPrecursorChargeState + '-' +
                                      maximumAssumedPrecursorChargeState);
                        log.WriteLine("Clean Precursor: " + cleanPrecursor);
                        log.WriteLine("Enable ETD Pre-Processing: " + enableEtdPreProcessing);
                        log.WriteLine("Clean TMT Duplex: " + cleanTmtDuplex);
                        log.WriteLine("Clean iTRAQ 4-Plex: " + cleanItraq4Plex);
                        log.WriteLine("Clean TMT 6-Plex: " + cleanTmt6Plex);
                        log.WriteLine("Clean iTRAQ 8-Plex: " + cleanItraq8Plex);
                        log.WriteLine();
                    }

                    int first_scan_number = -1;
                    raw.GetFirstSpectrumNumber(ref first_scan_number);
                    int last_scan_number = -1;
                    raw.GetLastSpectrumNumber(ref last_scan_number);

                    // DJB Addition, Check all the scans first to see if they are a precursor scan or not
                    var msScans = new bool[last_scan_number + 1];

                    for (int scanNumber = first_scan_number; scanNumber <= last_scan_number; scanNumber++)
                    {
                        int msn = 0;
                        ((IXRawfile4) raw).GetMSOrderForScanNum(scanNumber, ref msn);

                        // skip MS1s
                        if (msn == 1)
                        {
                            msScans[scanNumber] = true;
                            continue;
                        }

                        // Retention Time
                        double time = -1.0;
                        raw.RTFromScanNum(scanNumber, ref time);
                        retention_times.Add(scanNumber, time);

                        // Precursor m/z
                        double precursorMZ = double.NaN;
                        ((IXRawfile4) raw).GetPrecursorMassForScanNum(scanNumber, 2, ref precursorMZ);
                        scan_filter_mzs.Add(scanNumber, precursorMZ);
                        precursor_mzs.Add(scanNumber, precursorMZ);

                        double centroid_peak_width = -1.0;
                        object precursor_labels = null;
                        object precursor_flags = null;
                        int mass_list_array_size = -1;
                        bool no_precursor_scan;

                        int precursor_scan_number = scanNumber - 1;

                        no_precursor_scan = true;

                        for (int i = precursor_scan_number; i >= 0; i--)
                        {
                            if (!msScans[i])
                                continue;
                            no_precursor_scan = false;
                            precursor_scan_number = i;
                            break;
                        }



                        string scan_filter = null;
                        raw.GetFilterForScanNum(scanNumber, ref scan_filter);

                        if (!no_precursor_scan && IncludeLog)
                        {
                            precursor_labels = null;
                            precursor_flags = null;
                            raw.GetLabelData(ref precursor_labels, ref precursor_flags, ref precursor_scan_number);

                            var precursor_data = (double[,]) precursor_labels;

                            if (precursor_data.Length == 0)
                            {
                                centroid_peak_width = -1.0;
                                precursor_labels = null;
                                precursor_flags = null;
                                mass_list_array_size = -1;
                                raw.GetMassListFromScanNum(ref precursor_scan_number, null, 0, 0, 0, 1,
                                    ref centroid_peak_width, ref precursor_labels, ref precursor_flags,
                                    ref mass_list_array_size);
                                precursor_data = (double[,]) precursor_labels;
                            }

                            

                            int? precursor_index = null;
                            for (int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                            {
                                if (Math.Abs(precursor_data[(int) RawLabelDataColumn.MZ, i] - precursorMZ) <=
                                    PEAK_IDENTIFICATION_MASS_TOLERANCE)
                                {
                                    if (!precursor_index.HasValue ||
                                        precursor_data[(int) RawLabelDataColumn.Intensity, i] >
                                        precursor_data[(int) RawLabelDataColumn.Intensity, precursor_index.Value])
                                    {
                                        precursor_index = i;
                                    }
                                }
                            }

                            if (!precursor_index.HasValue)
                            {
                                for (int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                                {
                                    if (!precursor_index.HasValue ||
                                        Math.Abs(precursor_data[(int) RawLabelDataColumn.MZ, i] - precursorMZ) <
                                        Math.Abs(precursor_data[(int) RawLabelDataColumn.MZ, precursor_index.Value] -
                                                    precursorMZ))
                                    {
                                        precursor_index = i;
                                    }
                                }
                            }

                            if (precursor_index.HasValue)
                                {
                                    precursorMZ = precursor_data[(int) RawLabelDataColumn.MZ, precursor_index.Value];

                                    //precursor_mzs.Add(scan_number, precursor_mz);
                                    precursor_intensities.Add(scanNumber,
                                        precursor_data[(int) RawLabelDataColumn.Intensity, precursor_index.Value]);

                                    object precursor_header_labels = null;
                                    object precursor_header_values = null;
                                    int precursor_array_size = -1;
                                    raw.GetTrailerExtraForScanNum(precursor_scan_number, ref precursor_header_labels,
                                        ref precursor_header_values, ref precursor_array_size);
                                    var precursor_header_label_strings = (string[]) precursor_header_labels;
                                    var precursor_header_value_strings = (string[]) precursor_header_values;
                                    if (precursor_header_label_strings != null && precursor_header_value_strings != null)
                                    {
                                        for (int header_i = precursor_header_label_strings.GetLowerBound(0);
                                            header_i <= precursor_header_label_strings.GetUpperBound(0);
                                            header_i++)
                                        {
                                            if (
                                                precursor_header_label_strings[header_i].StartsWith(
                                                    "Ion Injection Time (ms)"))
                                            {
                                                precursor_denormalized_intensities.Add(scanNumber,
                                                    precursor_data[(int) RawLabelDataColumn.Intensity, precursor_index.Value
                                                        ]*double.Parse(precursor_header_value_strings[header_i])/1000.0);
                                            }
                                        }
                                    }

                                    if (precursor_data.GetLength(0) > 2)
                                    {
                                        precursor_sns.Add(scanNumber,
                                            (precursor_data[(int) RawLabelDataColumn.Intensity, precursor_index.Value] -
                                             precursor_data[(int) RawLabelDataColumn.NoiseBaseline, precursor_index.Value])/
                                            precursor_data[(int) RawLabelDataColumn.NoiseLevel, precursor_index.Value]);
                                    }
                                    else
                                    {
                                        precursor_sns.Add(scanNumber, null);
                                    }

                                    int peak_depth = 1;
                                    for (int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                                    {
                                        if (i != precursor_index.Value)
                                        {
                                            if (precursor_data[(int) RawLabelDataColumn.Intensity, i] >
                                                precursor_data[(int) RawLabelDataColumn.Intensity, precursor_index.Value])
                                            {
                                                peak_depth++;
                                            }
                                        }
                                    }

                                    precursor_peak_depths.Add(scanNumber, peak_depth);
                                }
                            
                        }
                     
                        object header_labels = null;
                        object header_values = null;
                        int array_size = -1;
                        raw.GetTrailerExtraForScanNum(scanNumber, ref header_labels, ref header_values, ref array_size);
                        var header_label_strings = (string[]) header_labels;
                        var header_value_strings = (string[]) header_values;
                        int charge = 0;
                        if (header_label_strings != null && header_value_strings != null)
                        {
                            for (int header_i = header_label_strings.GetLowerBound(0);
                                header_i <= header_label_strings.GetUpperBound(0);
                                header_i++)
                            {
                                if (header_label_strings[header_i].StartsWith("Charge"))
                                {
                                    charge = int.Parse(header_value_strings[header_i]);
                                    if (scan_filter.Contains(" - "))
                                    {
                                        charge = -charge;
                                    }
                                    precursor_charge_states.Add(scanNumber, charge);
                                }
                                else if (header_label_strings[header_i].StartsWith("Elapsed Scan Time (sec)"))
                                {
                                    elapsed_scan_times.Add(scanNumber, double.Parse(header_value_strings[header_i]));
                                }
                                else if (header_label_strings[header_i].StartsWith("Ion Injection Time (ms)"))
                                {
                                    ion_injection_times.Add(scanNumber, double.Parse(header_value_strings[header_i]));
                                }
                            }
                        }


                        object labels = null;
                        object flags = null;
                        raw.GetLabelData(ref labels, ref flags, ref scanNumber);

                        var data = (double[,]) labels;
                        if (data.Length == 0)
                        {
                            centroid_peak_width = -1.0;
                            labels = null;
                            flags = null;
                            mass_list_array_size = -1;
                            raw.GetMassListFromScanNum(ref scanNumber, null, 0, 0, 0, 1, ref centroid_peak_width,
                                ref labels, ref flags, ref mass_list_array_size);
                            data = (double[,]) labels;
                        }

                        double total_ion_current = 0.0;
                        double base_peak_mz = -1.0;
                        double base_peak_intensity = -1.0;
                        for (int data_i = data.GetLowerBound(1); data_i <= data.GetUpperBound(1); data_i++)
                        {
                            total_ion_current += data[(int) RawLabelDataColumn.Intensity, data_i];

                            if (base_peak_mz < 0.0 ||
                                data[(int) RawLabelDataColumn.Intensity, data_i] > base_peak_intensity)
                            {
                                base_peak_mz = data[(int) RawLabelDataColumn.MZ, data_i];
                                base_peak_intensity = data[(int) RawLabelDataColumn.Intensity, data_i];
                            }
                        }

                        var charges = new List<int>();
                        if (charge == 0 || no_precursor_scan)
                        {
                            for (int assumed_charge_state = minimumAssumedPrecursorChargeState;
                                assumed_charge_state <= maximumAssumedPrecursorChargeState;
                                assumed_charge_state++)
                            {
                                charges.Add(assumed_charge_state);
                            }
                        }
                        else
                        {
                            charges.Add(charge);
                        }

                        string mass_analyzer = scan_filter.Substring(0, 4).ToUpper();
                        if (!mass_analyzer.Contains("MS"))
                        {
                            mass_analyzer = "TQMS";
                        }

                        string fragmentation_method = null;
                        if (groupByActivationEnergyTime)
                        {
                            foreach (int i in AllIndicesOf(scan_filter, '@'))
                            {
                                string temp_scan_filter = scan_filter.Substring(i + 1);
                                temp_scan_filter = temp_scan_filter.Substring(0, temp_scan_filter.IndexOf(' '));
                                fragmentation_method += temp_scan_filter.ToUpper() + '-';
                            }
                        }
                        else
                        {
                            foreach (int i in AllIndicesOf(scan_filter, '@'))
                            {
                                fragmentation_method += scan_filter.Substring(i + 1, 3).ToUpper() + '-';
                            }
                        }
                        fragmentation_method = fragmentation_method.Substring(0, fragmentation_method.Length - 1);

                        string base_output_filename = Path.GetFileNameWithoutExtension(filepath) + '_' + mass_analyzer +
                                                      '_' + fragmentation_method;

                        precursor_fragmentation_methods.Add(scanNumber, fragmentation_method);

                        if (!spectrum_counts.ContainsKey(mass_analyzer))
                        {
                            spectrum_counts.Add(mass_analyzer, 0);
                        }
                        spectrum_counts[mass_analyzer]++;

                        if (!dta_counts.ContainsKey(mass_analyzer))
                        {
                            dta_counts.Add(mass_analyzer, 0);
                        }
                        dta_counts[mass_analyzer] += charges.Count;

                        if (!spectrum_counts.ContainsKey(fragmentation_method))
                        {
                            spectrum_counts.Add(fragmentation_method, 0);
                        }
                        spectrum_counts[fragmentation_method]++;

                        if (!dta_counts.ContainsKey(fragmentation_method))
                        {
                            dta_counts.Add(fragmentation_method, 0);
                        }
                        dta_counts[fragmentation_method] += charges.Count;

                        if (!spectrum_counts.ContainsKey(mass_analyzer + ' ' + fragmentation_method))
                        {
                            spectrum_counts.Add(mass_analyzer + ' ' + fragmentation_method, 0);
                        }
                        spectrum_counts[mass_analyzer + ' ' + fragmentation_method]++;

                        if (!dta_counts.ContainsKey(mass_analyzer + ' ' + fragmentation_method))
                        {
                            dta_counts.Add(mass_analyzer + ' ' + fragmentation_method, 0);
                        }
                        dta_counts[mass_analyzer + ' ' + fragmentation_method] += charges.Count;

                        if (sequestDtaOutput || omssaTxtOutput || mascotMgfOutput)
                        {
                            var all_peaks = new List<MSPeak>();
                            for (int data_i = data.GetLowerBound(1); data_i <= data.GetUpperBound(1); data_i++)
                            {
                                double mz = data[(int) RawLabelDataColumn.MZ, data_i];

                                all_peaks.Add(new MSPeak(mz, data[(int) RawLabelDataColumn.Intensity, data_i]));
                            }

                            foreach (int charge_i in charges)
                            {
                                double retention_time_min = double.NaN;
                                raw.RTFromScanNum(scanNumber, ref retention_time_min);
                                double retention_time_s = retention_time_min*60;

                                string dta_filepath = Path.GetFileNameWithoutExtension(filepath) +
                                                      '.' + mass_analyzer + '.' + fragmentation_method +
                                                      '.' + scanNumber + '.' +
                                                      scanNumber + '.' +
                                                      charge_i + '.' +
                                                      "RT_" + retention_time_min.ToString("0.000") + "_min_" +
                                                      retention_time_s.ToString("0.0") + "_s" +
                                                      ".dta";

                                double precursor_mass = MassFromMZ(precursorMZ, charge_i);

                                var dta_content_sb = new StringBuilder();

                                var mgf_content_sb = new StringBuilder();

                                dta_content_sb.AppendLine((precursor_mass + PROTON_MASS).ToString("0.00000") + ' ' +
                                                          charge_i);

                                mgf_content_sb.AppendLine("BEGIN IONS");
                                mgf_content_sb.AppendLine("Title=" + Path.GetFileNameWithoutExtension(dta_filepath));
                                mgf_content_sb.AppendLine("SCANS=" + scanNumber);
                                mgf_content_sb.AppendLine("RTINSECONDS=" + retention_time_s);
                                mgf_content_sb.AppendLine("PEPMASS=" + precursorMZ.ToString("0.00000"));
                                mgf_content_sb.AppendLine("CHARGE=" + charge_i.ToString("0+;0-"));

                                var peaks = new List<MSPeak>(all_peaks);
                                bool isHCD = fragmentation_method.StartsWith("HCD");
                                bool isETD = fragmentation_method.StartsWith("ETD") ||
                                             fragmentation_method.StartsWith("ECD");

                                // FOR AARON
#if Aaron_Experiment
                                isETD = isETD || scan_filter.Contains("hcd0.5") || scan_filter.Contains("hcd1.5");
                                #endif
                                //

                                // precursor cleaning
                                if (cleanPrecursor || (enableEtdPreProcessing && isETD))
                                {
                                    int p = 0;
                                    while (p < peaks.Count)
                                    {
                                        double mz = peaks[p].MZ;

                                        if (mz >= precursorMZ - LOW_PRECURSOR_CLEANING_WINDOW_MZ &&
                                            mz <= precursorMZ + HIGH_PRECURSOR_CLEANING_WINDOW_MZ)
                                        {
                                            peaks.RemoveAt(p);
                                        }
                                        else
                                        {
                                            p++;
                                        }
                                    }
                                }

                                if (neutralLossesIncluded)
                                {
                                    var mzs = new List<KeyValuePair<double, double>>();
                                    foreach (double nl_mass in neutralLosses)
                                    {
                                        double mz = precursorMZ - MZFromMass(nl_mass, charge_i);
                                        double min = mz - LOW_PRECURSOR_CLEANING_WINDOW_MZ;
                                        double max = mz + HIGH_PRECURSOR_CLEANING_WINDOW_MZ;
                                        mzs.Add(new KeyValuePair<double, double>(min, max));
                                    }
                                    int p = 0;
                                    while (p < peaks.Count)
                                    {
                                        double mz = peaks[p].MZ;
                                        if (mz >= precursorMZ)
                                        {
                                            break;
                                        }
                                        bool removed = false;
                                        foreach (var minmax in mzs)
                                        {
                                            if (mz >= minmax.Key && mz <= minmax.Value)
                                            {
                                                peaks.RemoveAt(p);
                                                removed = true;
                                                break;
                                            }
                                        }
                                        if (!removed)
                                        {
                                            p++;
                                        }
                                    }
                                }

                                // ETD pre-processing
                                if (enableEtdPreProcessing && isETD)
                                {
                                    // negative ETD
                                    if (scan_filter.Contains(" - "))
                                    {
                                        int p1 = 0;
                                        while (p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            bool clean = false;

                                            for (int reduced_precursor_charge = -2;
                                                reduced_precursor_charge >= charge_i + 1;
                                                reduced_precursor_charge--)
                                            {
                                                if (mz >=
                                                    MZFromMass(
                                                        precursor_mass - NETD_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA,
                                                        reduced_precursor_charge) &&
                                                    mz <=
                                                    MZFromMass(
                                                        precursor_mass + NETD_HIGH_NEUTRAL_LOSS_CLEANING_WINDOW_DA,
                                                        reduced_precursor_charge))
                                                {
                                                    clean = true;
                                                    break;
                                                }

                                                if (mz >=
                                                    MZFromMass(precursor_mass + NETD_ADDUCT_CLEANING_WINDOW_DA,
                                                        reduced_precursor_charge) - NETD_ADDUCT_LOW_CLEANING_WINDOW_MZ &&
                                                    mz <=
                                                    MZFromMass(precursor_mass + NETD_ADDUCT_CLEANING_WINDOW_DA,
                                                        reduced_precursor_charge) + NETD_ADDUCT_LOW_CLEANING_WINDOW_MZ)
                                                {
                                                    clean = true;
                                                    break;
                                                }
                                            }

                                            if (!clean)
                                            {
                                                if (mz >=
                                                    MZFromMass(precursor_mass, -1) -
                                                    NETD_SINGLY_CHARGED_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_MZ)
                                                {
                                                    clean = true;
                                                }
                                            }

                                            if (clean)
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                        // positive ETD
                                    else
                                    {
                                        int p1 = 0;
                                        while (p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            bool clean = false;

                                            for (int reduced_precursor_charge = 1;
                                                reduced_precursor_charge <= charge_i - 1;
                                                reduced_precursor_charge++)
                                            {
                                                if (mz >=
                                                    MZFromMass(precursor_mass - LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA,
                                                        reduced_precursor_charge) &&
                                                    mz <
                                                    MZFromMass(precursor_mass, reduced_precursor_charge) +
                                                    HIGH_PRECURSOR_CLEANING_WINDOW_MZ)
                                                {
                                                    clean = true;
                                                    break;
                                                }
                                            }

                                            if (clean)
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                }

                                // TMT duplex cleaning
                                if (cleanTmtDuplex)
                                {
                                    if (fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") ||
                                        fragmentation_method.StartsWith("HCD"))
                                    {
                                        for (int reduced_charge_i = charge_i - 1;
                                            reduced_charge_i >= 1;
                                            reduced_charge_i--)
                                        {
                                            double precursor_tmt_duplex_tag_cleaning_mz = precursorMZ*reduced_charge_i -
                                                                                          TMT_DUPLEX_CAD_TAG_LOSS_DA/
                                                                                          reduced_charge_i;

                                            int p1 = 0;
                                            while (p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;

                                                if ((mz >=
                                                     MINIMUM_TMT_DUPLEX_CAD_REPORTER_MZ -
                                                     TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     MAXIMUM_TMT_DUPLEX_CAD_REPORTER_MZ +
                                                     TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >= TMT_DUPLEX_CAD_TAG_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <= TMT_DUPLEX_CAD_TAG_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_duplex_tag_cleaning_mz -
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_duplex_tag_cleaning_mz +
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                                {
                                                    peaks.RemoveAt(p1);
                                                }
                                                else
                                                {
                                                    p1++;
                                                }
                                            }
                                        }
                                    }
                                    else if (fragmentation_method.StartsWith("ETD"))
                                    {
                                        for (int reduced_charge_i = charge_i - 1;
                                            reduced_charge_i >= 1;
                                            reduced_charge_i--)
                                        {
                                            double precursor_tmt_duplex_reporter_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                                    TMT_DUPLEX_ETD_REPORTER_LOSS_DA/
                                                                                                    reduced_charge_i;
                                            double precursor_tmt_duplex_tag_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                               TMT_DUPLEX_ETD_TAG_LOSS_DA/
                                                                                               reduced_charge_i;

                                            int p1 = 0;
                                            while (p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;
                                                if ((mz >=
                                                     MINIMUM_TMT_DUPLEX_ETD_REPORTER_MZ -
                                                     TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     MAXIMUM_TMT_DUPLEX_ETD_REPORTER_MZ +
                                                     TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >= TMT_DUPLEX_ETD_TAG_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <= TMT_DUPLEX_ETD_TAG_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_duplex_reporter_loss_cleaning_mz -
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_duplex_reporter_loss_cleaning_mz +
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_duplex_tag_loss_cleaning_mz -
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_duplex_tag_loss_cleaning_mz +
                                                     TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                                {
                                                    peaks.RemoveAt(p1);
                                                }
                                                else
                                                {
                                                    p1++;
                                                }
                                            }
                                        }
                                    }
                                }
                                // iTRAQ 4-plex cleaning
                                if (cleanItraq4Plex)
                                {
                                    if (fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") ||
                                        fragmentation_method.StartsWith("HCD"))
                                    {
                                        double precursor_itraq_4plex_tag_cleaning_mz = precursorMZ*charge_i -
                                                                                       ITRAQ_4PLEX_CAD_TAG_LOSS_DA;

                                        int p1 = 0;
                                        while (p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            if ((mz >=
                                                 MINIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ -
                                                 ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 MAXIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ +
                                                 ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >= ITRAQ_4PLEX_CAD_TAG_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <= ITRAQ_4PLEX_CAD_TAG_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >=
                                                 precursor_itraq_4plex_tag_cleaning_mz -
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 precursor_itraq_4plex_tag_cleaning_mz +
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                    else if (fragmentation_method.StartsWith("ETD"))
                                    {
                                        double precursor_itraq_4plex_reporter_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                                 ITRAQ_4PLEX_ETD_REPORTER_LOSS_DA;
                                        double precursor_itraq_4plex_tag_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                            ITRAQ_4PLEX_ETD_TAG_LOSS_DA;

                                        int p1 = 0;
                                        while (p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;
                                            if ((mz >=
                                                 MINIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ -
                                                 ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 MAXIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ +
                                                 ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >= ITRAQ_4PLEX_ETD_TAG_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <= ITRAQ_4PLEX_ETD_TAG_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >=
                                                 precursor_itraq_4plex_reporter_loss_cleaning_mz -
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 precursor_itraq_4plex_reporter_loss_cleaning_mz +
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >=
                                                 precursor_itraq_4plex_tag_loss_cleaning_mz -
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 precursor_itraq_4plex_tag_loss_cleaning_mz +
                                                 ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                }
                                // TMT 6-plex cleaning
                                if (cleanTmt6Plex)
                                {
                                    if (fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") ||
                                        fragmentation_method.StartsWith("HCD"))
                                    {
                                        for (int reduced_charge_i = charge_i - 1;
                                            reduced_charge_i >= 1;
                                            reduced_charge_i--)
                                        {
                                            double precursor_tmt_6plex_tag_cleaning_mz = precursorMZ*reduced_charge_i -
                                                                                         TMT_6PLEX_CAD_TAG_LOSS_DA/
                                                                                         reduced_charge_i;

                                            int p1 = 0;
                                            while (p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;

                                                if ((mz >=
                                                     MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ -
                                                     TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     MAXIMUM_TMT_6PLEX_CAD_REPORTER_MZ +
                                                     TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >= TMT_6PLEX_CAD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <= TMT_6PLEX_CAD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_6plex_tag_cleaning_mz -
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_6plex_tag_cleaning_mz +
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                                {
                                                    peaks.RemoveAt(p1);
                                                }
                                                else
                                                {
                                                    p1++;
                                                }
                                            }
                                        }
                                    }
                                    else if (fragmentation_method.StartsWith("ETD"))
                                    {
                                        for (int reduced_charge_i = charge_i - 1;
                                            reduced_charge_i >= 1;
                                            reduced_charge_i--)
                                        {
                                            double precursor_tmt_6plex_reporter_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                                   TMT_6PLEX_ETD_REPORTER_LOSS_DA/
                                                                                                   reduced_charge_i;
                                            double precursor_tmt_6plex_tag_loss_cleaning_mz = precursorMZ*charge_i -
                                                                                              TMT_6PLEX_ETD_TAG_LOSS_DA/
                                                                                              reduced_charge_i;

                                            int p1 = 0;
                                            while (p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;
                                                if ((mz >=
                                                     MINIMUM_TMT_6PLEX_ETD_REPORTER_MZ -
                                                     TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     MAXIMUM_TMT_6PLEX_ETD_REPORTER_MZ +
                                                     TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >= TMT_6PLEX_ETD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <= TMT_6PLEX_ETD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_6plex_reporter_loss_cleaning_mz -
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_6plex_reporter_loss_cleaning_mz +
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                    ||
                                                    (mz >=
                                                     precursor_tmt_6plex_tag_loss_cleaning_mz -
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                     &&
                                                     mz <=
                                                     precursor_tmt_6plex_tag_loss_cleaning_mz +
                                                     TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                                {
                                                    peaks.RemoveAt(p1);
                                                }
                                                else
                                                {
                                                    p1++;
                                                }
                                            }
                                        }
                                    }
                                }
                                // iTRAQ 8-plex cleaning
                                if (cleanItraq8Plex)
                                {
                                    if (fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") ||
                                        fragmentation_method.StartsWith("HCD"))
                                    {
                                        double precursor_itraq_4plex_tag_cleaning_mz = precursorMZ*charge_i -
                                                                                       ITRAQ_8PLEX_CAD_TAG_LOSS_DA;

                                        int p1 = 0;
                                        while (p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            if ((mz >=
                                                 MINIMUM_ITRAQ_8PLEX_CAD_REPORTER_MZ -
                                                 ITRAQ_8PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 MAXIMUM_ITRAQ_8PLEX_CAD_REPORTER_MZ +
                                                 ITRAQ_8PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >= ITRAQ_8PLEX_CAD_TAG_MZ - ITRAQ_8PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <= ITRAQ_8PLEX_CAD_TAG_MZ + ITRAQ_8PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                ||
                                                (mz >=
                                                 precursor_itraq_4plex_tag_cleaning_mz -
                                                 ITRAQ_8PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                 &&
                                                 mz <=
                                                 precursor_itraq_4plex_tag_cleaning_mz +
                                                 ITRAQ_8PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                }

                                MSPeak base_peak = null;
                                foreach (MSPeak peak in peaks)
                                {
                                    if (base_peak == null || peak.Intensity > base_peak.Intensity)
                                    {
                                        base_peak = peak;
                                    }
                                }

                                if (omssaTxtOutput)
                                {
                                    foreach (MSPeak peak in peaks)
                                    {
                                        dta_content_sb.AppendFormat(@" {0:0.0000} {1:0.00}", peak.MZ, peak.Intensity);
                                        dta_content_sb.AppendLine();
                                    }
                                }

                                if (mascotMgfOutput)
                                {
                                    foreach (MSPeak peak in peaks)
                                    {
                                        mgf_content_sb.AppendLine(string.Format("{0:0.00000} {1:0.00}", peak.MZ,
                                            peak.Intensity));
                                    }
                                }

                                //foreach(MSPeak peak in peaks)
                                //{
                                //    dta_content_sb.AppendLine(' ' + peak.MZ.ToString("0.00000") + ' ' +
                                //        peak.Intensity.ToString("0.00"));
                                //    mgf_content_sb.AppendLine(peak.MZ.ToString("0.00000") + ' ' +
                                //        peak.Intensity.ToString("0.00"));
                                //}

                                if (sequestDtaOutput)
                                {
                                    dta = new StreamWriter(Path.Combine(outputFolder, dta_filepath));

                                    if (dta_content_sb.Length > 0)
                                    {
                                        dta.Write(dta_content_sb.ToString());
                                    }

                                    dta.Close();
                                }

                                if (omssaTxtOutput)
                                {
                                    string txt_filepath = Path.Combine(outputFolder,
                                        base_output_filename + ".txt");

                                    if (!txt_outputs.ContainsKey(txt_filepath))
                                    {
                                        txt_outputs.Add(txt_filepath, new StreamWriter(txt_filepath));
                                    }

                                    StreamWriter txt = txt_outputs[txt_filepath];

                                    txt.WriteLine("<dta id=\"" + scanNumber + "\" name=\"" + dta_filepath + "\">");
                                    txt.WriteLine();

                                    //foreach (MSPeak peak in peaks)
                                    //{
                                    //    txt.WriteLine(" {0:0.0000} {1:0.00}", peak.MZ, peak.Intensity);
                                    //}

                                    if (dta_content_sb.Length > 0)
                                    {
                                        txt.Write(dta_content_sb.ToString());
                                    }

                                    txt.WriteLine();
                                    txt.WriteLine();
                                }

                                if (mascotMgfOutput)
                                {
                                    string mgf_filepath = Path.Combine(outputFolder,
                                        base_output_filename + ".mgf");

                                    if (!mgf_outputs.ContainsKey(mgf_filepath))
                                    {
                                        mgf_outputs.Add(mgf_filepath, new StreamWriter(mgf_filepath));
                                    }

                                    StreamWriter mgf = mgf_outputs[mgf_filepath];

                                    if (mgf_content_sb.Length > 0)
                                    {
                                        mgf.Write(mgf_content_sb.ToString());
                                    }

                                    mgf.WriteLine("END IONS");
                                    mgf.WriteLine();
                                }
                            }
                        }

                        double progress = (double) (scanNumber - first_scan_number + 1)/
                                          (last_scan_number - first_scan_number + 1);
                        onUpdateProgress(new ProgressEventArgs((int) (progress*100)));
                    }

                    raw.Close();

                    if (txt_outputs != null)
                    {
                        foreach (StreamWriter sw in txt_outputs.Values)
                        {
                            if (sw != null)
                            {
                                sw.Close();
                            }
                        }
                    }
                    if (mgf_outputs != null)
                    {
                        foreach (StreamWriter sw in mgf_outputs.Values)
                        {
                            if (sw != null)
                            {
                                sw.Close();
                            }
                        }
                    }

                    if (IncludeLog)
                    {

                        log.WriteLine("Spectrum Type\tNumber of Scans");
                        foreach (var kvp in spectrum_counts)
                        {
                            log.WriteLine(kvp.Key + '\t' + kvp.Value);
                        }
                        log.WriteLine();

                        log.WriteLine("Spectrum Type\tNumber of DTAs");
                        foreach (var kvp in dta_counts)
                        {
                            log.WriteLine(kvp.Key + '\t' + kvp.Value);
                        }
                        log.WriteLine();

                        double? min_elapsed = null;
                        double? max_elapsed = null;
                        double mean_elapsed = 0.0;
                        foreach (double elapsed_scan_time in elapsed_scan_times.Values)
                        {
                            if (!min_elapsed.HasValue || elapsed_scan_time < min_elapsed)
                            {
                                min_elapsed = elapsed_scan_time;
                            }

                            if (!max_elapsed.HasValue || elapsed_scan_time > max_elapsed)
                            {
                                max_elapsed = elapsed_scan_time;
                            }

                            mean_elapsed += elapsed_scan_time;
                        }
                        mean_elapsed /= elapsed_scan_times.Count;

                        if (min_elapsed.HasValue)
                        {
                            log.WriteLine("Minimum Fragmentation Elapsed Scan Time (sec): " + min_elapsed.Value);
                        }
                        if (max_elapsed.HasValue)
                        {
                            log.WriteLine("Maximum Fragmentation Elapsed Scan Time (sec): " + max_elapsed.Value);
                        }
                        if (!Double.IsNaN(mean_elapsed))
                        {
                            log.WriteLine("Average Fragmentation Elapsed Scan Time (sec): " + mean_elapsed);
                        }

                        log.WriteLine();



                        double? min_injection = null;
                        double? max_injection = null;
                        double mean_injection = 0.0;
                        foreach (double ion_injection_time in ion_injection_times.Values)
                        {
                            if (!min_injection.HasValue || ion_injection_time < min_injection)
                            {
                                min_injection = ion_injection_time;
                            }

                            if (!max_injection.HasValue || ion_injection_time > max_injection)
                            {
                                max_injection = ion_injection_time;
                            }

                            mean_injection += ion_injection_time;
                        }
                        mean_injection /= ion_injection_times.Count;

                        if (min_injection.HasValue)
                        {
                            log.WriteLine("Minimum Fragmentation Ion Injection Time (msec): " + min_injection.Value);
                        }
                        if (max_injection.HasValue)
                        {
                            log.WriteLine("Maximum Fragmentation Ion Injection Time (msec): " + max_injection.Value);
                        }
                        if (!Double.IsNaN(mean_injection))
                        {
                            log.WriteLine("Average Fragmentation Ion Injection Time (msec): " + mean_injection);
                        }

                        log.WriteLine();

                        log.WriteLine("Fragmentation Scan Summary");
                        log.Write("Fragmentation Scan Number\t");
                        log.Write("Retention Time (min.)\t");
                        log.Write("Scan Filter m/z\t");
                        log.Write("Precursor m/z\t");
                        log.Write("Precursor Intensity\t");
                        log.Write("Precursor Denormalized Intensity\t");
                        log.Write("Precursor Charge State\t");
                        log.Write("Precursor S/N Ratio\t");
                        log.Write("Precursor Peak Depth\t");
                        log.Write("Fragmentation Method\t");
                        log.Write("Elapsed Scan Time (sec)\t");
                        log.Write("Ion Injection Time (msec)");
                        log.WriteLine();
                        foreach (int sn2 in retention_times.Keys)
                        {
                            log.Write(sn2.ToString() + '\t');
                            log.Write(retention_times[sn2].ToString("0.00") + '\t');
                            log.Write(scan_filter_mzs[sn2].ToString("0.00") + '\t');
                            log.Write((precursor_mzs.ContainsKey(sn2) ? precursor_mzs[sn2].ToString("0.00000") : "n/a") +
                                      '\t');
                            log.Write((precursor_intensities.ContainsKey(sn2)
                                ? precursor_intensities[sn2].ToString("0.0")
                                : "n/a") + '\t');
                            log.Write((precursor_denormalized_intensities.ContainsKey(sn2)
                                ? precursor_denormalized_intensities[sn2].ToString("0.0")
                                : "n/a") + '\t');
                            log.Write((precursor_charge_states.ContainsKey(sn2)
                                ? precursor_charge_states[sn2].ToString()
                                : "n/a") + '\t');
                            log.Write((precursor_sns.ContainsKey(sn2) && precursor_sns[sn2].HasValue
                                ? precursor_sns[sn2].Value.ToString()
                                : "n/a") + '\t');
                            log.Write((precursor_peak_depths.ContainsKey(sn2)
                                ? precursor_peak_depths[sn2].ToString()
                                : "n/a") + '\t');
                            log.Write((precursor_fragmentation_methods.ContainsKey(sn2)
                                ? precursor_fragmentation_methods[sn2]
                                : "n/a") + '\t');
                            log.Write((elapsed_scan_times.ContainsKey(sn2) ? elapsed_scan_times[sn2].ToString() : "n/a") +
                                      '\t');
                            log.Write((ion_injection_times.ContainsKey(sn2) ? ion_injection_times[sn2].ToString() : "n/a") +
                                      '\t');
                            log.WriteLine();
                        }

                        log.Close();

                    }

                    onFinishedFile(new FilepathEventArgs(filepath));
                }

                onFinished(new EventArgs());
            }
            catch (Exception ex)
            {
                onThrowException(new ExceptionEventArgs(ex));
            }
            finally
            {
                if (overall_log != null && IncludeLog)
                {
                    overall_log.Close();
                }
                if (raw != null)
                {
                    raw.Close();
                }
                if (log != null && IncludeLog)
                {
                    log.Close();
                }
                if (dta != null)
                {
                    dta.Close();
                }
                if (txt_outputs != null)
                {
                    foreach (StreamWriter sw in txt_outputs.Values)
                    {
                        if (sw != null)
                        {
                            sw.Close();
                        }
                    }
                    foreach (StreamWriter sw in mgf_outputs.Values)
                    {
                        if (sw != null)
                        {
                            sw.Close();
                        }
                    }
                }
            }
        }

        private static double MassFromMZ(double mz, int charge)
        {
            return mz*Math.Abs(charge) - charge*PROTON_MASS;
        }

        private static double MZFromMass(double mass, int charge)
        {
            return (mass + charge*PROTON_MASS)/Math.Abs(charge);
        }

        private static IEnumerable<int> AllIndicesOf(string s, char c)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c)
                {
                    yield return i;
                }
            }
        }
    }
}