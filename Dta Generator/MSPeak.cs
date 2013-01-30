namespace DtaGenerator
{
    public struct MSPeak
    {
        public double MZ;
        public double Intensity;
       
        public MSPeak(double mz, double intensity)
        {
            MZ = mz;
            Intensity = intensity;
        }

        public override string ToString()
        {
            return string.Format("[{0:0.000}, {1:0.00}]", MZ, Intensity);
        }
    }
}
