using System;
using System.Collections.Generic;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;

namespace Coon.Compass.FdrOptimizer
{
    public class PeptideHit : IComparer<PeptideHit>,  IFalseDiscovery<double>
    {
        private const double PROTON_MASS = Constants.Proton;
        private const double C12_C13_MASS_DIFFERENCE = Constants.Carbon13 - Constants.Carbon;

        private string line;
        public string Line {
            get
            {
                return line;
            }
            set
            {
                line = value;               
            }
        }

        private string extendedLine = null;
        public string ExtendedLine
        {
            get
            {
                if (extendedLine == null)
                {
                    extendedLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", Line, IsolationMZ, IsolationMass, TheoreticalNeutralMass, ExperimentalNeutralMass, PrecursorMassError, AdjustedPrecursorMassError, QValue);
                }
                return extendedLine;
            }
        }

        public string Sequence { get; set; } 

        public string DynamicModifications { get; set; }

        public double EValueScore { get; set; }      

        public bool IsDecoy { get; set; }

        public double IsolationMZ { get; set; }

        public double IsolationMass { get; set; }

        public double TheoreticalNeutralMass { get; set; }

        public double ExperimentalNeutralMass { get; set; }

        public double PrecursorMassError { get; set; }

        public double AdjustedPrecursorMassError { get; set; }

        public double QValue { get; set; }
      
        public PeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool isDecoy, 
            Peptide2 peptide, List<double> ms1peaks, double headermz, int charge)
        {
            Line = line;
            Sequence = sequence;
            DynamicModifications = dynamicModifications;
            EValueScore = eValueScore;
            IsDecoy = isDecoy;
            TheoreticalNeutralMass = peptide.Mass;
            CalculatePrecursorMassErrorFromIsolation(ms1peaks, headermz, charge);
            QValue = double.NaN;
        }

        private void CalculatePrecursorMassErrorFromIsolation(List<double> ms1peaks, double headermz, int charge)
        {                  
            //double smallestDiff = double.MaxValue;
            double bestmatch = headermz;
            //int index = ms1peaks.BinarySearch(headermz);
            //if (index < 0)
            //{
            //    int minindex = ~index - 1;
            //    if (minindex < 0) minindex = 0;
            //    for (int i = minindex; i < minindex + 3 && i < ms1peaks.Count; i++)
            //    {
            //        double mz = ms1peaks[i];
            //        double mz_diff = headermz - mz;
            //        double mz_diff_abs = Math.Abs(mz_diff);
            //        if (mz_diff_abs < smallestDiff)
            //        {
            //            smallestDiff = mz_diff_abs;
            //            bestmatch = mz;
            //        }
            //    }
            //}
            //else
            //{
            //    bestmatch = ms1peaks[index];
            //}
            IsolationMZ = bestmatch;  
            IsolationMass = Mass.MassFromMz(IsolationMZ, charge);           
            double mass_error = IsolationMass - TheoreticalNeutralMass;
            double mass_offset = Math.Round(mass_error / C12_C13_MASS_DIFFERENCE) * C12_C13_MASS_DIFFERENCE;
            ExperimentalNeutralMass = IsolationMass - mass_offset;
            mass_error = ExperimentalNeutralMass - TheoreticalNeutralMass;
            PrecursorMassError = MassTolerance.GetTolerance(ExperimentalNeutralMass, TheoreticalNeutralMass, MassToleranceType.PPM);                 
        }

        public static int CompareDecreasing(PeptideHit x, PeptideHit y)
        {
            return Compare(x, y, -1);
        }

        public static int CompareIncreasing(PeptideHit x, PeptideHit y)
        {
            return Compare(x, y, 1);
        }

        public static int Compare(PeptideHit x, PeptideHit y, int direction)
        {
            if (x.EValueScore.Equals(y.EValueScore))
            {
                if (x.IsDecoy)
                    return 1;
                if (y.IsDecoy)
                    return -1;
                return 0;
            }
            direction = Math.Sign(direction);
            return x.EValueScore.CompareTo(y.EValueScore)*direction;
        }

        public int Compare(PeptideHit x, PeptideHit y)
        {
            return Compare(x, y, 1);
        }

        double IFalseDiscovery<double>.FdrScoreMetric
        {
            get { return EValueScore; }
        }
    }
}
