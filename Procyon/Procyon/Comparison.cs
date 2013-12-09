using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coon.Compass.Procyon
{
    public class Comparison
    {
        public Dictionary<string, double> CombinedPvalueDict { get; set; }
        public Dictionary<string, double> CombinedQvalueDict { get; set; }
        public Dictionary<string, List<QuantEntry>> SignificantQuantEntryDict { get; set; }
        public Dictionary<string, List<QuantEntry>> AllQuantEntryDict { get; set; }
        public Dictionary<string, Dictionary<AnnotationType, Dictionary<string, double>>> AnnotationDictToPrint { get; set; }

        public string NumeratorGroupString { get; set; }
        public string DenominatorGroupString { get; set; }

        public double Log2Ratio { get; set; }

        public bool TestSignificance { get; set; }

        public string ComparisonName { get; set; }
        public long ComparisonHash { get; set; }

        public Comparison(string numeratorGroupString, string denominatorGroupString, bool testSignificance)
        {
            CombinedPvalueDict = new Dictionary<string, double>();
            CombinedQvalueDict = new Dictionary<string, double>();
            SignificantQuantEntryDict = new Dictionary<string, List<QuantEntry>>();
            AllQuantEntryDict = new Dictionary<string, List<QuantEntry>>();
            AnnotationDictToPrint = new Dictionary<string, Dictionary<AnnotationType, Dictionary<string, double>>>();

            NumeratorGroupString = numeratorGroupString;
            DenominatorGroupString = denominatorGroupString;
            TestSignificance = testSignificance;

            ComparisonName = numeratorGroupString.ToString() + "/" + denominatorGroupString.ToString();
            ComparisonHash = ComparisonName.GetHashCode();
        }
    }
}
