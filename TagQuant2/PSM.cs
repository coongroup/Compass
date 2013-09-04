using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagQuant
{
    public class PSM
    {

        public string CsvFile { get; set; }
        public int SpectrumNumber { get; set; }
        public Dictionary<TagInformation, QuantPeak> QuantPeaks; 

        public PSM(string csvFile, int spectrumNumber, Dictionary<TagInformation, QuantPeak> quantPeaks)
        {
            CsvFile = csvFile;
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
