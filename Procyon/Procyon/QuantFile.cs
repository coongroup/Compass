using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coon.Compass.Procyon.alglib;
using Meta.Numerics;
using Meta.Numerics.Statistics;


namespace Coon.Compass.Procyon
{
    public class QuantFile
    {
        public string FileLoaction { get; set; }
        public Dictionary<string, List<double>> HeadertoValueDict { get; set; }
        public Dictionary<string, List<double>> HeadertoSubsetValueDict { get; set; }
        public Dictionary<string, double> HeadertoNormValueDict { get; set; }
        public Dictionary<string, string> HeadersToPrint { get; set; }
        public Dictionary<string, string> UniqueGroupNamesToPrint { get; set; }
        public Dictionary<string, QuantEntry> CombinedQuantEntryDict { get; set; }
        public Dictionary<string, string> CombinedHeadersToPrint { get; set; }
        public Dictionary<string, string> CombinedGroupNamesToPrint { get; set; }
        public Dictionary<string, List<QuantEntry>> SignificantQuantEntryDict { get; set; }
        public Dictionary<string, List<QuantEntry>> AllQuantEntryDict { get; set; }
        public Dictionary<string, Comparison> Comparisons { get; set; }
        public Dictionary<string, QuantEntry> QuantEntriesForNorm { get; set; }

        public List<string> FileLocations { get; set; }
        public List<QuantEntry> QuantEntries { get; set; }
        public string UniprotHeader { get; set; }

        public QuantFile(string fileLocation, List<string> quantHeaders)
        {
            HeadertoValueDict = new Dictionary<string, List<double>>();
            HeadertoNormValueDict = new Dictionary<string, double>();
            HeadersToPrint = new Dictionary<string, string>();
            UniqueGroupNamesToPrint = new Dictionary<string, string>();
            SignificantQuantEntryDict = new Dictionary<string, List<QuantEntry>>();
            AllQuantEntryDict = new Dictionary<string, List<QuantEntry>>();
            Comparisons = new Dictionary<string, Comparison>();
            QuantEntriesForNorm = new Dictionary<string, QuantEntry>();

            foreach (string header in quantHeaders)
            {
                List<double> addList = new List<double>();
                HeadertoValueDict.Add(header, addList);
            }

            QuantEntries = new List<QuantEntry>();
            FileLoaction = fileLocation;
        }

        public QuantFile(List<QuantFile> quantFiles, Dictionary<string, string> fileNameAndHeadertoUniqueGroupName)
        {
            CombinedQuantEntryDict = new Dictionary<string, QuantEntry>();
            CombinedHeadersToPrint = new Dictionary<string, string>();
            CombinedGroupNamesToPrint = new Dictionary<string, string>();
            FileLocations = new List<string>();
            Comparisons = new Dictionary<string, Comparison>();

            foreach(QuantFile quantFile in quantFiles)
            {
                //Add the file locations for all files
                FileLocations.Add(quantFile.FileLoaction);
                UniprotHeader = quantFile.UniprotHeader;

                //Add all of the headers to print to one dictionary - should all be the same headers
                foreach (KeyValuePair<string, string> kvp in quantFile.HeadersToPrint)
                {
                    string outString = null;
                    if (!CombinedHeadersToPrint.TryGetValue(kvp.Key, out outString))
                    {
                        CombinedHeadersToPrint.Add(kvp.Key, kvp.Value);
                    }
                }

                //Combine all of the unique group names to print out 
                foreach (KeyValuePair<string, string> kvp in quantFile.UniqueGroupNamesToPrint)
                {
                    string outString = null;
                    if (!CombinedGroupNamesToPrint.TryGetValue(kvp.Key, out outString))
                    {
                        CombinedGroupNamesToPrint.Add(kvp.Key, kvp.Value);
                    }
                }

                //Combine the quantitation entries from each file
                foreach (QuantEntry quantEntry in quantFile.QuantEntries)
                {
                    QuantEntry outQuantEntry = null;
                    if (CombinedQuantEntryDict.TryGetValue(quantEntry.UniqueID, out outQuantEntry))
                    {
                        //If this entry already exsits simply add to the quant entry that exists
                        outQuantEntry.AddQuantEntry(quantEntry, fileNameAndHeadertoUniqueGroupName);
                    }
                    else
                    {
                        //Simply add the quant entry of it is the first time it is added
                        quantEntry.FillCombinedDictionary(fileNameAndHeadertoUniqueGroupName);
                        CombinedQuantEntryDict.Add(quantEntry.UniqueID, quantEntry);
                    }
                }
            }
        }

