using System.Collections.Generic;

namespace BatchFdrOptimizer
{
    public class AscendingFullPeptideHitEValueScoreComparer : Comparer<FullPeptideHit>
    {
        private bool higherScoresAreBetter;

        public AscendingFullPeptideHitEValueScoreComparer(bool higherScoresAreBetter)
        {
            this.higherScoresAreBetter = higherScoresAreBetter;
        }

        public override int Compare(FullPeptideHit x, FullPeptideHit y)
        {
            return higherScoresAreBetter ? -(x.EValueScore.CompareTo(y.EValueScore)) : x.EValueScore.CompareTo(y.EValueScore);
        }
    }
}
