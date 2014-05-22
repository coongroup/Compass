using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.OMSSA;
using CSMSL.IO.Thermo;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util.Collections;
using LumenWorks.Framework.IO.Csv;

namespace Coon.Compass.FdrOptimizer
{
    public class InputFile
    {
        public string FilePath { get; private set; }

        public string Name
        {
            get { return Path.GetFileName(FilePath); }
        }

        public string RawFilePath { get; set; }

        public string RawFileName { get; private set; }

        public bool IsBatched { get; set; }

        public InputFile(string filePath)
        {
            FilePath = filePath;
            HasPPMInfo = false;
            IsBatched = false;
            _data = new Dictionary<int, SortedMaxSizedContainer<PSM>>();
            PeptideSpectralMatches = new List<PSM>();
            SystematicPrecursorMassError = double.NaN;
            MaximumPrecursorMassError = double.NaN;
            PrecursorMassToleranceThreshold = double.NaN;
        }

        public int TotalScans
        {
            get { return _data.Count; }
        }

        public List<PSM> PeptideSpectralMatches;

        public List<Peptide> Peptides;

        public List<Peptide> FdrFilteredPeptides;

        public List<PSM> FdrFilteredPSMs;

        public List<InputFile> SubInputFiles { get; set; }

        public bool HasPPMInfo { get; set; }

        public int PsmCount
        {
            get
            {
                return PeptideSpectralMatches.Count;
            }
        }

        public double AverageMSMSInjectionTime { get; set; }

        public double MaxMSMSInjectionTime { get; set; }

        public double MinMSMSInjectionTime { get; set; }

        public int TotalMSMSscans { get; set; }

        public int TotalMSscans { get; set; }

        public int MaxMSMSScansBetweenMS {get; set;}

        public double AverageMSMSSCansBetweenMS {get;set;}

        public double SystematicPrecursorMassError { get; private set; }

        public double MaximumPrecursorMassError { get; set; }

        public double ScoreThreshold { get; set; }

        public double PrecursorMassToleranceThreshold { get; set; }

        private readonly Dictionary<int, SortedMaxSizedContainer<PSM>> _data;
        
        public void Read(IList<Modification> fixedModifications, int numberOfTopHits = 1, bool higherScoresAreBetter = false)
        {
            _data.Clear();
            bool first = true;
            using (CsvReader reader = new CsvReader(new StreamReader(FilePath), true))
            {
                string[] headers = reader.GetFieldHeaders();
                HasPPMInfo = headers.Contains("Precursor Mass Error (ppm)");
                while (reader.ReadNextRecord())
                {
                    if (first)
                    {
                        RawFileName = reader["Filename/id"].Split('.')[0];
                        first = false;
                    }

                    int scanNumber = int.Parse(reader["Spectrum number"]);

                    PSM psm = new PSM(scanNumber) {Score = double.Parse(reader["E-value"])};
                    SortedMaxSizedContainer<PSM> peptides;
                    if (!_data.TryGetValue(scanNumber, out peptides))
                    {
                        peptides = new SortedMaxSizedContainer<PSM>(numberOfTopHits);
                        _data.Add(scanNumber, peptides);
                    }

                    if (!peptides.Add(psm))
                        continue;
                    psm.FileName = reader["Filename/id"];
                    psm.Charge = int.Parse(reader["Charge"]);
                    psm.IsDecoy = reader["Defline"].StartsWith("DECOY_");
                    if (HasPPMInfo)
                        psm.PrecursorMassError = double.Parse(reader["Precursor Mass Error (ppm)"]);
                    psm.SetSequenceAndMods(reader["Peptide"].ToUpper(), fixedModifications, reader["Mods"]);
                }
            }

            PeptideSpectralMatches.Clear();
            foreach (SortedMaxSizedContainer<PSM> set in _data.Values)
            {
                PeptideSpectralMatches.AddRange(set);
            }
        }

