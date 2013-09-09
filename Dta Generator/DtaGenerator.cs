//#define Aaron_Experiment

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSMSL.IO.Thermo;
using MSFileReaderLib;
using CSMSL.IO;
using CSMSL.Spectral;
using System.Linq;
using System.Threading.Tasks;

namespace DtaGenerator
{
    public class DtaGenerator
    {
        private static double PROTON_MASS = 1.00727638;
        
        private static MzRange TMT126 = new MzRange(126.1, 126.3);
        private static MzRange TMT127 = new MzRange(127.1, 127.3);
        private static MzRange TMT128 = new MzRange(128.1, 128.3);
        private static MzRange TMT129 = new MzRange(129.1, 129.3);
        private static MzRange TMT130 = new MzRange(130.1, 130.3);
        private static MzRange TMT131 = new MzRange(131.1, 131.3);

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
        private List<double> neutralLosses;
        private int minimumAssumedPrecursorChargeState;
        private int maximumAssumedPrecursorChargeState;
        private bool cleanPrecursor;
        private bool enableEtdPreProcessing;
        private bool cleanTmtDuplex;
        private bool cleanItraq4Plex;
        private bool cleanTmt6Plex;
        private bool cleanItraq8Plex;
        private bool groupByActivationEnergyTime;
        private bool sequestDtaOutput;
        private bool omssaTxtOutput;
        private bool mascotMgfOutput;
        private string outputFolder;

        private Dictionary<string, StreamWriter> txt_outputs = null;
        private Dictionary<string, StreamWriter> mgf_outputs = null;
        private string LogFolder = string.Empty;
        private bool neutralLossesIncluded = false;
        private bool _useOverallLog = true;
        private List<MzRange> DefaultCleaningRanges = null;


        public DtaGenerator(IList<string> rawFilepaths,
            int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            bool cleanPrecursor, bool enableEtdPreProcessing,
            bool cleanTmtDuplex, bool cleanItraq4Plex, bool cleanTmt6Plex, bool cleanItraq8Plex,
            bool groupByActivationEnergyTime,
            bool sequestDtaOutput, bool omssaTxtOutput, bool mascotMgfOutput,
            string outputFolder,
            List<double> neutralLosses)
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
        }

        public void GenerateDtas()
        {     
            try
            {                
                onStarting(new EventArgs());

                if(!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }                                       

                neutralLossesIncluded = (this.neutralLosses != null && this.neutralLosses.Count > 0);
                DefaultCleaningRanges = new List<MzRange>();

                // Loop over each raw file and write their DTA
                foreach (string rawfilepath in rawFilepaths)
                {
                    WriteFile(rawfilepath);
                }               
                //Parallel.ForEach<string>(rawFilepaths, rawfilepath =>
                //{
                //     WriteFile(rawfilepath);
                //});

                if (_useOverallLog)
                {
                    string LogFolder = Path.Combine(outputFolder, "log");
                    if (!Directory.Exists(LogFolder))
                    {
                        Directory.CreateDirectory(LogFolder);
                    }

                    using (StreamWriter overall_log = new StreamWriter(Path.Combine(outputFolder, "DTA_Generator_log.txt")))
                    {
                        overall_log.WriteLine("DTA Generator PARAMETERS");
                        overall_log.WriteLine("Assumed Precursor Charge State Range: " + minimumAssumedPrecursorChargeState.ToString() + '-' + maximumAssumedPrecursorChargeState.ToString());
                        overall_log.WriteLine("Clean Precursor: " + cleanPrecursor.ToString());
                        overall_log.WriteLine("Enable ETD Pre-Processing: " + enableEtdPreProcessing.ToString());
                        overall_log.WriteLine("Clean TMT Duplex: " + cleanTmtDuplex.ToString());
                        overall_log.WriteLine("Clean iTRAQ 4-Plex: " + cleanItraq4Plex.ToString());
                        overall_log.WriteLine("Clean TMT 6-Plex: " + cleanTmt6Plex.ToString());
                        overall_log.WriteLine("Clean iTRAQ 8-Plex: " + cleanItraq8Plex.ToString());

                        if (neutralLossesIncluded)
                        {
                            overall_log.WriteLine("Neutral Losses: true");
                        }

                        overall_log.WriteLine();
                        foreach (string raw_filepath in rawFilepaths)
                        {
                            overall_log.WriteLine(raw_filepath);
                        }
                    }
                }               
            }
            catch(Exception ex)
            {
                onThrowException(new ExceptionEventArgs(ex));
            }
            finally
            { 
                onFinished(new EventArgs());
            }
        }

        public void WriteLog()
        {
            
        }

