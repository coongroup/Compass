using System;

namespace LowResFdrOptimizer
{
    public class PeptideHit
    {
        private string line;

        public string Line
        {
            get { return line; }
            set { line = value; }
        }

        public string ExtendedLine
        {
            get { return line + ',' + qValue.ToString("R"); }
        }

        private string sequence;

        public string Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        private string dynamicModifications;

        public string DynamicModifications
        {
            get { return dynamicModifications; }
            set { dynamicModifications = value; }
        }

        private double eValueScore;

        public double EValueScore
        {
            get { return eValueScore; }
            set { eValueScore = value; }
        }

        private bool decoy;

        public bool Decoy
        {
            get { return decoy; }
            set { decoy = value; }
        }

        private double qValue;

        public double QValue
        {
            get { return qValue; }
            set { qValue = value; }
        }

        public PeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool decoy)
        {
            this.line = line;
            this.sequence = sequence;
            this.dynamicModifications = dynamicModifications;
            this.eValueScore = eValueScore;
            this.decoy = decoy;
            this.qValue = double.NaN;
        }
    }
}
