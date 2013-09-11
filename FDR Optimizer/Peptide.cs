using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
                    BestMatch = psm;
            }
            if (psm.IsDecoy)
                IsDecoy = true;
            PSMs.Add(psm);
        }

        public double PrecursorErrorPPM
        {
            get
            {
                return BestMatch.PrecursorMassError.Value;
            }
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
