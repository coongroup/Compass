using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Spectral;

namespace TagQuant
{
    public class QuantPeak
    {
        public MsnDataScan DataScan { get; set; }

        public double InjectionTime { get; set; }

        public double RawIntensity { get; set; }

        public TagInformation Tag { get; set; }

        public double DeNormalizedIntensity
        {
            get
            {
                if (IsNoisedCapped)
                    return Noise*InjectionTime;
                return RawIntensity*InjectionTime;
            }
        }

        public double MZ { get; set; }

        public bool IsNoisedCapped { get; set; }

        public double Noise { get; set; }

        public QuantPeak(TagInformation tag, IPeak peak, double injectionTime, MsnDataScan scan, double noise = 0, bool isNoiseCapped = false)
        {
            Tag = tag;
            if (peak == null)
            {
                RawIntensity = 0;
                MZ = 0;
            }
            else
            {
                RawIntensity = peak.Y;
                MZ = peak.X;
            }
            DataScan = scan;
            InjectionTime = injectionTime;
            Noise = noise;
            IsNoisedCapped = isNoiseCapped;
        }

        public double PurityCorrectedIntensity { get; set; }

        public double PurityCorrectedNormalizedIntensity { get; set; }
    }
}
