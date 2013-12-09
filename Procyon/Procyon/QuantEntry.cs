using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meta.Numerics.Statistics;
using Coon.Compass.Procyon.alglib;


namespace Coon.Compass.Procyon
{
    public class QuantEntry
    {
        public Dictionary<string, double> SampleValues { get; set; }
        public Dictionary<string, double> NormalizedSampleValues {get; set;}
        public Dictionary<string, double> Log2NormalizedSampleValues {get; set;}
        public Dictionary<string, double> MeanNormalizedSampleValues { get; set; }
        public Dictionary<string, string> HeadersToPrintDict { get; set; }
        public Dictionary<string, double> CombinedMeanNormalizedSampleValues { get; set; }
        public Dictionary<string, double> CombinedLog2NormalizedSampleValues { get; set; }
        public Dictionary<string, List<string>> CombinedHeadersToPrintSampleValues { get; set; }
        public Dictionary<string, string> UniqueHeaderToGroupString { get; set; }
        public Dictionary<string, List<double>> MeanNormValuesByGroup { get; set; }
        public Dictionary<string, List<double>> Log2IntValuesByGroup { get; set; }
        public Dictionary<AnnotationType, List<AnnotationEntry>> AnnotationEntries { get; set; }
        public Dictionary<string, bool> ComparisonToSignificanceDict { get; set; }
        public Dictionary<string, double> ComparisonToPValue { get; set; }
        public Dictionary<string, double> ComparisonToQValue { get; set; }
        public Dictionary<string, double> ComparisonToPrintAverage { get; set; }

        public List<string> UniprotIDList { get; set; }
        public List<string> UniprotStringList { get; set; }
        public List<string> GeneNameList { get; set; }

        public double SequenceCoverage { get; set; }

        public double Log2FoldChange { get; set; }

        public long UniqueIDHash { get; set; }

        public string FileLocation { get; set; }
        public string UniqueID { get; set; }

        public bool UsedForNormalization { get; set; }

        public QuantEntry(string fileLocation, string uniqueHeader, string uniqueHeader2, Dictionary<string, string> uniqueGroupNameToGroupString)
        {
            SampleValues = new Dictionary<string, double>();
            NormalizedSampleValues = new Dictionary<string, double>();
            MeanNormalizedSampleValues = new Dictionary<string, double>();
            Log2NormalizedSampleValues = new Dictionary<string, double>();
            HeadersToPrintDict = new Dictionary<string, string>();
            CombinedMeanNormalizedSampleValues = new Dictionary<string, double>();
            CombinedLog2NormalizedSampleValues = new Dictionary<string, double>();
            CombinedHeadersToPrintSampleValues = new Dictionary<string, List<string>>();
            AnnotationEntries = new Dictionary<AnnotationType, List<AnnotationEntry>>();
            UniprotIDList = new List<string>();
            UniprotStringList = new List<string>();
            GeneNameList = new List<string>();
            MeanNormValuesByGroup = new Dictionary<string, List<double>>();
            Log2IntValuesByGroup = new Dictionary<string, List<double>>();
            ComparisonToSignificanceDict = new Dictionary<string,bool>();
            ComparisonToPValue = new Dictionary<string,double>();
            ComparisonToQValue = new Dictionary<string,double>();
            ComparisonToPrintAverage = new Dictionary<string, double>();

            UniqueHeaderToGroupString = uniqueGroupNameToGroupString;
            UniqueID = uniqueHeader + uniqueHeader2;
            UniqueIDHash = UniqueID.GetHashCode();

            FileLocation = fileLocation;
        }

