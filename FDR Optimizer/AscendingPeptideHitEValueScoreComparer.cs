using System.Collections.Generic;

namespace Coon.Compass.FdrOptimizer
{
    public class AscendingPeptideHitEValueScoreComparer : Comparer<PeptideHit>
    {
        private bool higherScoresAreBetter;

        public AscendingPeptideHitEValueScoreComparer(bool higherScoresAreBetter)
        {
            this.higherScoresAreBetter = higherScoresAreBetter;
        }

        public override int Compare(PeptideHit x, PeptideHit y)
        {
            return higherScoresAreBetter ? -(x.EValueScore.CompareTo(y.EValueScore)) : x.EValueScore.CompareTo(y.EValueScore);
        }
    }
}
