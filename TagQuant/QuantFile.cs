using System.Collections.Generic;

namespace TagQuant
{
    public class QuantFile
    {
        public Dictionary<string, PSM> Psms; 

        public string FilePath { get; set; }

        public QuantFile(string filePath)
        {
            FilePath = filePath;
            Psms = new Dictionary<string, PSM>();
        }

        public void AddPSM(PSM psm)
        {
           Psms.Add(psm.FilenameID, psm);
        }

        public PSM this[string filenameID]
        {
            get
            {
                return Psms[filenameID];
            }
        }
    }
}
