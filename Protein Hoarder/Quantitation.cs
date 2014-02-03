using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Coon.Compass.ProteinHoarder
{
    public class Quantitation
    {
        public int Plex { get; set; }

        public int PSMs { get; private set; }

        public HashSet<Peptide> UniquePeptides { get; set; }

        public List<Peptide> Peptides { get; set; }

        public List<Tuple<Peptide, double[], bool>> QuantData { get; set; }

        public static bool UseMedian;

        public Quantitation(int plex, Peptide peptide, double[] quant, bool ismissingChannel = false)
        {
            if (UseMedian && plex != 2)
            {
                throw new ArgumentException("You cannot use median normalized values on data that is not duplex");
            }

            Plex = plex;
            UniquePeptides = new HashSet<Peptide>();
            Peptides = new List<Peptide>();
            QuantData = new List<Tuple<Peptide, double[], bool>>();
            PSMs = 0;

            AddData(peptide, quant, ismissingChannel);
        }

        public void AddData(Peptide peptide, double[] quant, bool ismissingChannel = false)
        {
            PSMs++;
            UniquePeptides.Add(peptide);
            Peptides.Add(peptide);
            QuantData.Add(new Tuple<Peptide, double[], bool>(peptide, quant, ismissingChannel));
        }

        public double GetLog2Ratio(bool useOnlyCompleteSets, out double[] data)
        {
            int length = Plex*4;
            data = new double[length];
            
            List<double[]> quantToUse = new List<double[]>();

            if (useOnlyCompleteSets)
            {
                bool oneQuantHasAllData = false;
                foreach (Tuple<Peptide, double[], bool> datatuple in QuantData)
                {
                    if (!datatuple.Item3)
                    {
                        oneQuantHasAllData = true;
                        break;
                    }
                }

                if (oneQuantHasAllData)
                {
                    foreach (Tuple<Peptide, double[], bool> datatuple in QuantData)
                    {
                        if (!datatuple.Item3)
                            quantToUse.Add(datatuple.Item2);
                    }
                   
                }
                else
                {
                    foreach (Tuple<Peptide, double[], bool> datatuple in QuantData)
                    {
                        quantToUse.Add(datatuple.Item2);
                    }
                }
            }
            else
            {
                foreach (Tuple<Peptide, double[], bool> datatuple in QuantData)
                {
                    quantToUse.Add(datatuple.Item2);
                }
            }

            if (UseMedian)
            {
                List<double[]> allData = new List<double[]>();
                foreach (double[] quantData in quantToUse)
                {
                    allData.Add(quantData);
                }
                allData.Sort(new MeidanSorter());

                int size = allData.Count;
                int mid = size / 2;

                if (size % 2 == 0)
                {
                    double[] data1 = allData[mid];
                    double[] data2 = allData[mid - 1];

                    for (int i = 0; i < length; i++)
                    {
                        data[i] = (data1[i] + data2[i]) / 2.0;
                    }
                }
                else
                {
                    data = allData[mid];
                }
            }
            else
            {
                foreach (double[] quantData in quantToUse)
                {
                    for (int i = 0; i < length; i++)
                    {
                        data[i] += quantData[i];
                    }
                }
            }

            // Always getting the log2 ratio of the last two. for now
            double val1 = data[length - 1];
            double val2 = data[length - 2];
            double ratio = (val2 == 0) ? 0 : val1 / val2;
            double logRatio = Math.Log(ratio, 2);
            return logRatio;
        }

        public string ToOutput(bool duplexQuant = false, double medianLog2 = 0, bool useOnlyCompleteSets = false)
        {
            StringBuilder sb = new StringBuilder();

            double[] data;
            double log2Ratio = GetLog2Ratio(useOnlyCompleteSets, out data);

            foreach (double datum in data)
            {
                sb.Append(datum);
                sb.Append(',');
            }

            if (duplexQuant)
            {
                if (double.IsInfinity(log2Ratio))
                {
                    sb.Append("N/A,N/A");
                }
                else
                {

                    sb.Append(log2Ratio);
                    sb.Append(',');
                    sb.Append(log2Ratio - medianLog2); // normalized
                }
            }
            else
            {
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            return sb.ToString();
        }
    }

    public class MeidanSorter : IComparer<double[]>
    {
        public int Compare(double[] x, double[] y)
        {
            int xlength = x.Length;
            int ylength = y.Length;

            double x1 = x[xlength - 2];
            double x2 = x[xlength - 1];

            double xratio = (x2 == 0) ? 0 : x1/x2;

            double y1 = y[ylength - 2];
            double y2 = y[ylength - 1];
            double yratio = (y2 == 0) ? 0 : y1/y2;

            return xratio.CompareTo(yratio);
        }
    }
}