using System;
using MSFileReaderLib;

namespace BatchFdrOptimizer
{
    public class PeptideHit
    {
        protected const double PROTON_MASS = 1.00727638;
        protected const double C12_C13_MASS_DIFFERENCE = 1.0033548378;

        protected string sequence;

        public string Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        protected string dynamicModifications;

        public string DynamicModifications
        {
            get { return dynamicModifications; }
            set { dynamicModifications = value; }
        }

        protected double eValueScore;

        public double EValueScore
        {
            get { return eValueScore; }
            set { eValueScore = value; }
        }

        protected bool decoy;

        public bool Decoy
        {
            get { return decoy; }
            set { decoy = value; }
        }

        protected double isolationMZ;

        public double IsolationMZ
        {
            get { return isolationMZ; }
            set { isolationMZ = value; }
        }

        protected double isolationMass;

        public double IsolationMass
        {
            get { return isolationMass; }
            set { isolationMass = value; }
        }

        protected double theoreticalNeutralMass;

        public double TheoreticalNeutralMass
        {
            get { return theoreticalNeutralMass; }
            set { theoreticalNeutralMass = value; }
        }

        protected double experimentalNeutralMass;

        public double ExperimentalNeutralMass
        {
            get { return experimentalNeutralMass; }
            set { experimentalNeutralMass = value; }
        }

        protected double precursorMassError;

        public double PrecursorMassError
        {
            get { return precursorMassError; }
            set { precursorMassError = value; }
        }

        protected double adjustedPrecursorMassError;

        public double AdjustedPrecursorMassError
        {
            get { return adjustedPrecursorMassError; }
            set { adjustedPrecursorMassError = value; }
        }

        protected double qValue;

        public double QValue
        {
            get { return qValue; }
            set { qValue = value; }
        }

        public PeptideHit(string sequence, string dynamicModifications, double eValueScore, bool decoy, 
            Peptide peptide, IXRawfile2 raw, int scanNumber, int firstScanNumber, int charge)
        {
            this.sequence = sequence;
            this.dynamicModifications = dynamicModifications;
            this.eValueScore = eValueScore;
            this.decoy = decoy;
            this.theoreticalNeutralMass = peptide.Mass;
            CalculatePrecursorMassError(raw, scanNumber, firstScanNumber, charge);
            this.qValue = double.NaN;
        }

        public PeptideHit(string sequence, string dynamicModifications, double eValueScore, bool decoy, double isolationMZ, double isolationMass, double theoreticalNeutralMass, double experimentalNeutralMass, double precursorMassError, double adjustedPrecursorMassError, double qValue)
        {
            this.sequence = sequence;
            this.dynamicModifications = dynamicModifications;
            this.eValueScore = eValueScore;
            this.decoy = decoy;
            this.isolationMZ = isolationMZ;
            this.isolationMass = isolationMass;
            this.theoreticalNeutralMass = theoreticalNeutralMass;
            this.experimentalNeutralMass = experimentalNeutralMass;
            this.precursorMassError = precursorMassError;
            this.adjustedPrecursorMassError = adjustedPrecursorMassError;
            this.qValue = qValue;
        }

        protected void CalculatePrecursorMassError(IXRawfile2 raw, int scanNumber, int firstScanNumber, int charge)
        {
            int survey_scan_number = scanNumber;
            while(survey_scan_number >= firstScanNumber)
            {
                string temp_survey_scan_filter = null;
                raw.GetFilterForScanNum(survey_scan_number, ref temp_survey_scan_filter);
                if(!temp_survey_scan_filter.Contains(" ms ") || !temp_survey_scan_filter.Contains("FTMS"))
                {
                    survey_scan_number--;
                }
                else
                {
                    break;
                }
            }

            object labels_obj = null;
            object flags_obj = null;
            raw.GetLabelData(ref labels_obj, ref flags_obj, ref survey_scan_number);

            double[,] labels = (double[,])labels_obj;
            byte[,] flags = (byte[,])flags_obj;

            string scan_filter = null;
            raw.GetFilterForScanNum(scanNumber, ref scan_filter);
            string temp_scan_filter = scan_filter.Substring(0, scan_filter.IndexOf('@'));
            isolationMZ = double.Parse(temp_scan_filter.Substring(temp_scan_filter.LastIndexOf(' ') + 1));

            int? isolation_index = null;
            for(int i = labels.GetLowerBound(1); i <= labels.GetUpperBound(1); i++)
            {
                if(!isolation_index.HasValue || Math.Abs(labels[(int)RawLabelDataColumn.MZ, i] - isolationMZ) < Math.Abs(labels[(int)RawLabelDataColumn.MZ, isolation_index.Value] - isolationMZ))
                {
                    isolation_index = i;
                }
            }
            if(!isolation_index.HasValue)
            {
                throw new Exception("Could not find isolated precursor peak.");
            }
            isolationMZ = labels[(int)RawLabelDataColumn.MZ, isolation_index.Value];

            isolationMass = MassFromMZAndCharge(isolationMZ, charge);
            double mass_error = isolationMass - theoreticalNeutralMass;
            double mass_offset = Math.Round(mass_error / C12_C13_MASS_DIFFERENCE) * C12_C13_MASS_DIFFERENCE;
            experimentalNeutralMass = isolationMass - mass_offset;
            mass_error = experimentalNeutralMass - theoreticalNeutralMass;
            precursorMassError = mass_error / theoreticalNeutralMass * 1e6;
        }

        protected static double MZFromMassAndCharge(double mass, int charge)
        {
            return (mass + charge * PROTON_MASS) / Math.Abs(charge);
        }

        protected static double MassFromMZAndCharge(double mz, int charge)
        {
            return mz * Math.Abs(charge) - charge * PROTON_MASS;
        }
    }
}
