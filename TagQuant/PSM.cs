using System.Collections.Generic;

namespace Coon.Compass.TagQuant
{
    public class PSM
    {
        public string FilenameID { get; set; }
        public int SpectrumNumber { get; set; }
        public QuantPeak[] QuantPeaks;

        public PSM(string filenameID, int spectrumNumber, QuantPeak[] quantPeaks)
        {
            FilenameID = filenameID;
            SpectrumNumber = spectrumNumber;
            QuantPeaks = quantPeaks;
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
