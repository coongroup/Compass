using System.Collections.Generic;

namespace LowResFdrOptimizer
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
                return higherScoresAreBetter ? -(x.EValueScore.CompareTo(y.EValueScore)) : x.EValueScore.CompareTo(y.EValueScore);
            }
        }
    }
}
