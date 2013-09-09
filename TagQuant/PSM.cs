using System.Collections.Generic;

namespace TagQuant
{
    public class PSM
    {
        public string FilenameID { get; set; }
        public int SpectrumNumber { get; set; }
        public Dictionary<TagInformation, QuantPeak> QuantPeaks; 

        public PSM(string filenameID, int spectrumNumber, Dictionary<TagInformation, QuantPeak> quantPeaks)
        {
            FilenameID = filenameID;
            SpectrumNumber = spectrumNumber;
            QuantPeaks = new Dictionary<TagInformation, QuantPeak>(quantPeaks);
        }

        public QuantPeak this[TagInformation tag]
        {
            get
            {
                return QuantPeaks[tag];
            }
        }
    }
}
