namespace TagQuant
{
    public class TagInformation
    {
        public string ClassSampleName;
        public int ClassTag;
        public double TagMassETD;
        public double TagMassCAD;

        public TagInformation(int tag, string sample)
        {
            this.ClassSampleName = sample;
            this.ClassTag = tag;
            if (ClassTag == 113) { this.TagMassCAD = 113.1078; this.TagMassETD = 101.1073; }
            if (ClassTag == 114) { this.TagMassCAD = 114.1112; this.TagMassETD = 101.1073; }
            if (ClassTag == 115) { this.TagMassCAD = 115.1083; this.TagMassETD = 102.1044; }
            if (ClassTag == 116) { this.TagMassCAD = 116.1116; this.TagMassETD = 104.1111; }
            if (ClassTag == 117) { this.TagMassCAD = 117.1150; this.TagMassETD = 104.1111; }
            if (ClassTag == 118) { this.TagMassCAD = 118.1120; this.TagMassETD = 106.1115; }
            if (ClassTag == 119) { this.TagMassCAD = 119.1153; this.TagMassETD = 106.1115; }
            if (ClassTag == 121) { this.TagMassCAD = 121.1220; this.TagMassETD = 108.1182; }

            if (ClassTag == 126) { this.TagMassCAD = 126.1283; this.TagMassETD = 114.1279; }
            if (ClassTag == 127) { this.TagMassCAD = 127.1316; this.TagMassETD = 114.1279; }
            if (ClassTag == 128) { this.TagMassCAD = 128.1350; this.TagMassETD = 116.1347; }
            if (ClassTag == 129) { this.TagMassCAD = 129.1383; this.TagMassETD = 116.1347; }
            if (ClassTag == 130) { this.TagMassCAD = 130.1417; this.TagMassETD = 118.1415; }
            if (ClassTag == 131) { this.TagMassCAD = 131.1387; this.TagMassETD = 119.1384; }
        }
    }
}
