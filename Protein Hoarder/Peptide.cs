using System;
using System.Collections.Generic;

namespace Protein_Hoarder
{
    public class Peptide : IEquatable<Peptide>
    {
        public static int MappedCount = 0;

        public ProteinGroup BestPG = null;
        internal bool mapped = false;

        private int hCode;

        private string _leucineSequence;

        public Peptide(string sequence)
        {
            LeucineSequence = sequence;
            PSMs = new PsmList();
        }

        public Peptide(string sequence, PSM psm)
        {
            LeucineSequence = sequence;
            PSMs = new PsmList(psm);
        }

        public bool IsShared
        {
            get
            {
                return NumberOfSharingProteinGroups > 1;
            }
        }

        public int Length
        {
            get
            {
                return _leucineSequence.Length;
            }
        }

        public string LeucineSequence
        {
            get
            {
                return _leucineSequence;
            }
            private set
            {
                _leucineSequence = value;
                hCode = _leucineSequence.GetHashCode();
            }
        }

        public int NumberOfSharingProteinGroups
        {
            get
            {
                if (ProteinGroups != null)
                {
                    return ProteinGroups.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<ProteinGroup> ProteinGroups { get; private set; }

        public PsmList PSMs { get; set; }

        public bool Equals(Peptide other)
        {
            if (ReferenceEquals(this, other)) return true;
            return LeucineSequence.Equals(other.LeucineSequence);
        }

        public override int GetHashCode()
        {
            return hCode;// LeucineSequence.GetHashCode();
        }

        public void MarkAsMapped()
        {
            if (!mapped)
            {
                MappedCount++;
                mapped = true;
            }
        }

        public override string ToString()
        {
            return LeucineSequence;
        }

        internal void AddProteinGroup(ProteinGroup pg)
        {
            if (ProteinGroups == null)
                ProteinGroups = new List<ProteinGroup>();
            ProteinGroups.Add(pg);
        }
    }
}