namespace Coon.Compass.DtaGenerator
{
    public class MSPeak
    {
        protected double mz;

        public double MZ
        {
            get { return mz; }
            set { mz = value; }
        }

        protected double intensity;

        public double Intensity
        {
            get { return intensity; }
            set { intensity = value; }
        }

        public MSPeak() { }

        public MSPeak(double mz, double intensity)
        {
            this.mz = mz;
            this.intensity = intensity;
        }
    }
}
