using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Coon.Compass.TagQuant
{
    public class TagInformation : INotifyPropertyChanged
    {
        private static int TagNumber = 0;
        public int UniqueTagNumber;
        public string TagName { get; private set; }
        public string SampleName { get; set; }
        public int NominalMass { get; set; }
        public double MassEtd { get; set; }
        public double MassCAD { get; set; }
        public double M2 { get; set; }
        public double M1 { get; set; }
        public double P1 { get; set; }
        public double P2 { get; set; }
        public TagSetType TagSet { get; set; }

        public double TotalSignal { get; set; }
        public double NormalizedTotalSignal { get; set; }

        private bool _isUsed;
        public bool IsUsed
        {
            get
            {
                return _isUsed;
            }
            set
            {
                SetProperty(ref _isUsed, value);
            }
        }
        
        public TagInformation(int nominalMass, string tagName, string sampleSampleName, double cadMass, double etdMass, TagSetType tagSet, double m2 = 0, double m1 = 0, double p1 = 0, double p2 = 0, bool use = false)
        {
            UniqueTagNumber = TagNumber++;
            TotalSignal = 0;
            NormalizedTotalSignal = 0;

            NominalMass = nominalMass;
            TagName = tagName;
            SampleName = sampleSampleName;
            //if (string.IsNullOrEmpty(SampleName))
            //    SampleName = tagName;
            MassCAD = cadMass;
            MassEtd = etdMass;
            M2 = m2;
            M1 = m1;
            P1 = p1;
            P2 = p2;
            TagSet = tagSet;
            IsUsed = use;
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
