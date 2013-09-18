using System;
using System.Collections.Generic;

namespace Phosphinator
{
    public class PhosphopeptideStatistics
    {
        private Peptide peptide;

        public Peptide Peptide
        {
            get { return peptide; }
            set { peptide = value; }
        }

        private Dictionary<string, Dictionary<int, bool>> fragments;

        public Dictionary<string, Dictionary<int, bool>> Fragments
        {
            get { return fragments; }
            set { fragments = value; }
        }

        public int NumberOfMatchingFragments
        {
            get
            {
                int matching_fragments = 0;

                foreach(Dictionary<int, bool> fragment in fragments.Values)
                {
                    foreach(KeyValuePair<int, bool> kvp in fragment)
                    {
                        if(kvp.Value)
                        {
                            matching_fragments++;
                            break;
                        }
                    }
                }

                return matching_fragments;
            }
        }

        public int NumberOfTotalFragments
        {
            get { return fragments.Count; }
        }

        public int NumberOfMatchingFragmentIons
        {
            get
            {
                int matching_fragments = 0;

                foreach(Dictionary<int, bool> fragment in fragments.Values)
                {
                    foreach(KeyValuePair<int, bool> kvp in fragment)
                    {
                        if(kvp.Value)
                        {
                            matching_fragments++;
                        }
                    }
                }

                return matching_fragments;
            }
        }

        public int NumberOfTotalFragmentIons
        {
            get
            {
                int total_fragments = 0;

                foreach(Dictionary<int, bool> fragment in fragments.Values)
                {
                    total_fragments += fragment.Count;
                }

                return total_fragments;
            }
        }

        public PhosphopeptideStatistics(Peptide peptide)
        {
            this.peptide = peptide;
            fragments = new Dictionary<string, Dictionary<int, bool>>(2 * (peptide.Sequence.Length - 1));
        }
    }
}
