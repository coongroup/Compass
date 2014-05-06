using System.Collections.Generic;

namespace Coon.Compass.TagQuant
{
    public class PSM
    {
        public string FilenameID { get; set; }
        public int SpectrumNumber { get; set; }
        public QuantPeak[] QuantPeaks;
        public double Purity { get; set; }

        public PSM(string filenameID, int spectrumNumber, QuantPeak[] quantPeaks, double purity = 1)
        {
            FilenameID = filenameID;
            SpectrumNumber = spectrumNumber;
            QuantPeaks = quantPeaks;
            Purity = purity;
        }

        public QuantPeak this[TagInformation tag]
        {
            get
            {
                return QuantPeaks[tag.UniqueTagNumber];
            }
        }
    }
}
