using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DtaGenerator
{
    public class DtaGenerator
    {
        private const double PROTON_MASS = 1.00727638;

        private const double C12_C13_ISOTOPIC_SPACING = 1.0033548378;  // C-13 - C-12

        private const double PEAK_IDENTIFICATION_MASS_TOLERANCE = 0.01;

        private const int LOCK_MASS_ISOTOPIC_PEAKS = 3;

        private const double MASS_TOLERANCE = 5.0;
        private const MassToleranceUnits MASS_TOLERANCE_UNITS = MassToleranceUnits.ppm;

        private const double LOW_PRECURSOR_CLEANING_WINDOW_MZ = 5.0;
        private const double HIGH_PRECURSOR_CLEANING_WINDOW_MZ = 5.0;
        private const double LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA = 60.0;

        private const double MINIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ = 114.0;
        private const double MAXIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ = 117.0;
        private const double ITRAQ_4PLEX_CAD_TAG_MZ = 145.0;
        private const double ITRAQ_4PLEX_CAD_TAG_LOSS_DA = 145.0;

        private const double MINIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ = 101.0;
        private const double MAXIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ = 104.0;
        private const double ITRAQ_4PLEX_ETD_TAG_MZ = 162.0;
        private const double ITRAQ_4PLEX_ETD_REPORTER_LOSS_DA = 102.0;
        private const double ITRAQ_4PLEX_ETD_TAG_LOSS_DA = 161.0;

        private const double ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        private const double MINIMUM_TMT_DUPLEX_CAD_REPORTER_MZ = 126.0;
        private const double MAXIMUM_TMT_DUPLEX_CAD_REPORTER_MZ = 127.0;
        private const double TMT_DUPLEX_CAD_TAG_MZ = 226.0;
        private const double TMT_DUPLEX_CAD_TAG_LOSS_DA = 225.0;

        private const double MINIMUM_TMT_DUPLEX_ETD_REPORTER_MZ = 112.0;
        private const double MAXIMUM_TMT_DUPLEX_ETD_REPORTER_MZ = 114.0;
        private const double TMT_DUPLEX_ETD_TAG_MZ = 226.0;
        private const double TMT_DUPLEX_ETD_REPORTER_LOSS_DA = 113.0;
        private const double TMT_DUPLEX_ETD_TAG_LOSS_DA = 225.0;

        private const double TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

        private const double MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ = 126.0;
        private const double MAXIMUM_TMT_6PLEX_CAD_REPORTER_MZ = 131.0;
        private const double TMT_6PLEX_CAD_TAG_MZ = 230.0;
        private const double TMT_6PLEX_CAD_TAG_LOSS_DA = 229.0;

        private const double MINIMUM_TMT_6PLEX_ETD_REPORTER_MZ = 114.0;
        private const double MAXIMUM_TMT_6PLEX_ETD_REPORTER_MZ = 119.0;
        private const double TMT_6PLEX_ETD_TAG_MZ = 230.0;
        private const double TMT_6PLEX_ETD_REPORTER_LOSS_DA = 114.5;
        private const double TMT_6PLEX_ETD_TAG_LOSS_DA = 229.0;

        private const double TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ = 1.0;
        private const double TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ = 3.0;

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

        private IList<string> rawFilepaths;
        private int minimumAssumedPrecursorChargeState;
        private int maximumAssumedPrecursorChargeState;
        private bool enableLockMassRecalibration;
        private double lockMassMZ;
        private bool kenny;
        private bool enableEtdSpectralPreProcessing;
        private bool cleanItraq4Plex;
        private bool cleanTmtDuplex;
        private bool cleanTmt6Plex;
        private bool enableDeIsotoping;
        private double fragmentIntensityThreshold;
        private IntensityThresholdType fragmentIntensityThresholdType;
        private bool enableTopNPeaksFiltering;
        private int numberOfTopPeaks;
        private TopNPeaksFilteringDomain topNPeaksFilteringDomain;
        private double mzWindow;
        private bool groupByActivationEnergyTime;
        private bool individualDtaOutput;
        private bool mergedTxtOutput;
        private string outputFolder;

        public DtaGenerator(IList<string> rawFilepaths,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            bool enableLockMassRecalibration, double lockMassMZ,
            bool kenny,
            bool enableEtdSpectralPreProcessing,
            bool cleanItraq4Plex, bool cleanTmtDuplex, bool cleanTmt6Plex,
            bool enableDeIsotoping,
            double fragmentIntensityThreshold, IntensityThresholdType fragmentIntensityThresholdType,
            bool enableTopNPeaksFiltering, int numberOfTopPeaks, TopNPeaksFilteringDomain topNPeaksFilteringDomain, double mzWindow,
            bool groupByActivationEnergyTime,
            bool individualDtaOutput, bool mergedTxtOutput,
            string outputFolder)
        {
            this.rawFilepaths = rawFilepaths;
            this.minimumAssumedPrecursorChargeState = minimumAssumedPrecursorChargeState;
            this.maximumAssumedPrecursorChargeState = maximumAssumedPrecursorChargeState;
            this.enableLockMassRecalibration = enableLockMassRecalibration;
            this.lockMassMZ = lockMassMZ;
            this.kenny = kenny;
            this.enableEtdSpectralPreProcessing = enableEtdSpectralPreProcessing;
            this.cleanItraq4Plex = cleanItraq4Plex;
            this.cleanTmtDuplex = cleanTmtDuplex;
            this.cleanTmt6Plex = cleanTmt6Plex;
            this.enableDeIsotoping = enableDeIsotoping;
            this.fragmentIntensityThreshold = fragmentIntensityThreshold;
            this.fragmentIntensityThresholdType = fragmentIntensityThresholdType;
            this.enableTopNPeaksFiltering = enableTopNPeaksFiltering;
            this.numberOfTopPeaks = numberOfTopPeaks;
            this.topNPeaksFilteringDomain = topNPeaksFilteringDomain;
            this.mzWindow = mzWindow;
            this.groupByActivationEnergyTime = groupByActivationEnergyTime;
            this.individualDtaOutput = individualDtaOutput;
            this.mergedTxtOutput = mergedTxtOutput;
            this.outputFolder = outputFolder;
        }

        public void GenerateDtas()
        {
            XRAWFILE2Lib.IXRawfile3 raw = null;
            StreamWriter log = null;
            StreamWriter dta = null;
            Dictionary<string, StreamWriter> merged = null;

            try
            {
                onStarting(new EventArgs());

                string log_folder = Path.Combine(outputFolder, "log");
                if(!Directory.Exists(log_folder))
                {
                    Directory.CreateDirectory(log_folder);
                }

                for(int file_index = 0; file_index < rawFilepaths.Count; file_index++)
                {
                    string filepath = rawFilepaths[file_index];
                    onStartingFile(new FilepathEventArgs(filepath));

                    merged = new Dictionary<string, StreamWriter>();
                    SortedDictionary<string, int> spectrum_counts = new SortedDictionary<string, int>();
                    SortedDictionary<string, int> dta_counts = new SortedDictionary<string, int>();
                    SortedDictionary<int, double> retention_times = new SortedDictionary<int, double>();
                    SortedDictionary<int, double> elapsed_scan_times = new SortedDictionary<int, double>();
                    SortedDictionary<int, double> ion_injection_times = new SortedDictionary<int, double>();
                    SortedDictionary<int, double?> precursor_sns = new SortedDictionary<int, double?>();
                    SortedDictionary<int, int> precursor_peak_depths = new SortedDictionary<int, int>();

                    raw = (XRAWFILE2Lib.IXRawfile3)new XRAWFILE2Lib.XRawfile();
                    raw.Open(filepath);
                    raw.SetCurrentController(0, 1);

                    log = new StreamWriter(Path.Combine(log_folder, Path.GetFileNameWithoutExtension(filepath) + "_log.txt"));
                    log.AutoFlush = true;

                    log.WriteLine("DTA Generator PARAMETERS");
                    log.WriteLine("Assumed Precursor Charge State Range: " + minimumAssumedPrecursorChargeState.ToString() + '-' + maximumAssumedPrecursorChargeState.ToString());
                    log.WriteLine("Lock Mass Recalibration: " + enableLockMassRecalibration.ToString());
                    if(enableLockMassRecalibration)
                    {
                        log.WriteLine("Lock Mass m/z: " + lockMassMZ.ToString());
                    }
                    log.WriteLine("Kenny: " + kenny.ToString());
                    log.WriteLine("Enable ETD Spectral Pre-Processing: " + enableEtdSpectralPreProcessing.ToString());
                    log.WriteLine("Clean iTRAQ 4-Plex: " + cleanItraq4Plex.ToString());
                    log.WriteLine("Clean TMT Duplex: " + cleanTmtDuplex.ToString());
                    log.WriteLine("Clean TMT 6-Plex: " + cleanTmt6Plex.ToString());
                    log.WriteLine("Enable De-Isotoping: " + enableDeIsotoping.ToString());
                    log.WriteLine("Fragment Ion Intensity Threshold: " + fragmentIntensityThreshold.ToString() + " (" + fragmentIntensityThresholdType.ToString() + ')');
                    log.WriteLine("Enable Top N Peak Filtering: " + enableTopNPeaksFiltering.ToString());
                    if(enableTopNPeaksFiltering)
                    {
                        log.Write("Top N Peaks Filtering: " + numberOfTopPeaks.ToString() + " per ");
                        switch(topNPeaksFilteringDomain)
                        {
                            case TopNPeaksFilteringDomain.PerMZWindow:
                                log.WriteLine(mzWindow.ToString() + " m/z");
                                break;
                            case TopNPeaksFilteringDomain.PerSpectrum:
                                log.WriteLine(" Spectrum");
                                break;
                        }
                    }
                    log.WriteLine();

                    int first_scan_number = -1;
                    raw.GetFirstSpectrumNumber(ref first_scan_number);
                    int last_scan_number = -1;
                    raw.GetLastSpectrumNumber(ref last_scan_number);
                    for(int scan_number = first_scan_number; scan_number <= last_scan_number; scan_number++)
                    {
                        string scan_filter = null;
                        raw.GetFilterForScanNum(scan_number, ref scan_filter);

                        if(scan_filter.Contains("@"))
                        {
                            double time = -1.0;
                            raw.RTFromScanNum(scan_number, ref time);
                            retention_times.Add(scan_number, time);

                            string precursor_mz_scan_filter = scan_filter.Substring(0, scan_filter.LastIndexOf('@'));
                            double precursor_mz = double.Parse(precursor_mz_scan_filter.Substring(precursor_mz_scan_filter.LastIndexOf(' ') + 1));

                            string low_mz_scan_filter = scan_filter.Substring(scan_filter.IndexOf('[') + 1);
                            double low_mz = double.Parse(low_mz_scan_filter.Substring(0, low_mz_scan_filter.IndexOf('-')));
                            string high_mz_scan_filter = scan_filter.Substring(scan_filter.LastIndexOf('-') + 1);
                            double high_mz = double.Parse(high_mz_scan_filter.Substring(0, high_mz_scan_filter.IndexOf(']')));

                            int precursor_scan_number = scan_number;
                            bool no_precursor_scan = false;
                            string precursor_scan_filter = null;
                            do
                            {
                                precursor_scan_number--;
                                if(precursor_scan_number < first_scan_number)
                                {
                                    no_precursor_scan = true;
                                    break;
                                }
                                precursor_scan_filter = null;
                                raw.GetFilterForScanNum(precursor_scan_number, ref precursor_scan_filter);
                            } while(!precursor_scan_filter.Contains(" ms "));

                            if(!no_precursor_scan)
                            {
                                object precursor_labels = null;
                                object precursor_flags = null;
                                raw.GetLabelData(ref precursor_labels, ref precursor_flags, ref precursor_scan_number);

                                double[,] precursor_data = (double[,])precursor_labels;

                                if(precursor_data.Length == 0)
                                {
                                    double centroid_peak_width = -1.0;
                                    precursor_labels = null;
                                    precursor_flags = null;
                                    int mass_list_array_size = -1;
                                    raw.GetMassListFromScanNum(ref precursor_scan_number, null, 0, 0, 0, 1, ref centroid_peak_width, ref precursor_labels, ref precursor_flags, ref mass_list_array_size);
                                    precursor_data = (double[,])precursor_labels;
                                }

                                int? precursor_index = null;
                                for(int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                                {
                                    if(Math.Abs(precursor_data[(int)RawLabelDataColumn.MZ, i] - precursor_mz) <= PEAK_IDENTIFICATION_MASS_TOLERANCE)
                                    {
                                        if(!precursor_index.HasValue || precursor_data[(int)RawLabelDataColumn.Intensity, i] > precursor_data[(int)RawLabelDataColumn.Intensity, precursor_index.Value])
                                        {
                                            precursor_index = i;
                                        }
                                    }
                                }

                                if(!precursor_index.HasValue)
                                {
                                    for(int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                                    {
                                        if(!precursor_index.HasValue || Math.Abs(precursor_data[(int)RawLabelDataColumn.MZ, i] - precursor_mz) < Math.Abs(precursor_data[(int)RawLabelDataColumn.MZ, precursor_index.Value] - precursor_mz))
                                        {
                                            precursor_index = i;
                                        }
                                    }
                                }

                                if(!precursor_index.HasValue)
                                {
                                    throw new Exception("Could not find precursor in MS^1 scan number " + precursor_scan_number.ToString());
                                }

                                double precursor_mass_recalibration = 1.0;
                                if(enableLockMassRecalibration)
                                {
                                    double? precursor_lockmass_mz = null;
                                    for(int precursor_data_i = precursor_data.GetLowerBound(1); precursor_data_i <= precursor_data.GetUpperBound(1); precursor_data_i++)
                                    {
                                        if(!precursor_lockmass_mz.HasValue ||
                                            Math.Abs(precursor_data[(int)RawLabelDataColumn.MZ, precursor_data_i] - lockMassMZ) <
                                            Math.Abs(precursor_lockmass_mz.Value - lockMassMZ))
                                        {
                                            precursor_lockmass_mz = precursor_data[(int)RawLabelDataColumn.MZ, precursor_data_i];
                                        }
                                    }

                                    if(!precursor_lockmass_mz.HasValue || Math.Abs(precursor_lockmass_mz.Value - lockMassMZ) > PEAK_IDENTIFICATION_MASS_TOLERANCE)
                                    {
                                        log.WriteLine("Could not find lock mass in MS^1 scan number " + precursor_scan_number.ToString());
                                    }
                                    else
                                    {
                                        precursor_mass_recalibration = lockMassMZ / precursor_lockmass_mz.Value;
                                    }
                                }

                                precursor_mz = precursor_mass_recalibration * precursor_data[(int)RawLabelDataColumn.MZ, precursor_index.Value];

                                if(precursor_data.GetLength(0) > 2)
                                {
                                    precursor_sns.Add(scan_number, (precursor_data[(int)RawLabelDataColumn.Intensity, precursor_index.Value] - precursor_data[(int)RawLabelDataColumn.NoiseBaseline, precursor_index.Value]) / precursor_data[(int)RawLabelDataColumn.NoiseLevel, precursor_index.Value]);
                                }
                                else
                                {
                                    precursor_sns.Add(scan_number, null);
                                }

                                int peak_depth = 1;
                                for(int i = precursor_data.GetLowerBound(1); i <= precursor_data.GetUpperBound(1); i++)
                                {
                                    if(i != precursor_index.Value)
                                    {
                                        if(precursor_data[(int)RawLabelDataColumn.Intensity, i] > precursor_data[(int)RawLabelDataColumn.Intensity, precursor_index.Value])
                                        {
                                            peak_depth++;
                                        }
                                    }
                                }

                                precursor_peak_depths.Add(scan_number, peak_depth);
                            }

                            object header_labels = null;
                            object header_values = null;
                            int array_size = -1;
                            raw.GetTrailerExtraForScanNum(scan_number, ref header_labels, ref header_values, ref array_size);
                            string[] header_label_strings = (string[])header_labels;
                            string[] header_value_strings = (string[])header_values;
                            int charge = 0;
                            if(header_label_strings != null && header_value_strings != null)
                            {
                                for(int header_i = header_label_strings.GetLowerBound(0); header_i <= header_label_strings.GetUpperBound(0); header_i++)
                                {
                                    if(header_label_strings[header_i].StartsWith("Charge"))
                                    {
                                        charge = int.Parse(header_value_strings[header_i]);
                                    }
                                    else if(header_label_strings[header_i].StartsWith("Elapsed Scan Time (sec)"))
                                    {
                                        elapsed_scan_times.Add(scan_number, double.Parse(header_value_strings[header_i]));
                                    }
                                    else if(header_label_strings[header_i].StartsWith("Ion Injection Time (ms)"))
                                    {
                                        ion_injection_times.Add(scan_number, double.Parse(header_value_strings[header_i]));
                                    }
                                }
                            }

                            object labels = null;
                            object flags = null;
                            raw.GetLabelData(ref labels, ref flags, ref scan_number);

                            double[,] data = (double[,])labels;
                            if(data.Length == 0)
                            {
                                double centroid_peak_width = -1.0;
                                labels = null;
                                flags = null;
                                int mass_list_array_size = -1;
                                raw.GetMassListFromScanNum(ref scan_number, null, 0, 0, 0, 1, ref centroid_peak_width, ref labels, ref flags, ref mass_list_array_size);
                                data = (double[,])labels;
                            }

                            double? lockmass_mz = null;
                            double total_ion_current = 0.0;
                            double base_peak_mz = -1.0;
                            double base_peak_intensity = -1.0;
                            for(int data_i = data.GetLowerBound(1); data_i <= data.GetUpperBound(1); data_i++)
                            {
                                if(enableLockMassRecalibration)
                                {
                                    if(!lockmass_mz.HasValue ||
                                        Math.Abs(data[(int)RawLabelDataColumn.MZ, data_i] - lockMassMZ) <
                                        Math.Abs(lockmass_mz.Value - lockMassMZ))
                                    {
                                        lockmass_mz = data[(int)RawLabelDataColumn.MZ, data_i];
                                    }
                                }

                                total_ion_current += data[(int)RawLabelDataColumn.Intensity, data_i];

                                if(base_peak_mz < 0.0 ||
                                    data[(int)RawLabelDataColumn.Intensity, data_i] > base_peak_intensity)
                                {
                                    base_peak_mz = data[(int)RawLabelDataColumn.MZ, data_i];
                                    base_peak_intensity = data[(int)RawLabelDataColumn.Intensity, data_i];
                                }
                            }

                            double mass_recalibration_factor = 1.0;
                            if(enableLockMassRecalibration)
                            {
                                if(!lockmass_mz.HasValue || Math.Abs(lockmass_mz.Value - lockMassMZ) > PEAK_IDENTIFICATION_MASS_TOLERANCE)
                                {
                                    log.WriteLine("Could not find lock mass in MS^2 scan number " + scan_number.ToString());
                                }
                                else
                                {
                                    mass_recalibration_factor = lockMassMZ / lockmass_mz.Value;
                                    base_peak_mz *= mass_recalibration_factor;
                                }
                            }

                            List<int> charges = new List<int>();
                            if(charge == 0)
                            {
                                for(int assumed_charge_state = minimumAssumedPrecursorChargeState; assumed_charge_state <= maximumAssumedPrecursorChargeState; assumed_charge_state++)
                                {
                                    charges.Add(assumed_charge_state);
                                }
                            }
                            else
                            {
                                charges.Add(charge);
                            }

                            string mass_analyzer = scan_filter.Substring(0, 4).ToUpper();

                            string fragmentation_method;
                            if(groupByActivationEnergyTime)
                            {
                                string temp_scan_filter = scan_filter.Substring(scan_filter.IndexOf('@') + 1);
                                temp_scan_filter = temp_scan_filter.Substring(0, temp_scan_filter.IndexOf(' '));
                                fragmentation_method = temp_scan_filter.ToUpper();
                                if(kenny && scan_filter.Contains("det=2.00"))
                                {
                                    fragmentation_method = fragmentation_method.Remove(0, 3);
                                    fragmentation_method = "ETD" + fragmentation_method;
                                }
                            }
                            else
                            {
                                if(kenny && scan_filter.Contains("det=2.00"))
                                {
                                    fragmentation_method = "ETD";
                                }
                                else
                                {
                                    fragmentation_method = scan_filter.Substring(scan_filter.IndexOf('@') + 1, 3).ToUpper();
                                }
                            }

                            if(!spectrum_counts.ContainsKey(mass_analyzer))
                            {
                                spectrum_counts.Add(mass_analyzer, 0);
                            }
                            spectrum_counts[mass_analyzer]++;

                            if(!dta_counts.ContainsKey(mass_analyzer))
                            {
                                dta_counts.Add(mass_analyzer, 0);
                            }
                            dta_counts[mass_analyzer] += charges.Count;

                            if(!spectrum_counts.ContainsKey(fragmentation_method))
                            {
                                spectrum_counts.Add(fragmentation_method, 0);
                            }
                            spectrum_counts[fragmentation_method]++;

                            if(!dta_counts.ContainsKey(fragmentation_method))
                            {
                                dta_counts.Add(fragmentation_method, 0);
                            }
                            dta_counts[fragmentation_method] += charges.Count;

                            if(!spectrum_counts.ContainsKey(mass_analyzer + ' ' + fragmentation_method))
                            {
                                spectrum_counts.Add(mass_analyzer + ' ' + fragmentation_method, 0);
                            }
                            spectrum_counts[mass_analyzer + ' ' + fragmentation_method]++;

                            if(!dta_counts.ContainsKey(mass_analyzer + ' ' + fragmentation_method))
                            {
                                dta_counts.Add(mass_analyzer + ' ' + fragmentation_method, 0);
                            }
                            dta_counts[mass_analyzer + ' ' + fragmentation_method] += charges.Count;

                            if(individualDtaOutput || mergedTxtOutput)
                            {
                                StringBuilder dta_header_content_sb = new StringBuilder();

                                List<MSPeak> all_peaks = new List<MSPeak>();
                                for(int data_i = data.GetLowerBound(1); data_i <= data.GetUpperBound(1); data_i++)
                                {
                                    double mz = data[(int)RawLabelDataColumn.MZ, data_i];
                                    if(enableLockMassRecalibration)
                                    {
                                        mz *= mass_recalibration_factor;
                                    }

                                    if(enableLockMassRecalibration)
                                    {
                                        bool lock_mass_peak = false;

                                        for(int i = 0; i < LOCK_MASS_ISOTOPIC_PEAKS; i++)
                                        {
                                            if(Math.Abs((mz - (lockMassMZ + i * C12_C13_ISOTOPIC_SPACING)) / (lockMassMZ + i * C12_C13_ISOTOPIC_SPACING) * 1e6) <= MASS_TOLERANCE)
                                            {
                                                lock_mass_peak = true;
                                                break;
                                            }
                                        }

                                        if(lock_mass_peak)
                                        {
                                            continue;
                                        }
                                    }

                                    if(enableDeIsotoping && mass_analyzer == "FTMS" && isIsotopicPeak(data, data_i, MASS_TOLERANCE, MASS_TOLERANCE_UNITS))
                                    {
                                        continue;
                                    }

                                    if(data.GetLength(0) == Enum.GetValues(typeof(RawLabelDataColumn)).Length)
                                    {
                                        all_peaks.Add(new HighResolutionMSPeak(mz, data[(int)RawLabelDataColumn.Intensity, data_i], (double)data[(int)RawLabelDataColumn.Resolution, data_i], (float)data[(int)RawLabelDataColumn.NoiseBaseline, data_i], (float)data[(int)RawLabelDataColumn.NoiseLevel, data_i], (int)data[(int)RawLabelDataColumn.Charge, data_i]));
                                    }
                                    else
                                    {
                                        all_peaks.Add(new MSPeak(mz, data[(int)RawLabelDataColumn.Intensity, data_i]));
                                    }
                                }

                                foreach(int charge_i in charges)
                                {
                                    double precursor_mass = massFromMZ(precursor_mz, charge_i);

                                    StringBuilder dta_content_sb = new StringBuilder(dta_header_content_sb.ToString());

                                    dta_content_sb.AppendLine((precursor_mass + PROTON_MASS).ToString("0.00000") + ' ' + charge_i.ToString());

                                    List<MSPeak> peaks = new List<MSPeak>(all_peaks);
                                    if(enableEtdSpectralPreProcessing && fragmentation_method.StartsWith("ETD") || fragmentation_method.StartsWith("ECD"))
                                    {
                                        int p1 = 0;
                                        while(p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            bool clean = false;
                                            if(mz >= precursor_mz - LOW_PRECURSOR_CLEANING_WINDOW_MZ && mz <= precursor_mz + HIGH_PRECURSOR_CLEANING_WINDOW_MZ)
                                            {
                                                clean = true;
                                            }

                                            if(!clean)
                                            {
                                                for(int reduced_precursor_charge = 1; reduced_precursor_charge <= charge_i - 1; reduced_precursor_charge++)
                                                {
                                                    if(mz >= mzFromMass(precursor_mass - LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA, reduced_precursor_charge) &&
                                                        mz < mzFromMass(precursor_mass, reduced_precursor_charge) + HIGH_PRECURSOR_CLEANING_WINDOW_MZ)
                                                    {
                                                        clean = true;
                                                        break;
                                                    }
                                                }
                                            }

                                            if(clean)
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                    if(fragmentation_method.StartsWith("CID") && true)
                                    {
                                        int p1 = 0;
                                        while(p1 < peaks.Count)
                                        {
                                            double mz = peaks[p1].MZ;

                                            if(mz >= precursor_mz - 98 / charge_i - 5.0 && mz <= precursor_mz - 98 / charge_i + 5.0)
                                            {
                                                peaks.RemoveAt(p1);
                                            }
                                            else
                                            {
                                                p1++;
                                            }
                                        }
                                    }
                                    if(cleanItraq4Plex)
                                    {
                                        if(fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") || fragmentation_method.StartsWith("HCD"))
                                        {
                                            double precursor_itraq_4plex_tag_cleaning_mz = precursor_mz * charge_i - ITRAQ_4PLEX_CAD_TAG_LOSS_DA;

                                            int p1 = 0;
                                            while(p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;

                                                if((mz >= MINIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= MAXIMUM_ITRAQ_4PLEX_CAD_REPORTER_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    || (mz >= ITRAQ_4PLEX_CAD_TAG_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= ITRAQ_4PLEX_CAD_TAG_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    || (mz >= precursor_itraq_4plex_tag_cleaning_mz - ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= precursor_itraq_4plex_tag_cleaning_mz + ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                                                {
                                                    peaks.RemoveAt(p1);
                                                }
                                                else
                                                {
                                                    p1++;
                                                }
                                            }
                                        }
                                        else if(fragmentation_method.StartsWith("ETD"))
                                        {
                                            double precursor_itraq_4plex_reporter_loss_cleaning_mz = precursor_mz * charge_i - ITRAQ_4PLEX_ETD_REPORTER_LOSS_DA;
                                            double precursor_itraq_4plex_tag_loss_cleaning_mz = precursor_mz * charge_i - ITRAQ_4PLEX_ETD_TAG_LOSS_DA;

                                            int p1 = 0;
                                            while(p1 < peaks.Count)
                                            {
                                                double mz = peaks[p1].MZ;
                                                if((mz >= MINIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= MAXIMUM_ITRAQ_4PLEX_ETD_REPORTER_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    || (mz >= ITRAQ_4PLEX_ETD_TAG_MZ - ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= ITRAQ_4PLEX_ETD_TAG_MZ + ITRAQ_4PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                    || (mz >= precursor_itraq_4plex_reporter_loss_cleaning_mz - ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= precursor_itraq_4plex_reporter_loss_cleaning_mz + ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                    || (mz >= precursor_itraq_4plex_tag_loss_cleaning_mz - ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                    && mz <= precursor_itraq_4plex_tag_loss_cleaning_mz + ITRAQ_4PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
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
                                    if(cleanTmtDuplex)
                                    {
                                        if(fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") || fragmentation_method.StartsWith("HCD"))
                                        {
                                            for(int reduced_charge_i = charge_i - 1; reduced_charge_i >= 1; reduced_charge_i--)
                                            {
                                                double precursor_tmt_duplex_tag_cleaning_mz = precursor_mz * reduced_charge_i - TMT_DUPLEX_CAD_TAG_LOSS_DA / reduced_charge_i;

                                                int p1 = 0;
                                                while(p1 < peaks.Count)
                                                {
                                                    double mz = peaks[p1].MZ;

                                                    if((mz >= MINIMUM_TMT_DUPLEX_CAD_REPORTER_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= MAXIMUM_TMT_DUPLEX_CAD_REPORTER_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= TMT_DUPLEX_CAD_TAG_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= TMT_DUPLEX_CAD_TAG_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_duplex_tag_cleaning_mz - TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_duplex_tag_cleaning_mz + TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
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
                                        else if(fragmentation_method.StartsWith("ETD"))
                                        {
                                            for(int reduced_charge_i = charge_i - 1; reduced_charge_i >= 1; reduced_charge_i--)
                                            {
                                                double precursor_tmt_duplex_reporter_loss_cleaning_mz = precursor_mz * charge_i - TMT_DUPLEX_ETD_REPORTER_LOSS_DA / reduced_charge_i;
                                                double precursor_tmt_duplex_tag_loss_cleaning_mz = precursor_mz * charge_i - TMT_DUPLEX_ETD_TAG_LOSS_DA / reduced_charge_i;

                                                int p1 = 0;
                                                while(p1 < peaks.Count)
                                                {
                                                    double mz = peaks[p1].MZ;
                                                    if((mz >= MINIMUM_TMT_DUPLEX_ETD_REPORTER_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= MAXIMUM_TMT_DUPLEX_ETD_REPORTER_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= TMT_DUPLEX_ETD_TAG_MZ - TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= TMT_DUPLEX_ETD_TAG_MZ + TMT_DUPLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_duplex_reporter_loss_cleaning_mz - TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_duplex_reporter_loss_cleaning_mz + TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_duplex_tag_loss_cleaning_mz - TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_duplex_tag_loss_cleaning_mz + TMT_DUPLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
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
                                    if(cleanTmt6Plex)
                                    {
                                        if(fragmentation_method.StartsWith("CID") || fragmentation_method.StartsWith("PQD") || fragmentation_method.StartsWith("HCD"))
                                        {
                                            for(int reduced_charge_i = charge_i - 1; reduced_charge_i >= 1; reduced_charge_i--)
                                            {
                                                double precursor_tmt_6plex_tag_cleaning_mz = precursor_mz * reduced_charge_i - TMT_6PLEX_CAD_TAG_LOSS_DA / reduced_charge_i;

                                                int p1 = 0;
                                                while(p1 < peaks.Count)
                                                {
                                                    double mz = peaks[p1].MZ;

                                                    if((mz >= MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= MAXIMUM_TMT_6PLEX_CAD_REPORTER_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= TMT_6PLEX_CAD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= TMT_6PLEX_CAD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_6plex_tag_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_6plex_tag_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
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
                                        else if(fragmentation_method.StartsWith("ETD"))
                                        {
                                            for(int reduced_charge_i = charge_i - 1; reduced_charge_i >= 1; reduced_charge_i--)
                                            {
                                                double precursor_tmt_6plex_reporter_loss_cleaning_mz = precursor_mz * charge_i - TMT_6PLEX_ETD_REPORTER_LOSS_DA / reduced_charge_i;
                                                double precursor_tmt_6plex_tag_loss_cleaning_mz = precursor_mz * charge_i - TMT_6PLEX_ETD_TAG_LOSS_DA / reduced_charge_i;

                                                int p1 = 0;
                                                while(p1 < peaks.Count)
                                                {
                                                    double mz = peaks[p1].MZ;
                                                    if((mz >= MINIMUM_TMT_6PLEX_ETD_REPORTER_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= MAXIMUM_TMT_6PLEX_ETD_REPORTER_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= TMT_6PLEX_ETD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= TMT_6PLEX_ETD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_6plex_reporter_loss_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_6plex_reporter_loss_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                                                        || (mz >= precursor_tmt_6plex_tag_loss_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                                                        && mz <= precursor_tmt_6plex_tag_loss_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
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

                                    MSPeak base_peak = null;
                                    foreach(MSPeak peak in peaks)
                                    {
                                        if(base_peak == null || peak.Intensity > base_peak.Intensity)
                                        {
                                            base_peak = peak;
                                        }
                                    }

                                    double fragment_intensity_threshold;
                                    if(fragmentIntensityThresholdType == IntensityThresholdType.Relative)
                                    {
                                        fragment_intensity_threshold = (fragmentIntensityThreshold / 100.0) * base_peak.Intensity;
                                    }
                                    else
                                    {
                                        fragment_intensity_threshold = fragmentIntensityThreshold;
                                    }

                                    int p2 = 0;
                                    while(p2 < peaks.Count)
                                    {
                                        MSPeak peak = peaks[p2];

                                        if(fragmentIntensityThresholdType == IntensityThresholdType.SignalToNoiseRatio && !(peak is HighResolutionMSPeak))
                                        {
                                            throw new InvalidOperationException("Signal-to-noise ratio thresholds may only be used with FT data");
                                        }

                                        if((fragmentIntensityThresholdType != IntensityThresholdType.SignalToNoiseRatio && peak.Intensity < fragment_intensity_threshold) ||
                                            (fragmentIntensityThresholdType == IntensityThresholdType.SignalToNoiseRatio && peak is HighResolutionMSPeak &&
                                            ((HighResolutionMSPeak)peak).CalculateSignalToNoiseRatio() < fragment_intensity_threshold))
                                        {
                                            peaks.RemoveAt(p2);
                                        }
                                        else
                                        {
                                            p2++;
                                        }
                                    }

                                    if(enableTopNPeaksFiltering && peaks.Count > 0)
                                    {
                                        if(topNPeaksFilteringDomain == TopNPeaksFilteringDomain.PerSpectrum)
                                        {
                                            if(peaks.Count > numberOfTopPeaks)
                                            {
                                                if(peaks[0] is HighResolutionMSPeak)
                                                {
                                                    peaks.Sort(new DescendingSignalToNoiseRatioComparer());
                                                }
                                                else
                                                {
                                                    peaks.Sort(new DescendingIntensityComparer());
                                                }

                                                peaks.RemoveRange(numberOfTopPeaks, peaks.Count - numberOfTopPeaks);
                                            }
                                        }
                                        else if(topNPeaksFilteringDomain == TopNPeaksFilteringDomain.PerMZWindow)
                                        {
                                            int num_mz_windows = (int)Math.Ceiling((peaks[peaks.Count - 1].MZ - peaks[0].MZ) / mzWindow);
                                            if(num_mz_windows == 0)
                                            {
                                                num_mz_windows++;
                                            }
                                            List<List<MSPeak>> mz_windows = new List<List<MSPeak>>(num_mz_windows);
                                            for(int i = 0; i < num_mz_windows; i++)
                                            {
                                                mz_windows.Add(new List<MSPeak>());
                                            }

                                            foreach(MSPeak peak in peaks)
                                            {
                                                int segment = (int)Math.Floor((peak.MZ - peaks[0].MZ) / mzWindow);

                                                mz_windows[segment].Add(peak);
                                            }

                                            for(int i = 0; i < mz_windows.Count; i++)
                                            {
                                                List<MSPeak> mz_window_peaks = mz_windows[i];

                                                int num_top_peaks = numberOfTopPeaks;
                                                if(i == mz_windows.Count - 1)
                                                {
                                                    double position = (peaks[peaks.Count - 1].MZ - peaks[0].MZ) / mzWindow;
                                                    double offset = position - Math.Floor(position);
                                                    num_top_peaks = (int)Math.Round(offset * numberOfTopPeaks);
                                                }

                                                if(mz_window_peaks.Count > numberOfTopPeaks)
                                                {
                                                    if(peaks[0] is HighResolutionMSPeak)
                                                    {
                                                        peaks.Sort(new DescendingSignalToNoiseRatioComparer());
                                                    }
                                                    else
                                                    {
                                                        peaks.Sort(new DescendingIntensityComparer());
                                                    }

                                                    mz_window_peaks.RemoveRange(numberOfTopPeaks, mz_window_peaks.Count - numberOfTopPeaks);
                                                }
                                            }

                                            peaks.Clear();

                                            foreach(List<MSPeak> mz_window_peaks in mz_windows)
                                            {
                                                foreach(MSPeak peak in mz_window_peaks)
                                                {
                                                    peaks.Add(peak);
                                                }
                                            }
                                        }

                                        peaks.Sort(new AscendingMZComparer());
                                    }

                                    foreach(MSPeak peak in peaks)
                                    {
                                        dta_content_sb.Append(' ' + peak.MZ.ToString("0.00000") + ' ' +
                                            peak.Intensity.ToString("0.00"));

                                        dta_content_sb.AppendLine();
                                    }

                                    string dta_filepath = Path.GetFileNameWithoutExtension(filepath) +
                                        '.' + mass_analyzer + '.' + fragmentation_method +
                                        '.' + scan_number.ToString() + '.' +
                                        scan_number.ToString() + '.' +
                                        charge_i.ToString() + ".dta";

                                    if(individualDtaOutput)
                                    {
                                        dta = new StreamWriter(Path.Combine(outputFolder, dta_filepath));

                                        if(dta_content_sb.Length > 0)
                                        {
                                            dta.Write(dta_content_sb.ToString());
                                        }

                                        dta.Close();
                                    }

                                    if(mergedTxtOutput)
                                    {
                                        string base_output_filename = Path.GetFileNameWithoutExtension(filepath)
                                            + '_' + mass_analyzer + '_' + fragmentation_method;

                                        string output_filepath = Path.Combine(outputFolder,
                                            base_output_filename + ".txt");

                                        if(!merged.ContainsKey(output_filepath))
                                        {
                                            merged.Add(output_filepath, new StreamWriter(output_filepath));
                                        }

                                        StreamWriter merged_sw = merged[output_filepath];

                                        merged_sw.WriteLine("<dta id=\"" + scan_number.ToString() + "\" name=\"" + dta_filepath + "\">");
                                        merged_sw.WriteLine();

                                        if(dta_content_sb.Length > 0)
                                        {
                                            merged_sw.Write(dta_content_sb.ToString());
                                        }

                                        merged_sw.WriteLine();
                                        merged_sw.WriteLine();
                                    }
                                }
                            }
                        }

                        double progress = (double)file_index / rawFilepaths.Count;
                        progress += (double)(scan_number - first_scan_number + 1) /
                            (last_scan_number - first_scan_number + 1) / rawFilepaths.Count;
                        onUpdateProgress(new ProgressEventArgs((int)(progress * 100)));
                    }

                    raw.Close();

                    if(merged != null)
                    {
                        foreach(StreamWriter sw in merged.Values)
                        {
                            if(merged != null)
                            {
                                sw.Close();
                            }
                        }
                    }

                    log.WriteLine();

                    log.WriteLine("Spectrum Type\tNumber of Scans");
                    foreach(KeyValuePair<string, int> kvp in spectrum_counts)
                    {
                        log.WriteLine(kvp.Key + '\t' + kvp.Value.ToString());
                    }

                    log.WriteLine();

                    log.WriteLine("Spectrum Type\tNumber of DTAs");
                    foreach(KeyValuePair<string, int> kvp in dta_counts)
                    {
                        log.WriteLine(kvp.Key + '\t' + kvp.Value.ToString());
                    }

                    log.WriteLine();

                    double? min_elapsed = null;
                    double? max_elapsed = null;
                    double mean_elapsed = 0.0;
                    double? min_injection = null;
                    double? max_injection = null;
                    double mean_injection = 0.0;
                    foreach(int sn in retention_times.Keys)
                    {
                        double elapsed_scan_time = elapsed_scan_times[sn];
                        double ion_injection_time = ion_injection_times[sn];

                        if(!min_elapsed.HasValue || elapsed_scan_time < min_elapsed)
                        {
                            min_elapsed = elapsed_scan_time;
                        }
                        if(!min_injection.HasValue || ion_injection_time < min_injection)
                        {
                            min_injection = ion_injection_time;
                        }

                        if(!max_elapsed.HasValue || elapsed_scan_time > max_elapsed)
                        {
                            max_elapsed = elapsed_scan_time;
                        }
                        if(!max_injection.HasValue || ion_injection_time > max_injection)
                        {
                            max_injection = ion_injection_time;
                        }

                        mean_elapsed += elapsed_scan_time;
                        mean_injection += ion_injection_time;
                    }

                    mean_elapsed /= elapsed_scan_times.Count;
                    mean_injection /= ion_injection_times.Count;

                    if(min_elapsed.HasValue)
                    {
                        log.WriteLine("Minimum Fragmentation Elapsed Scan Time (sec): " + min_elapsed.Value.ToString());
                    }
                    if(max_elapsed.HasValue)
                    {
                        log.WriteLine("Maximum Fragmentation Elapsed Scan Time (sec): " + max_elapsed.Value.ToString());
                    }
                    if(!Double.IsNaN(mean_elapsed))
                    {
                        log.WriteLine("Average Fragmentation Elapsed Scan Time (sec): " + Math.Round(mean_elapsed, 2).ToString());
                    }

                    log.WriteLine();

                    if(min_injection.HasValue)
                    {
                        log.WriteLine("Minimum Fragmentation Ion Injection Time (msec): " + min_injection.Value.ToString());
                    }
                    if(max_injection.HasValue)
                    {
                        log.WriteLine("Maximum Fragmentation Ion Injection Time (msec): " + max_injection.Value.ToString());
                    }
                    if(!Double.IsNaN(mean_injection))
                    {
                        log.WriteLine("Average Fragmentation Ion Injection Time (msec): " + Math.Round(mean_injection, 3).ToString());
                    }

                    log.WriteLine();
                    log.WriteLine("Fragmentation Scan Number\tRetention Time (min.)\tElapsed Scan Time (sec)\tIon Injection Time (msec)\tPrecursor S/N Ratio\tPrecursor Peak Depth");

                    foreach(int sn2 in retention_times.Keys)
                    {
                        log.WriteLine(sn2.ToString() + '\t' + retention_times[sn2].ToString("0.00") + '\t' + elapsed_scan_times[sn2].ToString() + '\t' + ion_injection_times[sn2].ToString() + '\t' + (precursor_sns.ContainsKey(sn2) && precursor_sns[sn2].HasValue ? precursor_sns[sn2].Value.ToString() : "n/a") + '\t' + (precursor_peak_depths.ContainsKey(sn2) ? precursor_peak_depths[sn2].ToString() : "n/a"));
                    }

                    log.Close();

                    onFinishedFile(new FilepathEventArgs(filepath));
                }

                onFinished(new EventArgs());
            }
            catch(Exception ex)
            {
                onThrowException(new ExceptionEventArgs(ex));
            }
            finally
            {
                if(raw != null)
                {
                    raw.Close();
                }
                if(log != null)
                {
                    log.Close();
                }
                if(dta != null)
                {
                    dta.Close();
                }
                if(merged != null)
                {
                    foreach(StreamWriter sw in merged.Values)
                    {
                        if(sw != null)
                        {
                            sw.Close();
                        }
                    }
                }
            }
        }

        private static double massFromMZ(double mz, int charge)
        {
            return mz * Math.Abs(charge) - charge * PROTON_MASS;
        }

        private static double mzFromMass(double mass, int charge)
        {
            return (mass + charge * PROTON_MASS) / Math.Abs(charge);
        }

        private const double MINIMUM_ISOTOPIC_SPACING = 0.99640822;  // S-34 - S-33
        private const double MAXIMUM_ISOTOPIC_SPACING = 1.0062767459;  // H-2 - H-1

        private static bool isIsotopicPeak(double[,] labelData, int index, double massTolerance, MassToleranceUnits massToleranceUnits)
        {
            int charge = (int)labelData[(int)RawLabelDataColumn.Charge, index] == 0 ? 1 : (int)labelData[(int)RawLabelDataColumn.Charge, index];

            double minimum_mz = labelData[(int)RawLabelDataColumn.MZ, index] - MAXIMUM_ISOTOPIC_SPACING / charge;
            double maximum_mz = labelData[(int)RawLabelDataColumn.MZ, index] - MINIMUM_ISOTOPIC_SPACING / charge;
            switch(massToleranceUnits)
            {
                case MassToleranceUnits.Da:
                    minimum_mz -= massTolerance;
                    maximum_mz += massTolerance;
                    break;
                case MassToleranceUnits.mmu:
                    minimum_mz -= massTolerance / 1000.0;
                    maximum_mz += massTolerance / 1000.0;
                    break;
                case MassToleranceUnits.ppm:
                    minimum_mz -= massTolerance * minimum_mz / 1e6;
                    maximum_mz += massTolerance * maximum_mz / 1e6;
                    break;
            }

            int i = index;
            while(i > labelData.GetLowerBound(1) && labelData[(int)RawLabelDataColumn.MZ, i] >= minimum_mz)
            {
                i--;
                if(labelData[(int)RawLabelDataColumn.MZ, i] >= minimum_mz &&
                    labelData[(int)RawLabelDataColumn.MZ, i] <= maximum_mz)
                {
                    if(labelData[(int)RawLabelDataColumn.Charge, i] == labelData[(int)RawLabelDataColumn.Charge, index])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool containsPeak(double[,] data, double lowMZ, double highMZ, int charge, double minimumIntensity, ref int index)
        {
            for(int i = data.GetLowerBound(1); i <= data.GetUpperBound(1); i++)
            {
                if(data[(int)RawLabelDataColumn.MZ, i] > highMZ)
                {
                    break;
                }

                if(data[(int)RawLabelDataColumn.MZ, i] >= lowMZ)
                {
                    if(((int)data[(int)RawLabelDataColumn.Charge, i] == charge ||
                        (int)data[(int)RawLabelDataColumn.Charge, i] == 0) &&
                        data[(int)RawLabelDataColumn.Intensity, i] >= minimumIntensity)
                    {
                        index = i;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}