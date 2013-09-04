using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;

namespace TagQuant
{
    public class QuantFile
    {
        public Dictionary<int, PSM> Psms; 

        public string FilePath { get; set; }

        public QuantFile(string filePath)
        {
            FilePath = filePath;
            Psms = new Dictionary<int, PSM>();
        }

        public void AddPSM(PSM psm)
        {
           Psms.Add(psm.SpectrumNumber, psm);
        }
        
        public PSM this[int scanNumber]
        {
            get
            {
                return Psms[scanNumber];
            }
        }
    }
}