        public void UpdatePsmInformation(MSDataFile dataFile, bool is2dFDR = true, bool useMedian = true, double evalueThreshold = 1e-3)
        {
            List<double> errors = new List<double>();
            MaximumPrecursorMassError = 0;
            int count = 0;
            int msms = 0;
            int ms = 0;
            int localMSMS = -1;
            List<int> msmsBetweenMS = new List<int>();
            List<double> injectionTimes = new List<double>();
            for(int sn = dataFile.FirstSpectrumNumber; sn <= dataFile.LastSpectrumNumber; sn++)
            {
                int order = dataFile.GetMsnOrder(sn);
                if (order == 1)
                {
                    ms++;                
                    if(localMSMS >= 0)
                        msmsBetweenMS.Add(localMSMS);
                    localMSMS = 0;
                }
                else
                {
                    localMSMS++;
                    msms++;
                    injectionTimes.Add(dataFile.GetInjectionTime(sn));
                }
            }
            MaxMSMSScansBetweenMS = msmsBetweenMS.Max();
            AverageMSMSSCansBetweenMS = msmsBetweenMS.Average();
            TotalMSMSscans = msms;
            TotalMSscans = ms;
            AverageMSMSInjectionTime = injectionTimes.Average();
            MaxMSMSInjectionTime = injectionTimes.Max();
            
            foreach (KeyValuePair<int, SortedMaxSizedContainer<PSM>> kvp in _data)
            {
                int scanNumber = kvp.Key;
                SortedMaxSizedContainer<PSM> psms = kvp.Value;
               
                double isolationMZ = dataFile.GetPrecusorMz(scanNumber);
                Polarity polarity = dataFile.GetPolarity(scanNumber);

                foreach (PSM psm in psms)
                {
                    psm.Charge *= (int)polarity; // For negative mode ions
                    psm.IsolationMz = isolationMZ;
                    double isolationMass = Mass.MassFromMz(isolationMZ, psm.Charge);
                    double theoreticalMass = psm.MonoisotopicMass;
                    int nominalMassOffset;
                    double adjustedIsolationMass;
                    Tolerance tolerancePPM = Tolerance.CalculatePrecursorMassError(theoreticalMass,
                        isolationMass, out nominalMassOffset, out adjustedIsolationMass);
                    psm.AdjustedIsolationMass = adjustedIsolationMass;
                    psm.IsotopeSelected = nominalMassOffset;
                    if(!HasPPMInfo)
                        psm.PrecursorMassError = tolerancePPM.Value;
                                      
                    double positive = Math.Abs(psm.PrecursorMassError);
                    if (positive > MaximumPrecursorMassError)
                    {
                        MaximumPrecursorMassError = positive;
                    }

                    if (psm.FdrScoreMetric <= evalueThreshold)
                    {
                        errors.Add(psm.PrecursorMassError);
                    }

                    count++;
                }
            }
            
            SystematicPrecursorMassError = useMedian ? GetMedianValue(errors) : errors.Average();

            // Adjust all psms to 
            foreach (PSM psm in PeptideSpectralMatches)
            {
                psm.CorrectedPrecursorMassError = psm.PrecursorMassError - SystematicPrecursorMassError;
            }
        }

        public void ReducePsms(IEqualityComparer<Peptide> comparer)
        {
            Dictionary<Peptide, Peptide> peptides = new Dictionary<Peptide, Peptide>(comparer);
            foreach (PSM psm in PeptideSpectralMatches)
            {
                Peptide peptide = new Peptide(psm);
                Peptide realPeptide;
                if (peptides.TryGetValue(peptide, out realPeptide))
                {
                    realPeptide.AddPsm(psm);
                }
                else
                {
                    peptides.Add(peptide, peptide);
                }
            }
            Peptides = peptides.Values.ToList();
        }

        public static double GetMedianValue(List<double> values)
        {
            int count = values.Count;
            int midIndex = count / 2;
            values.Sort();
            double medianError;
            if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = values[midIndex - 1];
                double b = values[midIndex];
                medianError = (a + b) / 2.0;
            }
            else
            {
                // count is odd, return the middle element
                medianError = values[midIndex];
            }
            return medianError;
        }
    }
}
