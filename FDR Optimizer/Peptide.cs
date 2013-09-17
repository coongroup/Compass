using System;
using System.Collections.Generic;
using CSMSL.Analysis.Identification;

namespace Coon.Compass.FdrOptimizer
{
    public class Peptide : IFalseDiscovery<double>, IComparable<Peptide>
    {
        private List<PeptideSpectralMatch> PSMs;

        public PeptideSpectralMatch BestMatch;

        public int Count
        {
            get { return PSMs.Count; }
        }

        public Peptide(PeptideSpectralMatch psm)
        {
            PSMs = new List<PeptideSpectralMatch>();
            IsDecoy = false;
            AddPsm(psm);
        }

        public void AddPsm(PeptideSpectralMatch psm)
        {
            if (BestMatch == null)
            {
                BestMatch = psm;
            }
            else
            {
                if (psm.Score < BestMatch.Score)
                {
                    BestMatch = psm;
                } else if (psm.Score == BestMatch.Score)
                {
                    if (psm.CorrectedPrecursorMassError.Value < BestMatch.CorrectedPrecursorMassError.Value)
                        BestMatch = psm;
                }
            }
            if (psm.IsDecoy)
                IsDecoy = true;
            PSMs.Add(psm);
        }

        public double CorrectedPrecursorErrorPPM
        {
            get
            {
                return BestMatch.CorrectedPrecursorMassError.Value;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} (e-Value: {1:g4}{2})", BestMatch.Peptide, FdrScoreMetric, IsDecoy ? " Decoy" : "");
        }

        public bool IsDecoy
        {
            get; private set;
        }

        public double FdrScoreMetric
        {
            get { return BestMatch.Score; }
        }

        public int CompareTo(Peptide other)
        {
            return BestMatch.CompareTo(other.BestMatch);
        }
    }

    public class SequenceComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.BestMatch.Peptide.Sequence.Equals(y.BestMatch.Peptide.Sequence);
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.BestMatch.Peptide.Sequence.GetHashCode();
        }
    }

    public class IdentityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }

    public class SequenceILComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.BestMatch.Peptide.GetLeucineSequence().Equals(y.BestMatch.Peptide.GetLeucineSequence());
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.BestMatch.Peptide.GetLeucineSequence().GetHashCode();
        }
    }

    public class MassComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return Math.Abs(x.BestMatch.MonoisotopicMass - y.BestMatch.MonoisotopicMass) < 0.0000001;
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.BestMatch.MonoisotopicMass.GetHashCode();
        }
    }

    public class SequenceModComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.BestMatch.Peptide.Equals(y.BestMatch.Peptide);
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.BestMatch.Peptide.GetHashCode();
        }
    }

    public class SequenceMassComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.BestMatch.Peptide.Sequence.Equals(y.BestMatch.Peptide.Sequence) &&
                   x.BestMatch.Peptide.MonoisotopicMass.Equals(y.BestMatch.Peptide.MonoisotopicMass);
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.BestMatch.Peptide.Sequence.GetHashCode() + obj.BestMatch.Peptide.MonoisotopicMass.GetHashCode();
        }
    }
}
