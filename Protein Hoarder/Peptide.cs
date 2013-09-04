using System;
using System.Collections.Generic;

namespace Compass.ProteinHoarder
{
    public class Peptide : IEquatable<Peptide>
    {
        public static int MappedCount = 0;

        public ProteinGroup BestPG = null;
        internal bool IsMapped = false;

        private readonly int _hCode;

        public Peptide(string sequence)
        {
            LeucineSequence = sequence;
            _hCode = sequence.GetHashCode();
            PSMs = new PsmList();
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
                return LeucineSequence.Length;
            }
        }

        public string LeucineSequence { get; private set; }

        public int NumberOfSharingProteinGroups
        {
            get
            {
                return ProteinGroups != null ? ProteinGroups.Count : 0;
            }
        }

        public List<ProteinGroup> ProteinGroups { get; private set; }

        public PsmList PSMs { get; set; }

        public bool Equals(Peptide other)
        {
            return ReferenceEquals(this, other) || LeucineSequence.Equals(other.LeucineSequence);
        }

        public override int GetHashCode()
        {
            return _hCode;
        }

        public void MarkAsMapped()
        {
            if (IsMapped) 
                return;
            MappedCount++;
            IsMapped = true;
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