        public void GroupQuantData()
        {
            foreach (KeyValuePair<string, double> kvp in CombinedMeanNormalizedSampleValues)
            {
                string groupNumber = UniqueHeaderToGroupString[kvp.Key];
                List<double> outList = new List<double>();
                if (MeanNormValuesByGroup.TryGetValue(groupNumber, out outList))
                {
                    outList.Add(kvp.Value);
                }
                else
                {
                    List<double> addList = new List<double>();
                    List<double> addAverageList = new List<double>();
                    addList.Add(kvp.Value);
                    addAverageList.Add(kvp.Value);
                    MeanNormValuesByGroup.Add(groupNumber, addList);
                }
            }

            foreach (KeyValuePair<string, double> kvp in CombinedLog2NormalizedSampleValues)
            {
                string groupNumber = UniqueHeaderToGroupString[kvp.Key];
                List<double> outList = new List<double>();
                if (Log2IntValuesByGroup.TryGetValue(groupNumber, out outList))
                {
                    outList.Add(kvp.Value);
                }
                else
                {
                    List<double> addList = new List<double>();
                    addList.Add(kvp.Value);
                    Log2IntValuesByGroup.Add(groupNumber, addList);
                }
            }
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

        public void FillCombinedDictionary(Dictionary<string, string> fileNameAndHeadertoUniqueGroupName)
        {
            foreach (KeyValuePair<string, double> kvp in Log2NormalizedSampleValues)
            {
                string uniqueGroupNameKey = FileLocation + kvp.Key;
                CombinedLog2NormalizedSampleValues.Add(fileNameAndHeadertoUniqueGroupName[uniqueGroupNameKey], kvp.Value);
                CombinedMeanNormalizedSampleValues.Add(fileNameAndHeadertoUniqueGroupName[uniqueGroupNameKey], MeanNormalizedSampleValues[kvp.Key]);
            }

            foreach (KeyValuePair<string, string> kvp in HeadersToPrintDict)
            {
                List<string> addList = new List<string>();
                addList.Add(kvp.Value);
                CombinedHeadersToPrintSampleValues.Add(kvp.Key, addList);
            }
        }

        public void AddQuantEntry(QuantEntry addQuantEntry, Dictionary<string, string> fileNameAndHeadertoUniqueGroupName)
        {
            //Make a combined dictionary for Log2Normalized Values
            foreach (KeyValuePair<string, double> kvp in addQuantEntry.Log2NormalizedSampleValues)
            {
                string uniqueGroupNameKey = addQuantEntry.FileLocation + kvp.Key;
                CombinedLog2NormalizedSampleValues.Add(fileNameAndHeadertoUniqueGroupName[uniqueGroupNameKey], kvp.Value);
            }

            //Make a combined dictionary for Mean normalized Values
            foreach (KeyValuePair<string, double> kvp in addQuantEntry.MeanNormalizedSampleValues)
            {
                string uniqueGroupNameKey = addQuantEntry.FileLocation + kvp.Key;
                CombinedMeanNormalizedSampleValues.Add(fileNameAndHeadertoUniqueGroupName[uniqueGroupNameKey], kvp.Value);
            }

            //Add to the headers to print list - it will print out values from each file if they are not the same.
            foreach (KeyValuePair<string, string> kvp in addQuantEntry.HeadersToPrintDict)
            {
                List<string> outList = null;
                if (CombinedHeadersToPrintSampleValues.TryGetValue(kvp.Key, out outList))
                {
                    if(!outList.Contains(kvp.Value))
                    {
                        outList.Add(kvp.Value);
                    }
                }
                else
                {
                    List<string> addList = new List<string>();
                    addList.Add(kvp.Value);
                    CombinedHeadersToPrintSampleValues.Add(kvp.Key, addList);
                }
            }

            //Populate the Unique Header to Group Number Dictionary that will be used for Grouping the Quant Data
            foreach (KeyValuePair<string, string> uniqueGroupNameToNumber in addQuantEntry.UniqueHeaderToGroupString)
            {
                string outInt = null;
                if(!UniqueHeaderToGroupString.TryGetValue(uniqueGroupNameToNumber.Key, out outInt))
                {
                    UniqueHeaderToGroupString.Add(uniqueGroupNameToNumber.Key, uniqueGroupNameToNumber.Value);
                }
            }
        }

        public double MakeComparison(Comparison comparison, ValueType typeOfValue)
        {
            double returnlog2ratio = 0;
            List<double> outList = null;

            if (typeOfValue == ValueType.MeanNormalizedFoldChange || typeOfValue == ValueType.MeanNormalizedLog2Change)
            {
                if (MeanNormValuesByGroup.TryGetValue(comparison.NumeratorGroupString, out outList))
                {
                    double numeratorValue = MeanNormValuesByGroup[comparison.NumeratorGroupString].Average();
                    double denominatorValue = MeanNormValuesByGroup[comparison.DenominatorGroupString].Average();

                    double log2ratio = numeratorValue - denominatorValue;
                    returnlog2ratio = log2ratio;
                }
            }
            else
            {
                outList = null;
                if (Log2IntValuesByGroup.TryGetValue(comparison.NumeratorGroupString, out outList))
                {
                    double numeratorValue = Log2IntValuesByGroup[comparison.NumeratorGroupString].Average();
                    double denominatorValue = Log2IntValuesByGroup[comparison.DenominatorGroupString].Average();

                    double log2ratio = numeratorValue - denominatorValue;
                    returnlog2ratio = log2ratio;
                }
            }

            double realReturnvalue = 0;
            if (typeOfValue == ValueType.IntensityFoldChange || typeOfValue == ValueType.MeanNormalizedFoldChange)
            {
                if (returnlog2ratio > 0)
                {
                    realReturnvalue = Math.Pow(2, returnlog2ratio);
                }
                else
                {
                    realReturnvalue = -1 / Math.Pow(2, returnlog2ratio);
                }
            }
            else
            {
                realReturnvalue = returnlog2ratio;
            }

            ComparisonToPrintAverage.Add(comparison.ComparisonName, realReturnvalue);

            return realReturnvalue;
        }

        public void AddAnnotation(AnnotationType annotationType, AnnotationEntry annotationEntry)
        {
            List<AnnotationEntry> outEntries = null;
            if (AnnotationEntries.TryGetValue(annotationType, out outEntries))
            {
                if(!outEntries.Contains(annotationEntry))
                {
                    outEntries.Add(annotationEntry);
                }
            }
            else
            {
                List<AnnotationEntry> addList = new List<AnnotationEntry>();
                addList.Add(annotationEntry);
                AnnotationEntries.Add(annotationType, addList);
            }
        }

        public void PopulateUniprotList(string header)
        {
            List<string> uniprotIDCells = new List<string>();
            if (CombinedHeadersToPrintSampleValues.Count == 0)
            {
                if (header.Contains("Uniprot"))
                {
                    uniprotIDCells.Add(HeadersToPrintDict[header]);
                }
                else
                {
                    string parseit = HeadersToPrintDict[header];
                    string[] uniprotArray = parseit.Split('|');

                    if (uniprotArray.Count() > 0)
                    {
                        uniprotIDCells.Add(uniprotArray[1]);
                    }
                }
            }
            else
            {
                if (header.Contains("Uniprot"))
                {
                    uniprotIDCells = CombinedHeadersToPrintSampleValues[header];
                }
                else
                {
                    List<string> parseit = CombinedHeadersToPrintSampleValues[header];

                    foreach (string defline in parseit)
                    {
                        string[] uniprotArray = defline.Split('|');

                        if (uniprotArray.Count() > 1)
                        {
                            uniprotIDCells.Add(uniprotArray[1]);
                        }
                    }
                }
            }

            foreach (string uniprotIDCell in uniprotIDCells)
            {
                List<string> uniprotIDs = uniprotIDCell.Split('|').ToList();

                foreach (string uniprotID in uniprotIDs)
                {
                    if (!UniprotIDList.Contains(uniprotID))
                    {
                        UniprotIDList.Add(uniprotID);
                        UniprotStringList.Add(uniprotID);
                    }
                }
            }
        }

        public double PerformSignificanceTest(Comparison comparison, string sigString, double sigThreshold, ValueType typeOfValue)
        {
            //Add to the Comparison p value dictionary!!!!!!!!!
            double pvalue = 0;
            double newSigThershold = sigThreshold;
            string numeratorGroupNumber = comparison.NumeratorGroupString;
            string denominatorGroupNumber = comparison.DenominatorGroupString;

            Dictionary<string, List<double>> testDictionary = null;
            if (typeOfValue == ValueType.IntensityFoldChange || typeOfValue == ValueType.IntensityLog2Change)
            {
                testDictionary = Log2IntValuesByGroup;
            }
            else
            {
                testDictionary = MeanNormValuesByGroup;
            }

            List<double> outList = null;
            if (testDictionary.TryGetValue(comparison.NumeratorGroupString, out outList))
            {
                bool usepvalue = true;
                if (sigString.Equals("T-Test (Unequal Var)"))
                {
                    pvalue = TTestUneuqalVariance(comparison, testDictionary, typeOfValue);
                }
                else if (sigString.Equals("T-Test (Equal Var)"))
                {
                    pvalue = TTestEqualVariance();
                }
                else if (sigString.Equals("Fold Change"))
                {
                    usepvalue = false;
                    if (typeOfValue == ValueType.IntensityLog2Change || typeOfValue == ValueType.MeanNormalizedLog2Change)
                    {
                        newSigThershold = Math.Log(sigThreshold, 2);
                    }
                }

                ComparisonToPValue[comparison.ComparisonName] = pvalue;

                double testRatio = ComparisonToPrintAverage[comparison.ComparisonName];

                ComparisonToSignificanceDict[comparison.ComparisonName] = false;
                if (usepvalue && pvalue <= sigThreshold && pvalue != 0)
                {
                    ComparisonToSignificanceDict[comparison.ComparisonName] = true;
                }
                else if (!usepvalue && Math.Abs(testRatio) >= newSigThershold)
                {
                    ComparisonToSignificanceDict[comparison.ComparisonName] = true;
                }
            }

            return pvalue;
        }

        private double TTestUneuqalVariance(Comparison comparison, Dictionary<string, List<double>> testDict, ValueType typeOfValue)
        {
            double pvalue = 0;
            List<double[]> samples = new List<double[]>();

            string numeratorGroupString = comparison.NumeratorGroupString;
            string denominatorGroupString = comparison.DenominatorGroupString;

            samples.Add(testDict[numeratorGroupString].ToArray());
            samples.Add(testDict[denominatorGroupString].ToArray());

            double leftPvalue = 0;
            double rightPvalue = 0;
            double bothPvalue = 0;

            double[] set1 = samples[0];
            double[] set2 = samples[1];

            alglib.alglib.unequalvariancettest(set1, set1.Count(), set2, set2.Count(), out bothPvalue, out leftPvalue, out rightPvalue);

            pvalue = bothPvalue;

            double denominatorAverage = testDict[denominatorGroupString].Average();
            double numeratorAverage = testDict[numeratorGroupString].Average();

            double log2Ratio = numeratorAverage - denominatorAverage;
            double realReturnvalue = 0;
            if (typeOfValue == ValueType.IntensityFoldChange || typeOfValue == ValueType.MeanNormalizedFoldChange)
            {
                if (log2Ratio > 0)
                {
                    realReturnvalue = Math.Pow(2, log2Ratio);
                }
                else
                {
                    realReturnvalue = -1 / Math.Pow(2, log2Ratio);
                }
            }
            else
            {
                realReturnvalue = log2Ratio;
            }

            ComparisonToPrintAverage[comparison.ComparisonName] = realReturnvalue;

            Log2FoldChange = realReturnvalue;

            return pvalue;
        }

        private double TTestEqualVariance()
        {
            double pvalue = 0;

            

            return pvalue;
        }

    }
}
