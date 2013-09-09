namespace Coon.Compass.Lotor
{
    public class LocalizedHit
    {

        public PSM PSM;
        public PeptideIsoform LocalizedIsoform;
        public double AScore;
        public int StartResidue;
                
        public bool isCTermModified
        {
            get
            {
                return LocalizedIsoform.CTerminusModification != null;
            }
        }

        public string[] omssapsm;

        public LocalizedHit(PSM psm, PeptideIsoform isoform, double ascore)
        {
            PSM = psm;
            LocalizedIsoform = isoform;
            AScore = ascore;            
        }

    }
}
