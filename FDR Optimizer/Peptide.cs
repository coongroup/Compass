using System;
using System.Collections.Generic;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;

namespace Coon.Compass.FdrOptimizer
{
    public class Peptide : IFalseDiscovery<double>, IComparable<Peptide>, IMass
    {
        private List<PeptideSpectralMatch> PSMs;

        private PeptideSpectralMatch _bestMatch;
        public PeptideSpectralMatch BestMatch {
            get
            {
                return _bestMatch;
            }
            set
            {
                _bestMatch = value;
                LeucineSequence = value.Peptide.GetLeucineSequence();
                MonoisotopicMass = value.Peptide.MonoisotopicMass;
            }
        }

        public double MonoisotopicMass { get; private set; }

        public string LeucineSequence { get; private set; }

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
            return x.LeucineSequence.Equals(y.LeucineSequence);
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.LeucineSequence.GetHashCode();
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

    public class MassComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.MassEquals(y);
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
            if (!x.LeucineSequence.Equals(y.LeucineSequence))
                return false;
            IMass[] xMods = x.BestMatch.Peptide.GetModifications();
            IMass[] yMods = y.BestMatch.Peptide.GetModifications();
            for (int i = 0; i < xMods.Length; i++)
            {
                if (!Equals(xMods[i], yMods[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.LeucineSequence.GetHashCode();
        }
    }

    public class SequenceMassComparer : IEqualityComparer<Peptide>
    {
        public bool Equals(Peptide x, Peptide y)
        {
            return x.LeucineSequence.Equals(y.LeucineSequence) && x.MassEquals(y);
        }

        public int GetHashCode(Peptide obj)
        {
            return obj.LeucineSequence.GetHashCode();
        }
    }
}
