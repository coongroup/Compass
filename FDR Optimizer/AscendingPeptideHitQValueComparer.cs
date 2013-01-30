using System.Collections.Generic;

namespace FdrOptimizer
{
    public class AscendingPeptideHitQValueComparer : Comparer<PeptideHit>
    {
        private bool higherScoresAreBetter;

        public AscendingPeptideHitQValueComparer(bool higherScoresAreBetter)
        {
            this.higherScoresAreBetter = higherScoresAreBetter;
        }

        public override int Compare(PeptideHit x, PeptideHit y)
        {
            int comparison = x.QValue.CompareTo(y.QValue);
            if(comparison != 0)
            {
                return comparison;
            }
            else
            {
                int comparision2 = x.EValueScore.CompareTo(y.EValueScore);
                if (comparision2 == 0)
                {
                    if (x.Decoy && !y.Decoy)
                    {
                        return 1;
                    }
                    if (!x.Decoy && y.Decoy)
                    {
                        return -1;
                    }
                    return 0;
                }
                return higherScoresAreBetter ? -comparision2 : comparision2;
            }
        }
    }
}
