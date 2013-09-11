namespace Coon.Compass.FdrOptimizer
{
    public class AminoAcid
    {
        private char abbreviation;

        public char Abbreviation
        {
            get { return abbreviation; }
            set { abbreviation = value; }
        }

        private double mass;

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public AminoAcid(char abbreviation, double mass)
        {
            this.abbreviation = abbreviation;
            this.mass = mass;
        }
    }
}