        public void NormalizeDataSet(bool noNormalize, bool sumNormalize, bool medianNormalize, string uniprotHeader, string normalizationString, List<string> uniprotListForNormalization)
        {
            if (uniprotListForNormalization.Count == 0)
            {
                if (noNormalize)
                {
                    DoNotNormalizeDataSet(HeadertoValueDict);
                }
                else if (medianNormalize)
                {
                    MedianNormalizeDataSet(HeadertoValueDict);
                }
                else if (sumNormalize)
                {
                    SumNormalizeDataSet(HeadertoValueDict);
                }
            }
            else
            {
                //this will be a normalization based on a subset of proteins
                Dictionary<string, List<double>> subsetDictionary = new Dictionary<string, List<double>>();
                foreach (QuantEntry entry in QuantEntries)
                {
                    entry.PopulateUniprotList(uniprotHeader);

                    foreach (string uniprotID in entry.UniprotIDList)
                    {
                        if (uniprotListForNormalization.Contains(uniprotID))
                        {
                            QuantEntry outEntry = null;
                            if(!QuantEntriesForNorm.TryGetValue(entry.UniqueID, out outEntry))
                            {
                                QuantEntriesForNorm.Add(entry.UniqueID, entry);
                            }

                            foreach (KeyValuePair<string, double> kvp in entry.SampleValues)
                            {
                                List<double> outDouble = null;
                                if (subsetDictionary.TryGetValue(kvp.Key, out outDouble))
                                {
                                    outDouble.Add(kvp.Value);
                                }
                                else
                                {
                                    List<double> addList = new List<double>();
                                    addList.Add(kvp.Value);
                                    subsetDictionary.Add(kvp.Key, addList);
                                }
                            }
                        }
                    }
                }

                if (noNormalize)
                {
                    DoNotNormalizeDataSet(subsetDictionary);
                }
                else if (medianNormalize)
                {
                    MedianNormalizeDataSet(subsetDictionary);
                }
                else if (sumNormalize)
                {
                    SumNormalizeDataSet(subsetDictionary);
                }
            }
        }

        public void GroupQuantData()
        {
            foreach (QuantEntry quantEntry in CombinedQuantEntryDict.Values)
            {
                quantEntry.GroupQuantData();
            }
        }

        public void AddComparison(Comparison comparison, ValueType typeOfValue)
        {
            Comparison comparisonOut = null;
            if (!Comparisons.TryGetValue(comparison.ComparisonName, out comparisonOut))
            {
                Comparisons.Add(comparison.ComparisonName, comparison);
            }

            foreach (QuantEntry quantEntry in CombinedQuantEntryDict.Values)
            {
                quantEntry.MakeComparison(comparison, typeOfValue);
            }
        }

        public void PerformSignificanceTest(string sigTestingString, double sigThreshold, ValueType typeOfValue)
        {
            foreach (Comparison comparison in Comparisons.Values)
            {
                foreach (KeyValuePair<string, QuantEntry> kvp in CombinedQuantEntryDict)
                {
                    double pvalue = kvp.Value.PerformSignificanceTest(comparison, sigTestingString, sigThreshold, typeOfValue);

                    if (pvalue != 0)
                    {
                        comparison.CombinedPvalueDict.Add(kvp.Key, pvalue);
                    }
                }
            }
        }

        public void MulitpleComparisonCorrection(string multCompString, string thresholdTypeString, double sigThreshold)
        {
            if (multCompString.Equals("None"))
            {
                foreach (Comparison comparison in Comparisons.Values)
                {
                    comparison.CombinedQvalueDict = comparison.CombinedPvalueDict;
                }
            }
            else if (multCompString.Equals("Benjamini-Hochberg"))
            {
                foreach (Comparison comparison in Comparisons.Values)
                {
                    comparison.CombinedQvalueDict = BenjaminiHochberg(comparison.CombinedPvalueDict);
                }
            }

            bool significantOnQValue = false;
            if (thresholdTypeString.Equals("q-Value"))
            {
                significantOnQValue = true;
            }

            foreach (Comparison comparison in Comparisons.Values)
            {
                foreach (QuantEntry quantEntry in CombinedQuantEntryDict.Values)
                {
                    double outDouble = 0;
                    if (comparison.CombinedQvalueDict.TryGetValue(quantEntry.UniqueID, out outDouble))
                    {
                        quantEntry.ComparisonToQValue[comparison.ComparisonName] = outDouble;
                    }
                    else
                    {
                        quantEntry.ComparisonToQValue[comparison.ComparisonName] = 0;
                    }

                    if (significantOnQValue)
                    {
                        quantEntry.ComparisonToSignificanceDict[comparison.ComparisonName] = false;
                        if (quantEntry.ComparisonToQValue[comparison.ComparisonName] < sigThreshold && quantEntry.ComparisonToQValue[comparison.ComparisonName] != 0)
                        {
                            quantEntry.ComparisonToSignificanceDict[comparison.ComparisonName] = true;
                        }
                    }
                }
            }
        }

