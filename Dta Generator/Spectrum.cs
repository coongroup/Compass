using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DtaGenerator
{
    public class Spectrum
    {

        public double RetentionTime { get; set; }   
        public int ScanNumber { get; set; }
        public int MSn { get; set; }
        public RawFile Parent { get; set; }
        public List<MSPeak> Peaks { get; set; }
        public double? PrecursorMZ = null;
        public int? Charge = null;
        public Spectrum ParentSpectrum { get; set; }
        public bool IsETD { get; set; }
        public ScanType ScanType { get; set; }
        public MZAnalyzerType Analyzer { get; set; }
        public float InjectionTime { get; set; }

        private string _title = null;
        public string Title
        {
            get
            {
                if (_title == null)
                {
                    _title = string.Format("{0}|{1}|{2}|{3:0.0000} min", Path.GetFileNameWithoutExtension(Parent.FilePath),Enum.GetName(typeof(MZAnalyzerType), Analyzer),Enum.GetName(typeof(ScanType), ScanType), RetentionTime);
                }
                return _title;
            }
        }

        public int Count { get { return Peaks.Count; } }

        public Spectrum(RawFile parent, int scanNum, int msn, double rt, List<MSPeak> peaks)
        {
            Parent = parent;
            ScanNumber = scanNum;
            MSn = msn;
            RetentionTime = rt;
            Peaks = peaks;            
        }
        
        public void CleanSpectrum(MzRange range) 
        {
            for (int index = 0; index < Peaks.Count; index++)
            {
                if (Peaks[index].MZ < range.Min) continue;
                if (Peaks[index].MZ > range.Max) break;
                Peaks.RemoveAt(index);
                index--;
            }
        }

        public void CleanSpectrum(List<MzRange> ranges)
        {
            List<MzRange> reducedRanges = ReduceRanges(ranges);
            int rangeIndex = 0;
            MzRange range = reducedRanges[rangeIndex];
            for (int index = 0; index < Peaks.Count; index++)
            {                
                if (Peaks[index].MZ < range.Min) continue;
                if (Peaks[index].MZ > range.Max)
                {
                    rangeIndex++;
                    if (rangeIndex < reducedRanges.Count)
                    {
                        range = reducedRanges[rangeIndex];
                        continue;
                    }
                    else
                    {
                        return; 
                    }
                }
                Peaks.RemoveAt(index);
                index--;                
            }
        }             

        public string ToDTA()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<dta id=\"{0}\" name=\"{1}\">", ScanNumber, Title).AppendLine();
            sb.AppendFormat("{0:0.00000} {1:N0}", PrecursorMZ.Value + 1.00727638, Charge.Value).AppendLine();
            foreach (MSPeak peak in Peaks)
            {
                sb.AppendFormat(" {0:0.00000} {1:0.000}", peak.MZ, peak.Intensity).AppendLine();
            }
            return sb.ToString();           
        }

        public string ToMGF()
        {
            StringBuilder sb = new StringBuilder("BEGIN IONS").AppendLine();
            sb.AppendFormat("Title={0}", Parent.FilePath).AppendLine();
            sb.AppendFormat("SCANS={0}", ScanNumber).AppendLine();
            sb.AppendFormat("RTINSECONDS={0}", RetentionTime * 60).AppendLine();
            sb.AppendFormat("PEPMASS={0:0.00000}", PrecursorMZ.Value).AppendLine();
            sb.AppendFormat("CHARGE={0:0+;0-}", Charge.Value).AppendLine();     
            foreach (MSPeak peak in Peaks)
            {
                sb.AppendFormat("{0:0.00000} {1:0.000}", peak.MZ, peak.Intensity).AppendLine();                                                       
            }
            sb.AppendLine("END IONS");
            return sb.ToString();
        }

        public static List<MzRange> ReduceRanges(List<MzRange> ranges)
        {
            // Method by CEV
            ///////////////////////////////////////////////////////////////////////
            // Sort ranges using insertion sort

            int k, j;
            MzRange temp;

            for (k = 1; k < ranges.Count; k++)
            {
                temp = ranges[k];
                j = k - 1;
                while (j >= 0 && ranges[j].Min > temp.Min)
                {
                    ranges[j + 1] = ranges[j];
                    j--;
                }
                ranges[j + 1] = temp;
            }

            ///////////////////////////////////////////////////////////////////////
            // Combine overlapping ranges
            bool newRange = true;
            List<MzRange> combinedList = new List<MzRange>();
            double min = 0; double max = 0; k = 0;

            while (k < ranges.Count)
            {
                if (newRange)
                {
                    min = ranges[k].Min;
                    max = ranges[k].Max;
                    k++;
                    newRange = false;
                }
                else
                {
                    if (max > ranges[k].Min)
                    {
                        if (max < ranges[k].Max)
                        {
                            max = ranges[k].Max;
                        }
                        k++;
                    }
                    else
                    {                       
                        MzRange mz_range; mz_range.Min = min; mz_range.Max = max;
                        combinedList.Add(mz_range);
                        newRange = true;
                    }
                }
                if (k == ranges.Count)
                {
                    MzRange mz_range; mz_range.Min = min; mz_range.Max = max;
                    combinedList.Add(mz_range);
                }
            }

            return combinedList;
        }
    }

}