        private void WriteFile(string filepath)
        {
            onStartingFile(new FilepathEventArgs(filepath));

            //txt_outputs = new Dictionary<string, StreamWriter>();
            //mgf_outputs = new Dictionary<string, StreamWriter>();
            //SortedDictionary<string, int> spectrum_counts = new SortedDictionary<string, int>();
            //SortedDictionary<string, int> dta_counts = new SortedDictionary<string, int>();
            //SortedDictionary<int, double> retention_times = new SortedDictionary<int, double>();
            //SortedDictionary<int, double> scan_filter_mzs = new SortedDictionary<int, double>();
            //SortedDictionary<int, double> precursor_mzs = new SortedDictionary<int, double>();
            //SortedDictionary<int, double> precursor_intensities = new SortedDictionary<int, double>();
            //SortedDictionary<int, double> precursor_denormalized_intensities = new SortedDictionary<int, double>();
            //SortedDictionary<int, int> precursor_charge_states = new SortedDictionary<int, int>();
            //SortedDictionary<int, string> precursor_fragmentation_methods = new SortedDictionary<int, string>();
            //SortedDictionary<int, double> elapsed_scan_times = new SortedDictionary<int, double>();
            //SortedDictionary<int, double> ion_injection_times = new SortedDictionary<int, double>();
            //SortedDictionary<int, double?> precursor_sns = new SortedDictionary<int, double?>();
            //SortedDictionary<int, int> precursor_peak_depths = new SortedDictionary<int, int>();
           MSDataFile datafile = new ThermoRawFile(filepath);
            //   datafile.GetMsScans();
            //}
            //using (RawFile raw = new RawFile(filepath))
            //{
            //    // Open connection to the raw file
            //    raw.Open();
                           
                // Can generate massive background spectrum here... to remove from all the other spectra
                                
                using (StreamWriter writer = new StreamWriter(Path.Combine(outputFolder, Path.GetFileNameWithoutExtension(filepath) + ".txt")))
                {
                    int count = 0;
                    foreach(MSDataScan scan in datafile)
                    {
                        // Record Info
                        //if (_writeLog)
                        //{
                        //    WriteLog(msms);
                        //}

                        // Clean the spectrum
                        //CleanSpectrum(scan);

                        // Write the spectrum out
                        WriteSpectrumToFile(writer, scan);

                        // Change Progress
                        if (count > 100)
                        {
                            count = 0;
                            double progress = (double)(scan.SpectrumNumber - datafile.FirstSpectrumNumber + 1) / (datafile.LastSpectrumNumber - datafile.FirstSpectrumNumber + 1);
                            //onUpdateProgress(new ProgressEventArgs(progress,scan));
                        }
                        count++;
                    }
                }                
            
            //onFinishedFile(new FilepathEventArgs(filepath));
        }
    

        private void WriteSpectrumToFile(StreamWriter writer, MSDataScan msms)
        {
            if (omssaTxtOutput)
            {
                writer.WriteLine(msms);
            }
            //if (mascotMgfOutput) 
            //{
            //    writer.WriteLine(msms.ToMGF());
            //}
        }

        private void CleanSpectrum(Spectrum spectrum)
        {
            List<MzRange> ranges = new List<MzRange>(DefaultCleaningRanges); //Copy the default cleaning list (mzranges that don't depend on precursor information)
            
            // Get Spectrum Values
            double precursor_mz = spectrum.PrecursorMZ.Value;
            int precursor_charge = spectrum.Charge.Value;
            double precursor_mass = MZFromMass(precursor_mz, precursor_charge);
            bool isETD = spectrum.ScanType == ScanType.ETD || spectrum.ScanType == ScanType.ECD;

            if (cleanPrecursor || enableEtdPreProcessing && isETD)               
            {
                double half_width = spectrum.Parent.GetIsolationWidth(spectrum.ScanNumber) / 2.0;               
                ranges.Add(new MzRange(precursor_mz - half_width, precursor_mz + half_width));
            }

            // Clean ETD
            if (enableEtdPreProcessing && isETD && spectrum.Charge.HasValue)
            {
                ranges.AddRange(CleanETD(precursor_mz, precursor_mass, precursor_charge));                                           
            }

            // TMT 6-plex cleaning
            if (cleanTmt6Plex)
            {
                ranges.AddRange(CleanTMT6Plex(precursor_mz, precursor_charge, isETD));
            }
            
            // Neutral Loss cleaning
            if (neutralLossesIncluded)
            {
                ranges.AddRange(CleanNeutralLosses(precursor_mz, precursor_charge, neutralLosses));
            }

            if (true)
            {
                ranges.Add(new MzRange(368.234, 368.236));
                ranges.Add(new MzRange(372.241, 372.243));
                ranges.Add(new MzRange(376.247, 376.249));
                ranges.Add(new MzRange(155.124, 155.127));
                ranges.Add(new MzRange(172.150, 172.152));
                ranges.Add(new MzRange(184.140, 184.153));
                ranges.Add(new MzRange(226.161, 226.163));
                ranges.Add(new MzRange(155.124, 155.126));         
            }
                
            int peaks = spectrum.Count;
            spectrum.CleanSpectrum(ranges);
            int peaksremoved = peaks - spectrum.Count;
        }

