using System;
using System.Collections.Generic;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;

namespace Coon.Compass.FdrOptimizer
{
    public class PeptideSpectralMatch2 : IComparable<PeptideSpectralMatch2>, IComparer<PeptideSpectralMatch2>, IFalseDiscovery<double>, IMass
    {
        public int ScanNumber { get; set; }
        public double Evalue { get; set; }
        public string Sequence { get; set; }
        public bool IsDecoy { get; set; }
        public int Charge { get; set; }
        public double PrecursorMZ { get; set; }
        public MassTolerance PrecursorError { get; set; }

        public double MonoisotopicMass
        {
            get { return Peptide.Mass; }
        }

        public Peptide2 Peptide { get; private set; }

        public PeptideSpectralMatch2(int scanNumber, double evalue, Peptide2 peptide, bool isDecoy, int charge)
        {
            ScanNumber = scanNumber;
            Evalue = evalue;
            IsDecoy = isDecoy;
            Charge = charge;
            Peptide = peptide;
        }

        double IFalseDiscovery<double>.FdrScoreMetric
        {
            get { return Evalue; }
        }

        public int Compare(PeptideSpectralMatch2 x, PeptideSpectralMatch2 y)
        {
            if (!x.Evalue.Equals(y.Evalue)) 
                return x.Evalue.CompareTo(y.Evalue);
            if (x.IsDecoy)
                return 1;
            if (y.IsDecoy)
                return -1;
            return 0;
        }

        public int CompareTo(PeptideSpectralMatch2 other)
        {
            return Compare(this, other);
        }

        private const double C12C13Difference = Constants.Carbon13 - Constants.Carbon;
      
        public double UpdatePrecursorError(double observedMZ)
        {
            PrecursorMZ = observedMZ;
            double isolationMass = Mass.MassFromMz(PrecursorMZ, Charge);
            double massError = isolationMass - MonoisotopicMass;
            double massOffset = Math.Round(massError / C12C13Difference) * C12C13Difference;
            double experimentalNeutralMass = isolationMass - massOffset;
            massError = experimentalNeutralMass - MonoisotopicMass;
            PrecursorError = new MassTolerance(MassToleranceType.PPM, experimentalNeutralMass, MonoisotopicMass);
            return PrecursorError.Value;
        }
    }
}
