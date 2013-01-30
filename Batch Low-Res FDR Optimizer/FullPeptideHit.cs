using System;

namespace BatchLowResFdrOptimizer
{
    public class FullPeptideHit : PeptideHit
    {
        private bool lineHasExtendedInformation;

        private string line;

        public string Line
        {
            get { return line; }
            set { line = value; }
        }

        public string ExtendedLine
        {
            get { return lineHasExtendedInformation ? line : line + ',' + qValue.ToString("R"); }
        }

        public FullPeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool decoy)
            : base(sequence, dynamicModifications, eValueScore, decoy)
        {
            this.line = line;
            lineHasExtendedInformation = false;
        }

        public FullPeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool decoy, double qValue)
            : base(sequence, dynamicModifications, eValueScore, decoy, qValue)
        {
            this.line = line;
            lineHasExtendedInformation = true;
        }
    }
}