        public void TestAnnotationEnrichment(string annotationString, List<AnnotationType> annotationsToAdd, Dictionary<string, AnnotationEntry> annotationEntryDict, Dictionary<string, Dictionary<AnnotationType, List<string>>> uniprotToAnnotationList, double annotationSigThreshold)
        {
            AnnotateQuantEntries(annotationString, annotationsToAdd, annotationEntryDict, uniprotToAnnotationList);

            if (!annotationString.Equals("Annotate Only"))
            {
                //Determine what type of changes to test for significance
                List<string> sigTypeToTest = new List<string>();
                if (annotationString.Equals("Annotate,Test Enrichment (Significant)")) { sigTypeToTest.Add("Sig"); }
                else if (annotationString.Equals("Annotate,Test Enrichment (Sig. Up)")) { sigTypeToTest.Add("SigUp"); }
                else if (annotationString.Equals("Annotate,Test Enrichment (Sig. Down)")) { sigTypeToTest.Add("SigDown"); }
                else if (annotationString.Equals("Annotate,Test Enrichment (All Three)"))
                {
                    sigTypeToTest.Add("Sig");
                    sigTypeToTest.Add("SigUp");
                    sigTypeToTest.Add("SigDown");
                }

                Dictionary<AnnotationType, List<QuantEntry>> backgroundQuantEntries = new Dictionary<AnnotationType, List<QuantEntry>>();
                List<QuantEntry> allQuantEntries = CombinedQuantEntryDict.Values.ToList();
                foreach (AnnotationType annotationType in annotationsToAdd)
                {
                    backgroundQuantEntries.Add(annotationType, allQuantEntries);
                }

                //This will serve as the background for all of the types of annotations
                Dictionary<AnnotationType, Dictionary<string, int>> backgroundAnnotationDict = CreateAnnotationCountDict(backgroundQuantEntries);

                
                //Now for each significance type create a matching list for the fisher exact test
                foreach (Comparison comparison in Comparisons.Values)
                {
                    foreach (string sigType in sigTypeToTest)
                    {
                        Dictionary<AnnotationType, List<QuantEntry>> significantChangingQuantEntries = ListSignificantChangingQuantEntries(annotationsToAdd, comparison, sigType, allQuantEntries);
                        Dictionary<AnnotationType, Dictionary<string, int>> significantAnnotationDict = CreateAnnotationCountDict(significantChangingQuantEntries);

                        Dictionary<AnnotationType, Dictionary<string, double>> pvalueAnnoationDict = TestForEnrichment(backgroundAnnotationDict, significantAnnotationDict, annotationSigThreshold);

                        comparison.AnnotationDictToPrint.Add(sigType, pvalueAnnoationDict);
                    }
                }
            }
        }