        private static IEnumerable<MzRange> CleanETD(double precursor_mz, double precursor_mass, int precursor_charge)
        {
            double min, max;
            if (precursor_charge < 0)
            {
                // NETD    
                for (int z = -2; z >= precursor_charge + 1; z--)
                {
                    min = MZFromMass(precursor_mass - NETD_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA, z);
                    max = MZFromMass(precursor_mass + NETD_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_DA, z);
                    yield return new MzRange(min, max);
                    min = MZFromMass(precursor_mass + NETD_ADDUCT_CLEANING_WINDOW_DA, z) - NETD_ADDUCT_LOW_CLEANING_WINDOW_MZ;
                    max = MZFromMass(precursor_mass + NETD_ADDUCT_CLEANING_WINDOW_DA, z) + NETD_ADDUCT_LOW_CLEANING_WINDOW_MZ;
                     yield return new MzRange(min, max);
                }
                min = MZFromMass(precursor_mass, -1) - NETD_SINGLY_CHARGED_LOW_NEUTRAL_LOSS_CLEANING_WINDOW_MZ;
                yield return new MzRange(min, double.MaxValue);
            }
            else
            {
                // ETD
                for (int reduced_precursor_charge = 1; reduced_precursor_charge < precursor_charge; reduced_precursor_charge++)
                {
                    double cr_mz = MZFromMass(precursor_mz, reduced_precursor_charge);
                    min = cr_mz - 2; // TODO make variable
                    max = cr_mz + 2;
                    yield return new MzRange(min, max);
                }
            } 
        }

        private static IEnumerable<MzRange> CleanNeutralLosses(double precursor_mz, int precursor_charge, List<double> neutralLosses)
        {  
            foreach (double nl_mass in neutralLosses)
            {
                double mz = precursor_mz - MZFromMass(nl_mass, precursor_charge);
                double min = mz - LOW_PRECURSOR_CLEANING_WINDOW_MZ;
                double max = mz + HIGH_PRECURSOR_CLEANING_WINDOW_MZ;
                yield return new MzRange(min, max);               
            }            
        }

        private static IEnumerable<MzRange> CleanTMT6Plex(double precursor_mz, int precursor_charge, bool isETD)
        {
            double min, max;
            if (!isETD)
            {
                yield return TMT126;
                yield return TMT127;
                yield return TMT128;
                yield return TMT129;
                yield return TMT130;
                yield return TMT131;
                //min = MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ;
                //max = MINIMUM_TMT_6PLEX_CAD_REPORTER_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ;
                //yield return new MzRange(min, max);
                yield return new MzRange(TMT_6PLEX_CAD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ, TMT_6PLEX_CAD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ);
                for (int z = precursor_charge - 1; z >= 1; z--)
                {
                    double precursor_tmt_6plex_tag_cleaning_mz = precursor_mz * z - TMT_6PLEX_CAD_TAG_LOSS_DA / z;
                    min = precursor_tmt_6plex_tag_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ;
                    max = precursor_tmt_6plex_tag_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ;
                    yield return new MzRange(min, max);
                }
            }
            else
            {
                //                                    for (int reduced_charge_i = charge_i - 1; reduced_charge_i >= 1; reduced_charge_i--)
                //                                    {
                //                                        double precursor_tmt_6plex_reporter_loss_cleaning_mz = precursor_mz * charge_i - TMT_6PLEX_ETD_REPORTER_LOSS_DA / reduced_charge_i;
                //                                        double precursor_tmt_6plex_tag_loss_cleaning_mz = precursor_mz * charge_i - TMT_6PLEX_ETD_TAG_LOSS_DA / reduced_charge_i;

                //                                        int p1 = 0;
                //                                        while (p1 < peaks.Count)
                //                                        {
                //                                            double mz = peaks[p1].MZ;
                //                                            if ((mz >= MINIMUM_TMT_6PLEX_ETD_REPORTER_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                //                                                && mz <= MAXIMUM_TMT_6PLEX_ETD_REPORTER_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                //                                                || (mz >= TMT_6PLEX_ETD_TAG_MZ - TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ
                //                                                && mz <= TMT_6PLEX_ETD_TAG_MZ + TMT_6PLEX_CLEANING_MASS_TOLERANCE_MZ)
                //                                                || (mz >= precursor_tmt_6plex_reporter_loss_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                //                                                && mz <= precursor_tmt_6plex_reporter_loss_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ)
                //                                                || (mz >= precursor_tmt_6plex_tag_loss_cleaning_mz - TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ
                //                                                && mz <= precursor_tmt_6plex_tag_loss_cleaning_mz + TMT_6PLEX_LOSS_CLEANING_MASS_TOLERANCE_MZ))
                //                                            {
                //                                                peaks.RemoveAt(p1);
                //                                            }
                //                                            else
                //                                            {
                //                                                p1++;
                //                                            }
                //                                        }
                //                                    }
                //                                }
            }
            yield break;
        }     

        public static double MassFromMZ(double mz, int charge)
        {
            return mz * Math.Abs(charge) - charge * PROTON_MASS;
        }

        public static double MZFromMass(double mass, int charge)
        {
            return (mass + charge * PROTON_MASS) / Math.Abs(charge);
        }

        private static IEnumerable<int> AllIndicesOf(string s, char c)
        {
            for(int i = 0; i < s.Length; i++)
            {
                if(s[i] == c)
                {
                    yield return i;
                }
            }
        }
    }
}