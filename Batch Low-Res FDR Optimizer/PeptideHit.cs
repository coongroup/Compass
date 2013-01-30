using System;

namespace BatchLowResFdrOptimizer
{
    public class PeptideHit
    {
        protected const double PROTON_MASS = 1.00727638;
        protected const double C12_C13_MASS_DIFFERENCE = 1.0033548378;

        protected string sequence;

        public string Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        protected string dynamicModifications;

        public string DynamicModifications
        {
            get { return dynamicModifications; }
            set { dynamicModifications = value; }
        }

        protected double eValueScore;

        public double EValueScore
        {
            get { return eValueScore; }
            set { eValueScore = value; }
        }

        protected bool decoy;

        public bool Decoy
        {
            get { return decoy; }
            set { decoy = value; }
        }

        protected double qValue;

        public double QValue
        {
            get { return qValue; }
            set { qValue = value; }
        }

        public PeptideHit(string sequence, string dynamicModifications, double eValueScore, bool decoy)
        {
            this.sequence = sequence;
            this.dynamicModifications = dynamicModifications;
            this.eValueScore = eValueScore;
            this.decoy = decoy;
            this.qValue = double.NaN;
        }

        public PeptideHit(string sequence, string dynamicModifications, double eValueScore, bool decoy, double qValue)
        {
            this.sequence = sequence;
            this.dynamicModifications = dynamicModifications;
            this.eValueScore = eValueScore;
            this.decoy = decoy;
            this.qValue = qValue;
        }
    }
}