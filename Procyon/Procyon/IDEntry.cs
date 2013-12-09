using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coon.Compass.Procyon
{
    public class IDEntry
    {
        public Dictionary<string, double> SampleValues { get; set; }
        public Dictionary<string, double> NormalizedSampleValues {get; set;}
        public Dictionary<string, double> Log2NormalizedSampleValues {get; set;}
        public Dictionary<string, double> MeanNormalizedSampleValues { get; set; }
        public Dictionary<string, string> HeadersToPrintDict { get; set; }
        public Dictionary<string, double> CombinedSampleValues { get; set; }

        public List<string> UniprotIDList { get; set; }
        public List<string> GeneNameList { get; set; }

        public double SequenceCoverage { get; set; }

        public long UniqueIDHash { get; set; }

        public string FileLocation { get; set; }
        public string UniqueID { get; set; }


        public IDEntry(string uniqueHeader, string uniqueHeader2)
        {
            SampleValues = new Dictionary<string, double>();
            NormalizedSampleValues = new Dictionary<string, double>();
            MeanNormalizedSampleValues = new Dictionary<string, double>();
            Log2NormalizedSampleValues = new Dictionary<string, double>();
            HeadersToPrintDict = new Dictionary<string, string>();
            CombinedSampleValues = new Dictionary<string, double>();
            UniprotIDList = new List<string>();
            GeneNameList = new List<string>();

            UniqueID = uniqueHeader + uniqueHeader2;
            UniqueIDHash = UniqueID.GetHashCode();
        }

        public void NormalizeSampleValues(Dictionary<string, double> normalizationDict)
        {
            foreach (KeyValuePair<string, double> kvp in normalizationDict)
            {
                double normalizedValue = kvp.Value * SampleValues[kvp.Key];
                NormalizedSampleValues.Add(kvp.Key, normalizedValue);
                Log2NormalizedSampleValues.Add(kvp.Key, Math.Log(normalizedValue, 2));
            }

            List<double> log2IntensityValues = new List<double>();
            foreach (KeyValuePair<string, double> kvp in Log2NormalizedSampleValues)
            {
                log2IntensityValues.Add(kvp.Value);
            }

            double mean = log2IntensityValues.Average();

            foreach (KeyValuePair<string, double> kvp in Log2NormalizedSampleValues)
            {
                MeanNormalizedSampleValues.Add(kvp.Key, (kvp.Value - mean));
            }
        }
    }
}
