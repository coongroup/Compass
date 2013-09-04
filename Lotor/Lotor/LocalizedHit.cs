using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotor
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
                if (LocalizedIsoform[LocalizedIsoform.Length - 1].FixedModification != null)
                {
                    return LocalizedIsoform[LocalizedIsoform.Length - 1].FixedModification.Name.Contains("AcetylK");
                }
                else
                {
                    return false;
                }
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
