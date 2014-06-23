using System;
using System.Collections.Generic;

namespace Coon.Compass.TagQuant
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
            try
            {
                Psms.Add(psm.FilenameID, psm);
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("A PSM with the same id ({0}) has already been entered for this file ({1})", psm.FilenameID, FilePath), e);
            }
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
