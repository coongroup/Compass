using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MSFileReaderLib;

namespace DtaGenerator
{
    public class RawFile: IDisposable
    {
        public Spectrum this[int scanNum]
        {
            get
            {
                return GetSpectrum(scanNum);
            }
        }

        private IXRawfile4 baseRaw = null;
        public string FilePath { get; private set; }

        public bool IsOpen = false;
                
        private int _firstScanNumber = -1;
        public int FirstScanNumber
        {
            get
            {
                if (_firstScanNumber < 0)
                {
                    baseRaw.GetFirstSpectrumNumber(ref _firstScanNumber);
                }
                return _firstScanNumber;                    
            }
        }

        private int _lastScanNumber = -1;
        public int LastScanNumber
        {
            get
            {
                if (_lastScanNumber < 0)
                {
                    baseRaw.GetLastSpectrumNumber(ref _lastScanNumber);
                }
                return _lastScanNumber;                    
            }
        }

        private Dictionary<int, Spectrum> _scans_cache = null;
        private Dictionary<int, int> _msn_cache = null;

        public RawFile(string filepath)
        {
            FilePath = filepath;            
        }        

        public void Open()
        {
            baseRaw = (IXRawfile5)new MSFileReader_XRawfile();
            baseRaw.Open(this.FilePath);
            baseRaw.SetCurrentController(0, 1);
            _scans_cache = new Dictionary<int, Spectrum>();
            _msn_cache = new Dictionary<int, int>();
            IsOpen = true;           
        }
       
        public Spectrum GetSpectrum(int scanNumber)
        {
            Spectrum spectrum = null;
            if (!_scans_cache.TryGetValue(scanNumber, out spectrum))
            {               
                double rt = 0;
                int msn = GetMSNOrder(scanNumber);
                List<MSPeak> peaks = GetMassSpectrum(scanNumber);
                baseRaw.RTFromScanNum(scanNumber, ref rt);       
                spectrum = new Spectrum(this, scanNumber, msn, rt, peaks);
                _scans_cache.Add(scanNumber, spectrum);
            }
            return spectrum;   
        }
        
        public IEnumerable<Spectrum> GetNextMSMS()
        {           
            double rt = 0;          
            double precursor_mz = double.NaN;
            int last_surveyscan = FirstScanNumber;
            for (int scanNum = FirstScanNumber; scanNum < LastScanNumber; scanNum++)
            {
                int msn = GetMSNOrder(scanNum);
                if (msn > 1)
                {                    
                    List<MSPeak> peaks = GetMassSpectrum(scanNum);
                    baseRaw.RTFromScanNum(scanNum, ref rt);                
                    Spectrum spectrum = new Spectrum(this, scanNum, msn, rt, peaks);
                    //spectrum.ParentSpectrum = GetSpectrum(last_surveyscan); 
                    baseRaw.GetPrecursorMassForScanNum(scanNum, msn, ref precursor_mz);
                    spectrum.PrecursorMZ = precursor_mz;
                    spectrum.Charge = GetChargeState(scanNum);
                    spectrum.ScanType = GetScanType(scanNum);
                    spectrum.InjectionTime = GetInjectionTime(scanNum);
                    spectrum.Analyzer = GetAnalyzerType(scanNum);
                    yield return spectrum;
                }
                else
                {
                    // Assume a MS1 or survey scan
                    last_surveyscan = scanNum;
                }
            }
            yield break;
        }

        private List<MSPeak> GetMassSpectrum(int scanNum)
        {
            List<MSPeak> peaks = new List<MSPeak>();
            object peak_data = null;
            object flags = null;
            baseRaw.GetLabelData(ref peak_data, ref flags, ref scanNum);
            double[,] peaks_array = (double[,])peak_data;
            if (peaks_array.Length == 0)
            {
                // Handle low resolution data here
                double centroid_peak_width = -1.0;              
                int mass_list_array_size = -1;
                baseRaw.GetMassListFromScanNum(ref scanNum, null, 0, 0, 0, 1, ref centroid_peak_width, ref peak_data, ref flags, ref mass_list_array_size);
                peaks_array = (double[,])peak_data;
            }
            int max = peaks_array.GetLength(1);
            for (int i = 0; i < max; i++)
            {
                MSPeak peak; peak.MZ = peaks_array[0, i]; peak.Intensity = peaks_array[1, i];
                peaks.Add(peak);
            }
            return peaks;
        }

        private int GetMSNOrder(int scanNum)
        {
            int msn = 0;
            if (_msn_cache.TryGetValue(scanNum, out msn))
            {
                return msn;
            }
            else
            {
                baseRaw.GetMSOrderForScanNum(scanNum, ref msn);
                _msn_cache.Add(scanNum, msn);
            }
            return msn;
        }

        public double GetIsolationWidth(int scanNum)
        {
            object value = null;
            int msn = GetMSNOrder(scanNum);
            baseRaw.GetTrailerExtraValueForScanNum(scanNum, string.Format("MS{0} Isolation Width:", msn), ref value);
            if (value is double)
            {
                return (double)value;
            }
            return (float)value;
        }

        private int GetChargeState(int scanNum)
        {
            object value = null;           
            baseRaw.GetTrailerExtraValueForScanNum(scanNum, "Charge State:", ref value);
            return (int)((short)value);
        }

        private ScanType GetScanType(int scanNum)
        {
            int type = -1;
            int msn = GetMSNOrder(scanNum);
            baseRaw.GetActivationTypeForScanNum(scanNum, msn, ref type);
            return (ScanType)type;
        }

        private float GetInjectionTime(int scanNum)
        {
            object value = null;
            baseRaw.GetTrailerExtraValueForScanNum(scanNum, "Ion Injection Time (ms):", ref value);
            return (float)value;
        }

        private MZAnalyzerType GetAnalyzerType(int scanNum)
        {
            int analyzerType = -1;
            baseRaw.GetMassAnalyzerTypeForScanNum(scanNum, ref analyzerType);
            return (MZAnalyzerType)analyzerType;      
        }

        public void Close()
        {
            _msn_cache.Clear();
            _scans_cache.Clear();
            baseRaw.Close();
        }
    
        public void Dispose()
        {
            Close();
        }




    }
}
