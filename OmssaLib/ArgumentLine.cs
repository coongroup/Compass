using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OmssaLib
{
    public class ArgumentLine
    {
        /// <summary>
        /// Internal dictionary of keyvaluepairs for arguments
        /// </summary>
        private Dictionary<string, string> Arguments;

        /// (\-\w+)      Match a '-' with some word characters following it and group it into Group 1
        /// \s?          Optionally match a single whitespace (needed for capturing the last argument)  
        /// ([\w,.]+)?   Optionally match a word, number, ',', or '.' and group it into Group 2
        /// <summary>
        /// The regex which is used to parse argument lines
        /// </summary>
        public static Regex LineRegex = new Regex(@"(\-\w+)\s?([\w%\-,.]+)?");

        /// <summary>
        /// The Current Omssa Version that this class is built on.
        /// </summary>
        public static string OmssaVersion = "2.1.8";

        public string Name { get; set; }

        /// <summary>
        /// The Omssa-ready argument line given the different parameters specified in object.         
        /// </summary>
        public string Parameters
        {
            get
            {
                return ToString();
            }
            set
            {
                SetArguments(value);
            }
        }

        /// <summary>
        /// <para>(-umm)</para>
        /// Use memory mapped sequence libraries
        /// </summary>
        public bool UseMemoryMappedSequences
        {
            get
            {
                return Arguments.ContainsKey("-umm");
            }
            set
            {
                SetArgument("-umm", value);
            }
        }

        /// <summary>
        /// <para>(-w)</para>
        /// Include spectra and search params in search results
        /// </summary>
        public bool IncludeSearchParamtersInResults
        {
            get
            {
                return Arguments.ContainsKey("-w");
            }
            set
            {
                SetArgument("-w", value);
            }
        }

        /// <summary>
        /// <para>(-tem 0)</para>
        /// Precursor ion search type (0 = mono, 1 = avg, 2 = N15, 3 = exact, 4= multiisotope)
        /// </summary>
        public PrecursorIonSearchType PrecursorIonSearch
        {
            get
            {
                int defaultvalue = (int)PrecursorIonSearchType.Monoisotopic;
                int result;
                return (PrecursorIonSearchType)
                    ((TryGetArgument("-tem", out result, defaultvalue)) ?
                    result :
                    defaultvalue);
            }
            set
            {
                if(!Enum.IsDefined(typeof(PrecursorIonSearchType), value))
                {
                    throw new ArgumentOutOfRangeException("-tem", value,
                        "Precursor Search type is not valid.");
                }
                if(value == PrecursorIonSearchType.Monoisotopic)
                {
                    RemoveArgument("-tem");
                }
                else
                {
                    SetArgument("-tem", (int)value, (int)PrecursorIonSearchType.Monoisotopic);
                }
            }
        }

        /// <summary>
        /// <para>(-tom 0)</para>
        /// Product ion search type (0 = mono, 1 = avg, 2 = N15, 3 = exact)
        /// </summary>
        public ProductIonSearchType ProductIonSearch
        {
            get
            {
                int defaultvalue = (int)ProductIonSearchType.Monoisotopic;
                int result;
                return (ProductIonSearchType)
                    ((TryGetArgument("-tom", out result, defaultvalue)) ?
                    result :
                    defaultvalue);
            }
            set
            {
                if(!Enum.IsDefined(typeof(ProductIonSearchType), value))
                {
                    throw new ArgumentOutOfRangeException("-tom", value,
                        "Product ion search type is not valid.");
                }
                if(value == ProductIonSearchType.Monoisotopic)
                {
                    RemoveArgument("-tom");
                }
                else
                {
                    SetArgument("-tom", (int)value, (int)ProductIonSearchType.Monoisotopic);
                }
            }
        }

        /// <summary>
        /// <para>(-te 2.0)</para>
        /// Precursor ion m/z tolerance in Da (or ppm if -teppm flag set)
        /// </summary>
        public double PrecursorMZTolerance
        {
            get
            {
                double defaultval = 2.0;
                double result;
                return (TryGetArgument("-te", out result, defaultval)) ?
                    result :
                    defaultval;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-te", value, "Precursor ion m/z tolerance must be larger than 0");
                }
                if(value == 2.0)
                {
                    RemoveArgument("-te");
                }
                else
                {
                    SetArgument("-te", value, 2.0);
                }
            }
        }

        /// <summary>
        /// <para>(-to 0.8)</para>       
        /// Product ion m/z tolerance in Da
        /// </summary>
        public double ProductMZTolerance
        {
            get
            {
                double defaultval = 0.8;
                double result;
                return (TryGetArgument("-to", out result, defaultval)) ?
                    result :
                    defaultval;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-to", value, "Product ion m/z tolerance must be larger than 0");
                }
                if(value == 0.8)
                {
                    RemoveArgument("-to");
                }
                else
                {
                    SetArgument("-to", value, 0.8);
                }
            }
        }

        /// <summary>
        /// <para>(-d)</para> 
        /// Blast sequence library to search. Do not include .p* filename suffixes
        /// </summary>
        public string Database
        {
            get
            {
                return this["-d"];
            }
            set
            {
                SetArgument("-d", value);
            }
        }

        /// <summary>
        /// <para>(-tez 0)</para>
        /// Charge dependency of precursor mass tolerance (0 = none, 1 = linear)
        /// </summary>
        public ChargeDependencyType ChargeDependencyOfPrecursorMassTolerance
        {
            get
            {
                int defaultvalue = (int)ChargeDependencyType.None;
                int result;
                return (ChargeDependencyType)
                    ((TryGetArgument("-tez", out result, defaultvalue)) ?
                    result :
                    defaultvalue);
            }
            set
            {
                if(!Enum.IsDefined(typeof(ChargeDependencyType), value))
                {
                    throw new ArgumentOutOfRangeException("-tez", value,
                        "Charge Dependency is not valid.");
                }
                if(value == ChargeDependencyType.None)
                {
                    RemoveArgument("-tez");
                }
                else
                {
                    SetArgument("-tez", (int)value, (int)ChargeDependencyType.None);
                }
            }
        }

        /// <summary>
        /// <para>(-teppm)</para>
        /// Search precursor masses in units of ppm
        /// </summary>
        public PrecursorMZToleranceType PrecursorMZToleranceType
        {
            get
            {
                if(Arguments.ContainsKey("-teppm"))
                    return PrecursorMZToleranceType.PPM;
                return PrecursorMZToleranceType.DA;
            }
            set
            {
                SetArgument("-teppm", value == PrecursorMZToleranceType.PPM);
            }
        }

        /// <summary>
        /// <para>(-tex 1446.94)</para>
        /// Threshold in Da above which the mass of neutron should be added in exact
        /// mass search
        /// </summary>
        public double NeutronThresholdMZ
        {
            get
            {
                double defaultvalue = 1446.94;
                double result;
                return (TryGetArgument("-tex", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-tex", value,
                        "Threshold of neutron addition must be larger than 0");
                }
                if(value == 1446.94)
                {
                    RemoveArgument("-tex");
                }
                else
                {
                    SetArgument("-tex", value, 1446.94);
                }
            }
        }

        private InputFileType inputFileType = InputFileType.MultipleDtaXml;
        public InputFileType InputFileType
        {
            get
            {
                return inputFileType;
            }
            set
            {
                inputFileType = value;
            }
        }

        public string InputFile
        {
            get
            {
                return this[EnumHelper.GetDescription(InputFileType)];
            }
            set
            {
                SetArgument(EnumHelper.GetDescription(InputFileType), value);
            }
        }

        private OutputFileType outputFileType = OutputFileType.Csv;
        public OutputFileType OutputFileType
        {
            get
            {
                return outputFileType;
            }
            set
            {
                outputFileType = value;
            }
        }
        public string Output
        {
            get
            {
                return this[EnumHelper.GetDescription(OutputFileType)];
            }
            set
            {
                SetArgument(EnumHelper.GetDescription(OutputFileType), value);
            }
        }

        /// <summary>
        /// <para>(-i "1,4")</para>
        /// Id numbers of ions to search (comma delimited, no spaces)
        /// </summary>        
        public string IonSeries
        {
            get
            {
                string defaultvalue = "1,4";
                string result;
                return (TryGetArgument("-i", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == "1,4")
                {
                    RemoveArgument("-i");
                }
                else
                {
                    SetArgument("-i", value, "1,4");
                }
            }
        }

        /// <summary>
        /// <para>(-cl 0.0)</para>
        /// Low intensity cutoff as a fraction of max peak
        /// </summary>
        public double LowIntensityCutoff
        {
            get
            {
                double defaultvalue = 0.0;
                double result;
                return (TryGetArgument("-cl", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("-cl", value,
                        "Low intensity cutoff must be between 0 and 1");
                }
                if(value == 0.0)
                {
                    RemoveArgument("-cl");
                }
                else
                {
                    SetArgument("-cl", value, 0.0);
                }
            }
        }

        /// <summary>
        /// <para>(-ch 0.2)</para>
        /// High intensity cutoff as a fraction of max peak
        /// </summary>
        public double HighIntensityCutoff
        {
            get
            {
                double defaultvalue = 0.2;
                double result;
                return (TryGetArgument("-ch", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("-ch", value,
                        "High intensity cutoff must be between 0 and 1");
                }
                if(value == 0.2)
                {
                    RemoveArgument("-ch");
                }
                else
                {
                    SetArgument("-ch", value, 0.2);
                }
            }
        }

        /// <summary>
        /// <para>(-ci 0.0005)</para>
        /// Intensity cutoff increment as a fraction of max peak
        /// </summary>
        public double IntensityCutoffIncrement
        {
            get
            {
                double defaultvalue = 0.0005;
                double result;
                return (TryGetArgument("-ci", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("-ci", value,
                        "Intensity cutoff increment must be between 0 and 1");
                }
                if(value == 0.0005)
                {
                    RemoveArgument("-ci");
                }
                else
                {
                    SetArgument("-ci", value, 0.0005);
                }
            }
        }

        /// <summary>
        ///  (-cp 0) 
        ///  Eliminate charge reduced precursors in spectra (0=false, 1=true)
        ///  Default = 0
        /// </summary>
        public bool RemoveChargeReducedPrecursor
        {
            get
            {
                int defaultvalue = 0;
                int result;
                if(TryGetArgument("-cp", out result, defaultvalue))
                {
                    return (result == 1);
                }
                return false;
            }
            set
            {
                if(!value)
                {
                    RemoveArgument("-cp");
                }
                else
                {
                    SetArgument("-cp", value ? 1 : 0, 1);
                }
            }
        }

        /// <summary>
        /// <para>(-v 1)</para>
        /// Number of missed cleavages allowed
        /// </summary>
        public int NumberOfMissedCleavages
        {
            get
            {
                int defaultvalue = 1;
                int result;
                return (TryGetArgument("-v", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-v", value,
                        "Number of missed cleavages must be equal to or larger than 0");
                }
                if(value == 1)
                {
                    RemoveArgument("-v");
                }
                else
                {
                    SetArgument("-v", value, 1);
                }
            }
        }

        /// <summary>
        /// <para>(-w1 27)</para>
        /// Single charge window in Da
        /// </summary>
        public int SingleChargeWindow
        {
            get
            {
                int defaultvalue = 27;
                int result;
                return (TryGetArgument("-w1", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-w1", value,
                        "Single charge window must be equal to or larger than 0");
                }
                if(value == 27)
                {
                    RemoveArgument("-w1");
                }
                else
                {
                    SetArgument("-w1", value, 27);
                }
            }
        }

        /// <summary>
        /// <para>(-w2 14)</para>
        /// Double charge window in Da
        /// </summary>
        public int DoubleChargeWindow
        {
            get
            {
                int defaultvalue = 14;
                int result;
                return (TryGetArgument("-w2", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-w2", value,
                        "Double charge window must be equal to or larger than 0");
                }
                if(value == 14)
                {
                    RemoveArgument("-w2");
                }
                else
                {
                    SetArgument("-w2", value, 14);
                }
            }
        }

        /// <summary>
        /// <para>(-h1 2)</para>
        /// Number of peaks allowed in single charge window (0 = number of ion species)
        /// </summary>
        public int NumberOfPeaksAllowedInSingleChargeWindow
        {
            get
            {
                int defaultvalue = 2;
                int result;
                return (TryGetArgument("-h1", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-h1", value,
                        "The number of peaks in the single charge window must be equal to or larger than 0");
                }
                if(value == 2)
                {
                    RemoveArgument("-h1");
                }
                else
                {
                    SetArgument("-h1", value, 2);
                }
            }
        }

        /// <summary>
        /// <para>(-h2 2)</para>
        /// Number of peaks allowed in double charge window (0 = number of ion species)
        /// </summary>
        public int NumberOfPeaksAllowedInDoubleChargeWindow
        {
            get
            {
                int defaultvalue = 2;
                int result;
                return (TryGetArgument("-h2", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-h2", value,
                        "The number of peaks in the double charge window must be equal to or larger than 0");
                }
                if(value == 2)
                {
                    RemoveArgument("-h2");
                }
                else
                {
                    SetArgument("-h2", value, 2);
                }
            }
        }

        /// <summary>
        /// <para>(-hl 30)</para>
        /// Maximum number of hits retained per precursor charge state per spectrum
        /// during the search
        /// </summary>
        public int MaximumHitsRetainedPerChargeStatePerSpectrum
        {
            get
            {
                int defaultvalue = 30;
                int result;
                return (TryGetArgument("-hl", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-hl", value,
                        "The maximum number of hits retained per precursor charge state per spectrum be equal to or larger than 0");
                }
                if(value == 30)
                {
                    RemoveArgument("-hl");
                }
                else
                {
                    SetArgument("-hl", value, 30);
                }
            }
        }

        /// <summary>
        /// <para>(-hc 0)</para>
        /// Maximum number of hits reported per spectrum (0 = all)        
        /// </summary>
        public int MaximumHitsReportedPerSpectrum
        {
            get
            {
                int defaultvalue = 0;
                int result;
                return (TryGetArgument("-hc", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-hc", value,
                        "The maximum number of hits reported per spectrum be equal to or larger than 0");
                }
                if(value == 0)
                {
                    RemoveArgument("-hc");
                }
                else
                {
                    SetArgument("-hc", value, 0);
                }
            }
        }

        /// <summary>
        /// <para>(-ht 6)</para>
        /// Number of m/z values corresponding to the most intense peaks that must
        /// include one match to the theoretical peptide       
        /// </summary>
        public int NumberOfMostIntensePeaksNeededToMatchWithTheoreiticalPeptide
        {
            get
            {
                int defaultvalue = 6;
                int result;
                return (TryGetArgument("-ht", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-ht", value,
                        "The number of precursors matched to the most intense peaks must be larger than 0");
                }
                if(value == 6)
                {
                    RemoveArgument("-ht");
                }
                else
                {
                    SetArgument("-ht", value, 6);
                }
            }
        }

        /// <summary>
        /// <para>(-hm 2)</para>
        /// The minimum number of m/z matches a sequence library peptide must have for
        /// the hit to the peptide to be recorded       
        /// </summary>
        public int MinimumNumberOfHitsFromLibraryToHavePeptideHitRecorded
        {
            get
            {
                int defaultvalue = 2;
                int result;
                return (TryGetArgument("-hm", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-hm", value,
                        "The minimum number of hits for a library peptide must be larger than 0");
                }
                if(value == 2)
                {
                    RemoveArgument("-hm");
                }
                else
                {
                    SetArgument("-hm", value, 2);
                }
            }
        }

        /// <summary>
        /// <para>(-hs 4)</para>
        /// The minimum number of m/z values a spectrum must have to be searched       
        /// </summary>
        public int MinimumNumberOfPeaksInSpectrumToBeSearched
        {
            get
            {
                int defaultvalue = 4;
                int result;
                return (TryGetArgument("-hs", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-hs", value,
                        "The minimum number of m/z vales per spectra must be larger than 0");
                }
                if(value == 4)
                {
                    RemoveArgument("-hs");
                }
                else
                {
                    SetArgument("-hs", value, 4);
                }
            }
        }

        /// <summary>
        /// <para>(-he 1.0)</para>
        /// The maximum evalue allowed in the hit list        
        /// </summary>
        public double MaximumEValue
        {
            get
            {
                double defaultvalue = 1.0;
                double result;
                return (TryGetArgument("-he", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-he", value,
                        "The minimum e-value must be larger than 0");
                }
                if(value == 1.0)
                {
                    RemoveArgument("-he");
                }
                else
                {
                    SetArgument("-he", value, 1.0);
                }
            }
        }

        /// <summary>
        /// <para>(-mf)</para>
        /// Comma delimited (no spaces) list of id numbers for fixed modifications
        /// </summary>
        public string FixedMods
        {
            get
            {
                string defaultvalue = string.Empty;
                string result;
                return (TryGetArgument("-mf", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == string.Empty)
                {
                    RemoveArgument("-mf");
                }
                else
                {
                    SetArgument("-mf", value, string.Empty);
                }
            }
        }

        /// <summary>
        /// <para>(-mv)</para>
        /// Comma delimited (no spaces) list of id numbers for variable modifications
        /// </summary>
        public string VariableMods
        {
            get
            {
                string defaultvalue = string.Empty;
                string result;
                return (TryGetArgument("-mv", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == string.Empty)
                {
                    RemoveArgument("-mv");
                }
                else
                {
                    SetArgument("-mv", value, string.Empty);
                }
            }
        }

        /// <summary>
        /// <para>(-mnm)</para>
        /// N-term methionine should not be cleaved
        /// </summary>
        public bool NTermMethionineShouldNotBeCleaved
        {
            get
            {
                return Arguments.ContainsKey("-mnm");
            }
            set
            {
                SetArgument("-mnm", value);
            }
        }

        /// <summary>
        /// <para>(-mm 128)</para>
        /// The maximum number of mass ladders to generate per database peptide
        /// </summary>
        public int MaximumMassLadderPerDatabasePeptide
        {
            get
            {
                int defaultvalue = 128;
                int result;
                return (TryGetArgument("-mm", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-mm", value,
                        "The maximum number of mass ladders for a peptide must be greater than 0");
                }
                if(value == 128)
                {
                    RemoveArgument("-mm");
                }
                else
                {
                    SetArgument("-mm", value, 128);
                }
            }
        }

        /// <summary>
        /// <para>(-e 0)</para>
        /// Id number of enzyme to use (wrapped in a enum EnzymeType)
        /// </summary>
        public EnzymeType Enzyme
        {
            get
            {
                int defaultvalue = (int)EnzymeType.Trypsin;
                int result;
                return (EnzymeType)
                ((TryGetArgument("-e", out result, defaultvalue)) ?
                    result :
                    defaultvalue);
            }
            set
            {
                if(!Enum.IsDefined(typeof(EnzymeType), value))
                {
                    throw new ArgumentOutOfRangeException("-e", value,
                        "Enzyme type is not valid");
                }
                SetArgument("-e", (int)value, (int)EnzymeType.Trypsin);  
            }
        }

        /// <summary>
        /// <para>(-zh 3)</para>
        /// Maximum precursor charge to search when not 1+       
        /// </summary>
        public int MaximumPrecursorCharge
        {
            get
            {
                int defaultvalue = 3;
                int result;
                return (TryGetArgument("-zh", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 1)
                {
                    throw new ArgumentOutOfRangeException("-zh", value,
                        "Maximum Precursor Charge must be larger than 0");
                }
                if(value == 3)
                {
                    RemoveArgument("-zh");
                }
                else
                {
                    SetArgument("-zh", value, 3);
                }
            }
        }

        /// <summary>
        /// <para>(-zl 1)</para>
        /// Minimum precursor charge to search when not 1+       
        /// </summary>
        public int MinimumPrecursorCharge
        {
            get
            {
                int defaultvalue = 1;
                int result;
                return (TryGetArgument("-zl", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 1)
                {
                    throw new ArgumentOutOfRangeException("-zl", value,
                        "Minimum Precursor Charge must be larger than 0");
                }
                if(value == 1)
                {
                    RemoveArgument("-zl");
                }
                else
                {
                    SetArgument("-zl", value, 1);
                }
            }
        }

        /// <summary>
        /// <para>(-zoh 2)</para>
        /// Maximum product charge to search      
        /// </summary>
        public int MaximumProductCharge
        {
            get
            {
                int defaultvalue = 2;
                int result;
                return (TryGetArgument("-zoh", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-zoh", value,
                        "The maximum product charge must be greater than 0");
                }
                if(value == 2)
                {
                    RemoveArgument("-zoh");
                }
                else
                {
                    SetArgument("-zoh", value, 2);
                }
            }
        }

        /// <summary>
        /// <para>(-zt 3)</para>
        /// Minimum precursor charge to start considering multiply charged products     
        /// </summary>
        public int MinimumPrecursorChargeToConsiderMultiplyChargedProducts
        {
            get
            {
                int defaultvalue = 3;
                int result;
                return (TryGetArgument("-zt", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value <= 0)
                {
                    throw new ArgumentOutOfRangeException("-zt", value,
                        "The minimum precursor charge must be greater than 0");
                }
                if(value == 3)
                {
                    RemoveArgument("-zt");
                }
                else
                {
                    SetArgument("-zt", value, 3);
                }
            }
        }

        /// <summary>
        ///  <para>(-z1 0.95)</para>
        ///  Fraction of peaks below precursor used to determine if spectrum is charge 1
        /// </summary>
        public double FractionOfPeaksToAssignPlus1
        {
            get
            {
                double value = 0.95;
                string result;
                if(Arguments.TryGetValue("-z1", out result))
                {
                    double.TryParse(result, out value);
                }
                return value;
            }
            set
            {
                if(value == 0.95)
                {
                    RemoveArgument("-z1");
                }
                else
                {
                    SetArgument("-z1", value);
                }
            }
        }

        /// <summary>
        ///  <para>(-zc 1)</para>
        ///  Should charge plus one be determined algorithmically? (1=yes)
        /// </summary>
        public bool AlgorithmicallyDetermineChargePlusOne
        {
            get
            {
                int defaultvalue = 1;
                int result;
                if(TryGetArgument("-zc", out result, defaultvalue))
                {
                    return (result == 1);
                }
                return true;
            }
            set
            {
                if(!value)
                {
                    SetArgument("-zc", 0, 1);
                }
                else
                {
                    RemoveArgument("-zc");
                }
            }
        }

        /// <summary>
        ///  <para>(-zcc 2)</para> 
        /// How should precursor charges be determined? (1=believe the input file,
        /// 2=use a range)        
        /// </summary>
        public PrecursorChargeType PrecursorChargeType
        {
            get
            {
                int defaultvalue = (int)PrecursorChargeType.UseRange;
                int result;
                if(TryGetArgument("-zcc", out result, defaultvalue))
                {
                    return (PrecursorChargeType)result;
                }
                return (PrecursorChargeType)defaultvalue;
            }
            set
            {
                if(!Enum.IsDefined(typeof(PrecursorChargeType), value))
                {
                    throw new ArgumentOutOfRangeException("-zcc", value,
                        "Precursor Charge Type is not valid");
                }
                if(value == PrecursorChargeType.UseRange)
                {
                    RemoveArgument("-zcc");
                }
                else
                {
                    SetArgument("-zcc", (int)value, (int)PrecursorChargeType.UseRange);
                }
            }
        }

        /// <summary>
        /// <para>(-zn 1)</para>
        /// Search using negative or positive ions (1=positive, -1=negative)    
        /// </summary>
        public Polarity Polarity
        {
            get
            {
                int defaultvalue = (int)Polarity.Positive;
                int result;
                if(TryGetArgument("-zn", out result, defaultvalue))
                {
                    return (Polarity)result;
                }
                return (Polarity)defaultvalue;
            }
            set
            {
                if(!Enum.IsDefined(typeof(Polarity), value))
                {
                    throw new ArgumentOutOfRangeException("-zn", value,
                        "Polarity must be either 1 or -1");
                }
                if(value == Polarity.Positive)
                {
                    RemoveArgument("-zn");
                }
                else
                {
                    SetArgument("-zn", (int)value, (int)Polarity.Positive);
                }
            }
        }

        /// <summary>
        /// <para>(-pc 1)</para>
        /// Minimum number of precursors that match a spectrum      
        /// </summary>
        public int MinimumPrecursorsToMatchSpectrum
        {
            get
            {
                int defaultvalue = 1;
                int result;
                return (TryGetArgument("-pc", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-pc", value,
                        "The minimum number of precursors to match a spectrum must be equal or greater than 0");
                }
                if(value == 1)
                {
                    RemoveArgument("-pc");
                }
                else
                {
                    SetArgument("-pc", value, 1);
                }
            }
        }

        /// <summary>
        ///  <para>(-sb1 1)</para>
        ///  Should first forward (b1) product ions be in search (1=no)
        /// </summary>
        public bool SearchFirstForwardProductIon
        {
            get
            {
                int defaultvalue = 1;
                int result;
                if(TryGetArgument("-sb1", out result, defaultvalue))
                {
                    return (result != 1);
                }
                return false;
            }
            set
            {
                if(!value)
                {
                    SetArgument("-sb1", 0);
                }
                else
                {
                    RemoveArgument("-sb1");
                }
            }
        }

        /// <summary>
        ///  <para>(-sct 0)</para>
        ///  Should c terminus ions be searched (1=no)
        /// </summary>
        public bool SearchCTerminusIons
        {
            get
            {
                int defaultvalue = 0;
                int result;
                if(TryGetArgument("-sct", out result, defaultvalue))
                {
                    return (result != 1);
                }
                return true;
            }
            set
            {
                if(!value)
                {
                    SetArgument("-sct", 1);
                }
                else
                {
                    RemoveArgument("-sct");
                }
            }
        }

        /// <summary>
        /// <para>(-sp 100)</para>
        /// Max number of ions in each series being searched (0=all)     
        /// </summary>
        public int MaximumIonsPerSeries
        {
            get
            {
                int defaultvalue = 100;
                int result;
                return (TryGetArgument("-sp", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 100)
                {
                    RemoveArgument("-sp");
                }
                else
                {
                    SetArgument("-sp", value, 100);
                }
            }
        }

        /// <summary>
        ///  <para>(-scorr 0)</para>
        ///  Turn off correlation correction to score (1=off, 0=use correlation)
        /// </summary>
        public bool CorrelationCorrection
        {
            get
            {
                int defaultvalue = 0;
                int result;
                if(TryGetArgument("-scorr", out result, defaultvalue))
                {
                    return (result != 1);
                }
                return true;
            }
            set
            {
                if(!value)
                {
                    SetArgument("-scorr", 1);
                }
                else
                {
                    RemoveArgument("-scorr");
                }
            }
        }

        /// <summary>
        /// <para>(-mx mods.xml)</para>
        /// File containing modification data        
        /// </summary>
        public string ModFile
        {
            get
            {
                string defaultvalue = "mods.xml";
                string result;
                return (TryGetArgument("-mx", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == "mods.xml")
                {
                    RemoveArgument("-mx");
                }
                else
                {
                    SetArgument("-mx", value, "mods.xml");
                }
            }
        }

        /// <summary>
        /// <para>(-mux usermods.xml)</para>
        /// File containing user modification data        
        /// </summary>
        public string UserModFile
        {
            get
            {
                string defaultvalue = "usermods.xml";
                string result;
                return (TryGetArgument("-mux", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == "usermods.xml")
                {
                    RemoveArgument("-mux");
                }
                else
                {
                    SetArgument("-mux", value, "usermods.xml");
                }
            }
        }

        /// <summary>
        ///  <para>(-scorp 0.5)</para>
        ///  Probability of consecutive ion (used in correlation correction)
        /// </summary>
        public double ProbabilityOfConsecutiveIon
        {
            get
            {
                double defaultvalue = 0.5;
                double result;
                return (TryGetArgument("-scorp", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("-scorp", value,
                        "The probability of consecutive ions must be between 0 and 1.");
                }
                if(value == 0.5)
                {
                    RemoveArgument("-scorp");
                }
                else
                {
                    SetArgument("-scorp", value, 0.5);
                }
            }
        }

        /// <summary>
        /// <para>(-no 4)</para>
        /// Minimum size of peptides for no-enzyme and semi-tryptic searches     
        /// </summary>
        public int MinimumPeptideSizeNoEnzyme
        {
            get
            {
                int defaultvalue = 4;
                int result;
                return (TryGetArgument("-no", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 4)
                {
                    RemoveArgument("-no");
                }
                else
                {
                    SetArgument("-no", value, 4);
                }
            }
        }

        /// <summary>
        /// <para>(-nox 40)</para>
        /// Maximum size of peptides for no-enzyme and semi-tryptic searches (0=none)     
        /// </summary>
        public int MaximumPeptideSizeNoEnzyme
        {
            get
            {
                int defaultvalue = 40;
                int result;
                return (TryGetArgument("-nox", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 40)
                {
                    RemoveArgument("-nox");
                }
                else
                {
                    SetArgument("-nox", value, 40);
                }
            }
        }

        /// <summary>
        /// <para>(-ti 0)</para>
        /// When doing multiisotope search, number of isotopic peaks to search.  0 =
        ///monoisotopic peak only    
        /// </summary>
        public int NumberOfIsotopicPeaksToSearch
        {
            get
            {
                int defaultvalue = 0;
                int result;
                return (TryGetArgument("-ti", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-ti", value,
                        "The number of isotopes to search must be equal to or greater than 0.");
                }
                if(value == 0)
                {
                    RemoveArgument("-ti");
                }
                else
                {
                    SetArgument("-ti", value, 0);
                }
            }
        }

        /// <summary>
        /// <para>(-is 0.0)</para>
        /// Evalue threshold to include a sequence in the iterative search, 0 = all
        /// </summary>
        public double IterativeSearchEValueThreshold
        {
            get
            {
                double defaultvalue = 0.0;
                double result;
                return (TryGetArgument("-is", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 0.0)
                {
                    RemoveArgument("-is");
                }
                else
                {
                    SetArgument("-is", value, 0.0);
                }
            }
        }

        /// <summary>
        ///  <para>(-ir 0.0)</para>
        ///  Evalue threshold to replace a hit, 0 = only if better
        /// </summary>
        public double IterativeSearchEValueThresholdToReplaceHit
        {
            get
            {
                double defaultvalue = 0.0;
                double result;
                return (TryGetArgument("-ir", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 0.0)
                {
                    RemoveArgument("-ir");
                }
                else
                {
                    SetArgument("-ir", value, 0.0);
                }
            }
        }

        /// <summary>
        ///  <para>(-ii 0.01)</para>
        ///  Evalue threshold to iteratively search a spectrum again, 0 = always
        /// </summary>
        public double IterativeSearchEValueThresholdToSearchSpectrumAgain
        {
            get
            {
                double defaultvalue = 0.01;
                double result;
                return (TryGetArgument("-ii", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == 0.01)
                {
                    RemoveArgument("-ii");
                }
                else
                {
                    SetArgument("-ii", value, 0.01);
                }
            }
        }

        /// <summary>
        /// <para>(-p)</para>
        /// Id numbers of ion series to apply no product ions at proline rule at (comma
        /// delimited, no spaces)
        /// </summary>        
        public string IonSeriesNoProlineRule
        {
            get
            {
                string defaultvalue = string.Empty;
                string result;
                return (TryGetArgument("-p", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == string.Empty)
                {
                    RemoveArgument("-p");
                }
                else
                {
                    SetArgument("-p", value, string.Empty);
                }
            }
        }

        /// <summary>
        /// <para>(-x 0)</para>
        /// Comma delimited list of taxids to search (0 = all)
        /// </summary>        
        public string TaxIds
        {
            get
            {
                string defaultvalue = "0";
                string result;
                return (TryGetArgument("-x", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value == "0" || value == string.Empty)
                {
                    RemoveArgument("-x");
                }
                else
                {
                    SetArgument("-x", value, "0");
                }
            }
        }

        /// <summary>
        /// <para>(-nt 0)</para>
        /// Number of search threads to use, 0=autodetect     
        /// </summary>
        public int NumberOfSearchThreads
        {
            get
            {
                int defaultvalue = 0;
                int result;
                return (TryGetArgument("-nt", out result, defaultvalue)) ?
                    result :
                    defaultvalue;
            }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("-nt", value,
                        "The number of search threads must be positive or zero.");
                }
                if(value == 0)
                {
                    RemoveArgument("-nt");
                }
                else
                {
                    SetArgument("-nt", value, 0);
                }
            }
        }

        /// <summary>
        /// <para>(-ni)</para>
        /// Print informational messages
        /// </summary>
        public bool PrintInformationalMessages
        {
            get
            {
                return Arguments.ContainsKey("-ni");
            }
            set
            {
                SetArgument("-ni", value);
            }
        }

        /// <summary>
        /// <para>(-os)</para>
        /// Use Omssa 1.0 scoring
        /// </summary>
        public bool Omssa1Scoring
        {
            get
            {
                return Arguments.ContainsKey("-os");
            }
            set
            {
                SetArgument("-os", value);
            }
        }

        /// <summary>
        /// <para>(-nrs)</para>
        /// Turn off rank score
        /// </summary>
        public bool RankScoringOFf
        {
            get
            {
                return Arguments.ContainsKey("-nrs");
            }
            set
            {
                SetArgument("-nrs", value);
            }
        }

        public string this[string argument]
        {
            get
            {
                if(Arguments.ContainsKey(argument))
                {
                    return Arguments[argument];
                }
                return String.Empty;
            }
            set
            {
                if(Arguments.ContainsKey(argument))
                {
                    Arguments[argument] = value;
                }
                else
                {
                    Arguments.Add(argument, value);
                }
            }
        }


        /// <summary>
        /// If true, remove default values from the ToString()
        /// </summary>
        public bool RemoveDefaults { get; set; }

        public ArgumentLine()
            :this(true){ }            
        

        public ArgumentLine(bool removeDefaults)
        {
            Arguments = new Dictionary<string, string>();
            RemoveDefaults = removeDefaults;
        }

        public ArgumentLine(ArgumentLine arg)
        {
            Arguments = new Dictionary<string, string>(arg.Arguments);
            RemoveDefaults = arg.RemoveDefaults;
        }

        public ArgumentLine(string argumentline)
            : this(argumentline, true) { }

        public ArgumentLine(string argumentline, bool removeDefaults)
            : this()
        {
            RemoveDefaults = removeDefaults;
            SetArguments(argumentline);
        }

        public string GetArgument(string argument)
        {
            return this[argument];
        }

        public void SetArgument(string argument, string value)
        {
            if(value.Contains(" ") && (value[0] != '"' || value[value.Length - 1] != '"'))
            {
                this[argument] = '"' + value.Trim('"') + '"';
            }
            else
            {
                this[argument] = value;
            }
        }

        public void SetArgument(string argument, bool value)
        {
            if(value)
            {
                Arguments[argument] = String.Empty;
            }
            else
            {
                RemoveArgument(argument);
            }
        }

        public bool TrySetArgument(string argument, object value)
        {
            double dblvalue;
            int intvalue;
            switch(argument)
            {

                case "-fx":
                case "-d":
                case "-mux":
                case "-mx":
                    return true;
                case "-op":
                    SetOutputFile(value.ToString(), OmssaLib.OutputFileType.PepXml);
                    return true;
                case "-ox":
                    SetOutputFile(value.ToString(), OmssaLib.OutputFileType.Xml);
                    return true;
                case "-oc":
                    SetOutputFile(value.ToString(), OmssaLib.OutputFileType.Csv);
                    return true;
                case "-scorp":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        ProbabilityOfConsecutiveIon = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("scorp");
                case "-scorr":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        CorrelationCorrection = (intvalue == 0);
                        return true;
                    }
                    throw new ArgumentNullException("scorr");
                case "-zn":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfSearchThreads = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zn");
                case "-nt":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfSearchThreads = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("nt");
                case "-no":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumPeptideSizeNoEnzyme = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("no");
                case "-nox":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumPeptideSizeNoEnzyme = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("nox");
                case "-sp":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumIonsPerSeries = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("sp");
                case "-pc":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumPrecursorsToMatchSpectrum = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("pc");
                case "-hs":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumNumberOfPeaksInSpectrumToBeSearched = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("hs");
                case "-hm":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumNumberOfHitsFromLibraryToHavePeptideHitRecorded = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("hm");
                case "-he":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        MaximumEValue = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("he");
                case "-ht":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfMostIntensePeaksNeededToMatchWithTheoreiticalPeptide = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("ht");
                case "-hl":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumHitsRetainedPerChargeStatePerSpectrum = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("hl");
                case "-hc":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumHitsReportedPerSpectrum = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("hc");
                case "-h1":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfPeaksAllowedInSingleChargeWindow = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("h1");
                case "-h2":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfPeaksAllowedInDoubleChargeWindow = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("h2");
                case "-w1":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        SingleChargeWindow = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("w1");
                case "-w2":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        DoubleChargeWindow = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("w2");
                case "-ci":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        IntensityCutoffIncrement = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("ci");
                case "-ch":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        HighIntensityCutoff = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("ch");
                case "-cl":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        LowIntensityCutoff = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("cl");
                case "-z1":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        FractionOfPeaksToAssignPlus1 = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("z1");
                case "-i":
                    if(value.ToString().Length >= 1)
                    {
                        IonSeries = value.ToString();
                        return true;
                    }
                    throw new ArgumentNullException("i");
                case "-mf":
                    if(value.ToString().Length >= 1)
                    {
                        FixedMods = value.ToString();
                        return true;
                    }
                    throw new ArgumentNullException("mf");
                case "-x":
                    if(value.ToString().Length >= 1)
                    {
                        TaxIds = value.ToString();
                        return true;
                    }
                    throw new ArgumentNullException("x");
                case "-mv":
                    if(value.ToString().Length >= 1)
                    {
                        VariableMods = value.ToString();
                        return true;
                    }
                    throw new ArgumentNullException("mv");
                case "-tex":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        NeutronThresholdMZ = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("tex");
                case "-w":
                    if(String.IsNullOrEmpty(value.ToString()))
                    {
                        IncludeSearchParamtersInResults = true;
                        return true;
                    }
                    throw new ArgumentNullException("w");
                case "-umm":
                    if(String.IsNullOrEmpty(value.ToString()))
                    {
                        UseMemoryMappedSequences = true;
                        return true;
                    }
                    throw new ArgumentNullException("umm");
                case "-v":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfMissedCleavages = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("v");
                case "-zt":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumPrecursorChargeToConsiderMultiplyChargedProducts = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zt");
                case "-mnm":
                    if(String.IsNullOrEmpty(value.ToString()))
                    {
                        NTermMethionineShouldNotBeCleaved = true;
                        return true;
                    }
                    throw new ArgumentNullException("mnm");
                case "-sct":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        SearchCTerminusIons = (intvalue != 0);
                        return true;
                    }
                    throw new ArgumentNullException("sct");
                case "-sb1":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        SearchFirstForwardProductIon = (intvalue != 0);
                        return true;
                    }
                    throw new ArgumentNullException("sb1");
                case "-zc":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        AlgorithmicallyDetermineChargePlusOne = (intvalue == 1);
                        return true;
                    }
                    throw new ArgumentNullException("zc");
                case "-tom":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        ProductIonSearch = (ProductIonSearchType)intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("tom");
                case "-cp":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        RemoveChargeReducedPrecursor = intvalue == 1;
                        return true;
                    }
                    throw new ArgumentNullException("cp");
                case "-tez":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        ChargeDependencyOfPrecursorMassTolerance = (ChargeDependencyType)intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("tez");
                case "-teppm":
                    if(String.IsNullOrEmpty(value.ToString()))
                    {
                        PrecursorMZToleranceType = PrecursorMZToleranceType.PPM;
                        return true;
                    }
                    throw new ArgumentNullException("teppm");
                case "-ti":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        NumberOfIsotopicPeaksToSearch = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("ti");
                case "-tem":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        PrecursorIonSearch = (PrecursorIonSearchType)intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("tem");
                case "-zoh":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumProductCharge = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zoh");
                case "-zh":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MaximumPrecursorCharge = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zh");
                case "-zl":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        MinimumPrecursorCharge = intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zl");
                case "-zcc":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        PrecursorChargeType = (PrecursorChargeType)intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("zcc");
                case "-e":
                    if(int.TryParse(value.ToString(), out intvalue))
                    {
                        Enzyme = (EnzymeType)intvalue;
                        return true;
                    }
                    throw new ArgumentNullException("e");
                case "-te":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        PrecursorMZTolerance = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("te");
                case "-to":
                    if(double.TryParse(value.ToString(), out dblvalue))
                    {
                        ProductMZTolerance = dblvalue;
                        return true;
                    }
                    throw new ArgumentNullException("to");
                case "-p":
                    if(value.ToString().Length >= 1)
                    {
                        IonSeriesNoProlineRule = value.ToString();
                        return true;
                    }
                    throw new ArgumentNullException("p");
                default:
                    throw new ArgumentException("Is not a valid argument", argument);
            }
            return false; // not a valid argument
        }

        public void SetArgument(string argument, double value)
        {
            this[argument] = value.ToString();
        }

        private void SetArgument(string argument, double value, double defaultvalue)
        {
            if(value == defaultvalue && RemoveDefaults)
            {
                RemoveArgument(argument);
                return;
            }
            SetArgument(argument, value);
        }

        private void SetArgument(string argument, int value, int defaultvalue)
        {
            if(value == defaultvalue && RemoveDefaults)
            {
                RemoveArgument(argument);
                return;
            }
            SetArgument(argument, value);
        }

        private void SetArgument(string argument, string value, string defaultvalue)
        {
            if(value == defaultvalue && RemoveDefaults)
            {
                RemoveArgument(argument);
                return;
            }
            SetArgument(argument, value);
        }

        public bool SetArguments(string argumentline)
        {
            RemoveAllArguments();
            argumentline = argumentline.Replace("%","%");
            MatchCollection matches = LineRegex.Matches(argumentline);
            bool result = true;
            foreach(Match m in matches)
            {
                string arg = m.Groups[1].Value;
                string value = m.Groups[2].Value;
                //SetArgument(arg, value);
                bool r = TrySetArgument(arg, value);
                if(!r) result = false;
            }
            return result;
        }

        public bool UpdateArguments(string argumentline)
        {
            RemoveJustParameters();            
            MatchCollection matches = LineRegex.Matches(argumentline); 
            foreach(Match m in matches)
            {
                string arg = m.Groups[1].Value;
                string value = m.Groups[2].Value;      
                bool r = TrySetArgument(arg, value);
                if(!r)
                {                    
                    return false;
                }
            }
            return true;
        }

        public Dictionary<string, string> GetArgumnts()
        {
            return Arguments;
        }

        public bool TryGetArgument(string argument, out string value)
        {
            string result;
            if(Arguments.TryGetValue(argument, out result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }

        public bool TryGetArgument(string argument, out string value, string defaultvalue)
        {
            bool returnValue = TryGetArgument(argument, out value);
            if(value == defaultvalue)
            {
                RemoveArgument(argument);
                return false;
            }
            return returnValue;
        }

        public bool TryGetArgument(string argument, out double value)
        {
            string result;
            if(Arguments.TryGetValue(argument, out result))
            {
                if(double.TryParse(result, out value))
                {
                    return true;
                }
            }
            value = double.NaN;
            return false;
        }

        private bool TryGetArgument(string argument, out double value, double defaultvalue)
        {
            bool returnValue = TryGetArgument(argument, out value);
            if(value == defaultvalue)
            {
                RemoveArgument(argument);
                return false;
            }
            return returnValue;
        }

        public bool TryGetArgument(string argument, out int value)
        {
            string result;
            if(Arguments.TryGetValue(argument, out result))
            {
                if(int.TryParse(result, out value))
                {
                    return true;
                }
            }
            value = 0;
            return false;
        }

        private bool TryGetArgument(string argument, out int value, int defaultvalue)
        {
            bool returnValue = TryGetArgument(argument, out value);
            if(value == defaultvalue)
            {
                RemoveArgument(argument);
                return false;
            }
            return returnValue;
        }

        public bool RemoveArgument(string argument)
        {
            return Arguments.Remove(argument);
        }

        public void RemoveAllArguments()
        {
            Arguments.Clear();
        }

        public void RemoveJustParameters()
        {
            string mods,input,db,oc,ox,op,fm = null;
            Arguments.TryGetValue("-fx", out input);
            Arguments.TryGetValue("-d", out db);
            Arguments.TryGetValue("-mux", out mods);
            Arguments.TryGetValue("-oc", out oc);
            Arguments.TryGetValue("-ox", out ox);
            Arguments.TryGetValue("-op", out op);
            Arguments.TryGetValue("-fm", out fm);
            RemoveAllArguments();
            if(fm != null) Arguments["-fm"] = fm;
            if(input != null) Arguments["-fx"] = input;
            if(db != null) Arguments["-d"] = db;
            if(mods != null) Arguments["-mux"] = mods;
            if(oc != null) Arguments["-oc"] = oc;
            if(ox != null) Arguments["-ox"] = ox;
            if(op != null) Arguments["-op"] = op;
        }

        public void SetUserModFile(string name)
        {
            UserModFile = name;
        }

        public void SetInputFile(string name)
        {
            switch(Path.GetExtension(name.Replace("\"", String.Empty)))
            {
                case ".dta":
                case ".txt":
                default:
                    SetInputFile(name, InputFileType.MultipleDtaXml);
                    break;
                case ".mgf":
                    SetInputFile(name, InputFileType.Mgf);
                    break;
            }
        }

        public void SetInputFile(string name, InputFileType type)
        {
            string arg = EnumHelper.GetDescription(type);
            SetArgument(arg, name);
        }

        public string SetOutputFile(string name)
        {
            return SetOutputFile(name, OutputFileType.Csv);
        }        

        public string SetOutputFile(string name, OutputFileType type)
        {
            string arg = EnumHelper.GetDescription(type);
           
            switch(type)
            {
                case OutputFileType.Csv:
                    if(Path.HasExtension(name))
                    {
                        name = Path.ChangeExtension(name, "csv");
                    }
                    else
                    {
                        name += ".csv";
                    }
                    break;
                case OutputFileType.Xml:
                    if(Path.HasExtension(name))
                    {
                        name = Path.ChangeExtension(name, "xml");
                    }
                    else
                    {
                        name += ".xml";
                    }
                    break;
                case OutputFileType.PepXml:
                    if(Path.HasExtension(name))
                    {
                        name = Path.ChangeExtension(name, "pepXML");
                    }
                    else
                    {
                        name += ".pepXML";
                    }
                    break;
            }
            SetArgument(arg, name);
            return name;
        }

        public void ClearOutputFileTypes()
        {
            RemoveArgument("-oc");
            RemoveArgument("-ox");
            RemoveArgument("-op");
        }

        public void SetDataBase(string name)
        {
            Database = name;
            //Database = Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name));
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToString(bool justParameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, string> kvp in Arguments)
            {
                if(justParameters && (kvp.Key == "-fx" || kvp.Key == "-d" || kvp.Key == "-mux" || kvp.Key == "-oc" || kvp.Key == "-ox" || kvp.Key == "-op"))
                    continue;
                sb.Append(" ");
                sb.Append(kvp.Key);
                if(kvp.Value != null && kvp.Value != string.Empty && kvp.Value != string.Empty)
                {
                    sb.Append(" ");
                    //sb.Append("\string.Empty);
                    sb.Append(kvp.Value);
                    //sb.Append("\string.Empty);
                }
            }
            if(sb.Length > 1) sb.Remove(0, 1);    // remove the first " "
            return sb.ToString();
        }
    }
}