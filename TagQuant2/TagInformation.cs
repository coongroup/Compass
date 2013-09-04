using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TagQuant
{
    public class TagInformation : INotifyPropertyChanged
    {
        private static int TagNumber = 0;

        enum TagSets {C, N};
        HashSet<double> N_TagMzs = new HashSet<double>() {127.1253, 128.1287, 129.1320, 130.1354};
        

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

        public TagInformation(int tag, string sample)
        {
            UniqueTagNumber = TagNumber++;
            TotalSignal = 0;
            NormalizedTotalSignal = 0;

            this.SampleName = sample;
            this.NominalMass = tag;
            if (NominalMass == 113) { this.MassCAD = 113.1078; this.MassEtd = 101.1073; }
            if (NominalMass == 114) { this.MassCAD = 114.1112; this.MassEtd = 101.1073; }
            if (NominalMass == 115) { this.MassCAD = 115.1083; this.MassEtd = 102.1044; }
            if (NominalMass == 116) { this.MassCAD = 116.1116; this.MassEtd = 104.1111; }
            if (NominalMass == 117) { this.MassCAD = 117.1150; this.MassEtd = 104.1111; }
            if (NominalMass == 118) { this.MassCAD = 118.1120; this.MassEtd = 106.1115; }
            if (NominalMass == 119) { this.MassCAD = 119.1153; this.MassEtd = 106.1115; }
            if (NominalMass == 121) { this.MassCAD = 121.1220; this.MassEtd = 108.1182; }

            if (NominalMass == 126) { this.MassCAD = 126.1283; this.MassEtd = 114.1279; }
            if (NominalMass == 127) { this.MassCAD = 127.1316; this.MassEtd = 114.1279; }
            if (NominalMass == 128) { this.MassCAD = 128.1350; this.MassEtd = 116.1347; }
            if (NominalMass == 129) { this.MassCAD = 129.1383; this.MassEtd = 116.1347; }
            if (NominalMass == 130) { this.MassCAD = 130.1417; this.MassEtd = 118.1415; }
            if (NominalMass == 131) { this.MassCAD = 131.1387; this.MassEtd = 119.1384; }

            if (NominalMass == 132) { this.MassCAD = 127.1253; this.MassEtd = 0; }
            if (NominalMass == 133) { this.MassCAD = 129.1320; this.MassEtd = 0; }
            if (NominalMass == 134) { this.MassCAD = 128.1287; this.MassEtd = 0; }
            if (NominalMass == 135) { this.MassCAD = 130.1354; this.MassEtd = 0; }
        }

        public TagInformation(int nominalMass, string tagName, string sampleSampleName, double cadMass, double etdMass, TagSetType tagSet)
        {
            NominalMass = nominalMass;
            TagName = tagName;
            SampleName = sampleSampleName;
            if (string.IsNullOrEmpty(SampleName))
                SampleName = tagName;
            MassCAD = cadMass;
            MassEtd = etdMass;
            TagSet = tagSet;
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
