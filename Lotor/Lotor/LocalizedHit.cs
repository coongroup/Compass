namespace Coon.Compass.Lotor
{
    public class LocalizedHit
    {
        public static double AScoreThreshold { get; set; }

        public PSM PSM;
        public PeptideIsoform LocalizedIsoform;
        public PeptideIsoform SecondBestPeptideIsoform;
        public double AScore;
        public double PValue;

        public int MatchDifference { get; private set; }

        public bool IsLocalized
        {
            get
            {
                if (ReferenceEquals(LocalizedIsoform, SecondBestPeptideIsoform))
                    return true;
                return MatchDifference > 0;
                //return AScore >= AScoreThreshold;
            }
        }

        public int BestPeptideSDFCount { get; set; }
        public int SecondBestPeptideSDFCount { get; set; }

        public int StartResidue;
        public int NumberOfSiteDeterminingFragments { get; set; }
    
        public string[] omssapsm;

        public LocalizedHit(PSM psm, PeptideIsoform isoform, PeptideIsoform secondIsoform, int numSDFs, int sdfs, int sdfs2, double pvalue, double ascore)
        {
            PSM = psm;
            LocalizedIsoform = isoform;
            SecondBestPeptideIsoform = secondIsoform;
            NumberOfSiteDeterminingFragments = numSDFs;
            BestPeptideSDFCount = sdfs;
            SecondBestPeptideSDFCount = sdfs2;
            PValue = pvalue;
            AScore = ascore;
            MatchDifference = isoform.SpectralMatch.Matches - secondIsoform.SpectralMatch.Matches;
        }

    }
}