        private void AnnotateQuantEntries(string annotationString, List<AnnotationType> annotationsToAdd, Dictionary<string, AnnotationEntry> annotationEntryDict, Dictionary<string, Dictionary<AnnotationType, List<string>>> uniprotToAnnotationList)
        {
            foreach (QuantEntry quantEntry in CombinedQuantEntryDict.Values)
            {
                quantEntry.PopulateUniprotList(UniprotHeader);

                foreach(string uniprotID in quantEntry.UniprotIDList)
                {
                    Dictionary<AnnotationType, List<string>> annotationTypeToAnnotationEntry = null;

                    if (uniprotToAnnotationList.TryGetValue(uniprotID, out annotationTypeToAnnotationEntry))
                    {
                        foreach (AnnotationType annotationType in annotationsToAdd)
                        {
                            List<string> goTermsToadd = null;
                            if (annotationTypeToAnnotationEntry.TryGetValue(annotationType, out goTermsToadd))
                            {
                                foreach (string goID in goTermsToadd)
                                {
                                    quantEntry.AddAnnotation(annotationType, annotationEntryDict[goID]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<AnnotationType, List<QuantEntry>> ListSignificantChangingQuantEntries(List<AnnotationType> annotationsToAdd, Comparison comparison, string sigType, List<QuantEntry> quantEntries)
        {
            Dictionary<AnnotationType, List<QuantEntry>> retDict = new Dictionary<AnnotationType, List<QuantEntry>>();

            foreach (AnnotationType annotationType in annotationsToAdd)
            {
                List<QuantEntry> addList = new List<QuantEntry>();
                retDict.Add(annotationType, addList);

                if (sigType.Equals("Sig"))
                {
                    foreach (QuantEntry quantEntry in quantEntries)
                    {
                        bool outBool = false;
                        if (quantEntry.ComparisonToSignificanceDict.TryGetValue(comparison.ComparisonName, out outBool))
                        {
                            if (outBool)
                            {
                                retDict[annotationType].Add(quantEntry);
                            }
                        }
                    }
                }
                else if (sigType.Equals("SigUp"))
                {
                    foreach (QuantEntry quantEntry in quantEntries)
                    {
                        bool outBool = false;
                        if (quantEntry.ComparisonToSignificanceDict.TryGetValue(comparison.ComparisonName, out outBool))
                        {
                            if (outBool && quantEntry.Log2FoldChange > 0)
                            {
                                retDict[annotationType].Add(quantEntry);
                            }
                        }
                    }
                }
                else if (sigType.Equals("SigDown"))
                {
                    foreach (QuantEntry quantEntry in quantEntries)
                    {
                        bool outBool = false;
                        if (quantEntry.ComparisonToSignificanceDict.TryGetValue(comparison.ComparisonName, out outBool))
                        {
                            if (outBool && quantEntry.Log2FoldChange < 0)
                            {
                                retDict[annotationType].Add(quantEntry);
                            }
                        }
                    }
                }
            }

            return retDict;
        }

        private Dictionary<AnnotationType, Dictionary<string, int>> CreateAnnotationCountDict(Dictionary<AnnotationType, List<QuantEntry>> annotationQuantListDict)
        {
            Dictionary<AnnotationType, Dictionary<string, int>> retDict = new Dictionary<AnnotationType, Dictionary<string, int>>();

            foreach (KeyValuePair<AnnotationType, List<QuantEntry>> kvp in annotationQuantListDict)
            {
                Dictionary<string, int> addDict = new Dictionary<string,int>();
                retDict.Add(kvp.Key, addDict);

                foreach (QuantEntry quantEntry in kvp.Value)
                {
                    List<AnnotationEntry> annotationEntries = null;
                    if (quantEntry.AnnotationEntries.TryGetValue(kvp.Key, out annotationEntries))
                    {
                        foreach (AnnotationEntry annotationEntry in annotationEntries)
                        {
                            int outCount = 0;
                            string id = annotationEntry.UniqueID;
                            if (retDict[kvp.Key].TryGetValue(id, out outCount))
                            {
                                retDict[kvp.Key][id]++;
                            }
                            else
                            {
                                retDict[kvp.Key].Add(id, 1);
                            }
                        }
                    }
                }
            }

            return retDict;
        }

        private Dictionary<AnnotationType, Dictionary<string, double>> TestForEnrichment(Dictionary<AnnotationType, Dictionary<string, int>> backgroundDict, Dictionary<AnnotationType, Dictionary<string, int>> testDict, double annotationSigThreshold)
        {
            Dictionary<AnnotationType, Dictionary<string, double>> tempDict = new Dictionary<AnnotationType, Dictionary<string, double>>();

            foreach (KeyValuePair<AnnotationType, Dictionary<string, int>> kvp in testDict)
            {
                tempDict.Add(kvp.Key, FisherExactTest(backgroundDict[kvp.Key], kvp.Value));
            }

            Dictionary<AnnotationType, Dictionary<string, double>> retDict = new Dictionary<AnnotationType, Dictionary<string, double>>();

            foreach (KeyValuePair<AnnotationType, Dictionary<string, double>> kvp in tempDict)
            {
                Dictionary<string, double> addDict = new Dictionary<string,double>();
                retDict.Add(kvp.Key, addDict);

                foreach (KeyValuePair<string, double> kvp2 in kvp.Value)
                {
                    if (kvp2.Value <= annotationSigThreshold)
                    {
                        retDict[kvp.Key].Add(kvp2.Key, kvp2.Value);
                    }
                }
            }

            return retDict;
        }

        private void MedianNormalizeDataSet(Dictionary<string, List<double>> normalizationSubset)
        {
            Dictionary<string, double> tempDict = new Dictionary<string, double>();
            List<double> listForAverage = new List<double>();

            //Go through Each Column in the Quant File, Calculate Median and Save to Dictionary
            //Also save the median values to a list that will be used for Normalization
            foreach (KeyValuePair<string, List<double>> kvp in normalizationSubset)
            {
                kvp.Value.Sort();
                double middleIndexDouble = kvp.Value.Count / 2;
                int middleIndex = (int)Math.Round(middleIndexDouble);
                double median = kvp.Value.ElementAt(middleIndex);

                tempDict.Add(kvp.Key, median);
                listForAverage.Add(median);
            }

            double average = listForAverage.Average();
            foreach (KeyValuePair<string, double> kvp3 in tempDict)
            {
                double replaceValue = average / kvp3.Value;
                HeadertoNormValueDict.Add(kvp3.Key, replaceValue);
            }

            ApplyNormalizationFactors();
        }

        private void SumNormalizeDataSet(Dictionary<string, List<double>> normalizationSubset)
        {
            Dictionary<string, double> tempDict = new Dictionary<string, double>();
            List<double> listForAverage = new List<double>();
            foreach (KeyValuePair<string, List<double>> kvp in normalizationSubset)
            {
                tempDict.Add(kvp.Key, kvp.Value.Sum());
                listForAverage.Add(kvp.Value.Sum());
            }

            double average = listForAverage.Average();
            foreach (KeyValuePair<string, double> kvp3 in tempDict)
            {
                double replaceValue = average / kvp3.Value;
                HeadertoNormValueDict.Add(kvp3.Key, replaceValue);
            }

            ApplyNormalizationFactors();
        }

        private void DoNotNormalizeDataSet(Dictionary<string, List<double>> normalizationSubset)
        {
            foreach (KeyValuePair<string, List<double>> kvp in normalizationSubset)
            {
                HeadertoNormValueDict.Add(kvp.Key, 1);
            }

            ApplyNormalizationFactors();
        }

        private void ApplyNormalizationFactors()
        {
            if (QuantEntriesForNorm.Count > 0)
            {
                QuantEntries.Clear();
                foreach (QuantEntry entry in QuantEntriesForNorm.Values)
                {
                    QuantEntries.Add(entry);
                }
            }

            foreach (QuantEntry quantEntry in QuantEntries)
            {
                quantEntry.NormalizeSampleValues(HeadertoNormValueDict);
            }
        }

        private Dictionary<string, double> BenjaminiHochberg(Dictionary<string, double> pvalueDict)
        {
            Dictionary<string, double> tempretDict = new Dictionary<string, double>();
            Dictionary<string, double> retDict = new Dictionary<string, double>();
            Dictionary<string, double> sortedDict = pvalueDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            int count = 1;
            foreach (KeyValuePair<string, double> kvp in sortedDict)
            {
                double qValue = kvp.Value * sortedDict.Values.Count / (count);
                tempretDict.Add(kvp.Key, qValue);

                count++;
            }

            Dictionary<string, double> reverseSortedDict = tempretDict.Reverse().ToDictionary(x => x.Key, x => x.Value);

            int i = 0;
            foreach (KeyValuePair<string, double> kvp in reverseSortedDict)
            {
                if (i != 0)
                {
                    if (retDict.ElementAt(i - 1).Value < kvp.Value)
                    {
                        retDict.Add(kvp.Key, retDict.ElementAt(i - 1).Value);
                    }
                    else
                    {
                        retDict.Add(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    retDict.Add(kvp.Key, kvp.Value);
                }

                i++;
            }

            return retDict;
        }

        private Dictionary<string, double> FisherExactTest(Dictionary<string, int> background, Dictionary<string, int> test)
        {
            Dictionary<string, double> pvalueDict = new Dictionary<string, double>();

            int totalBackground = background.Values.Sum();
            int totalTest = test.Values.Sum();

            foreach (KeyValuePair<string, int> kvp in test)
            {
                int backgroundCount = background[kvp.Key];
                int testCount = kvp.Value;

                BinaryContingencyTable table = new BinaryContingencyTable();

                //I got this from Doug's code. I hope it is right. 
                int upperleft = testCount;
                int upperright = backgroundCount - upperleft;
                int lowerleft = totalTest - upperleft;
                int lowerright = totalBackground - totalTest - upperright;

                table[0, 0] = upperleft;
                table[0, 1] = upperright;
                table[1, 0] = lowerleft;
                table[1, 1] = lowerright;

                TestResult fisherExactResult = table.FisherExactTest();

                double pvalueLeft = fisherExactResult.LeftProbability;
                double pvalueRight = fisherExactResult.LeftProbability;

                pvalueDict.Add(kvp.Key, pvalueRight);

            }

            Dictionary<string, double> qvalueDict = BenjaminiHochberg(pvalueDict);

            return qvalueDict;
        }


    }
}
