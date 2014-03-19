using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.IO.OMSSA;
using CSMSL.IO.PepXML;
using CSMSL.Proteomics;

namespace OmssaConvertor
{
    public class Convertor
    {

        public Protease Protease { get; set; }
        public int MissedClevages { get; set; }
        public string UserModFile { get; set; }
        public string FastaDatabaseFile { get; set; }
        public List<int> FixedMods { get; set; }
        public List<int> VariableMods { get; set; }


        public Convertor()
        {
            FixedMods = new List<int>();
            VariableMods = new List<int>();
        }

        public void Convert(string omssaCSV, MSDataFile dataFile)
        {
            string filePath = Path.ChangeExtension(omssaCSV, ".pepxml");
            using (PepXmlWriter writer = new PepXmlWriter(filePath))
            {

                writer.WriteSampleProtease(Protease);

                writer.StartSearchSummary("OMSSA", true, true);

                writer.WriteProteinDatabase(FastaDatabaseFile);

                writer.WriteSearchProtease(Protease, MissedClevages);

                foreach (int modNumber in FixedMods)
                {
                    OmssaModification mod;
                    if (OmssaModification.TryGetModification(modNumber, out mod))
                    {
                        writer.WriteModification(mod, mod.Sites, true);
                    }
                }

                foreach (int modNumber in VariableMods)
                {
                    OmssaModification mod;
                    if (OmssaModification.TryGetModification(modNumber, out mod))
                    {
                        writer.WriteModification(mod, mod.Sites, false);
                    }
                }
            

                writer.SetCurrentStage(PepXmlWriter.Stage.Spectra, true);
                
                using (OmssaCsvPsmReader reader = new OmssaCsvPsmReader(omssaCSV, UserModFile))
                {
                    reader.AddMSDataFile(dataFile);
                    reader.LoadProteins(FastaDatabaseFile);
                    foreach (PeptideSpectralMatch psm in reader.ReadNextPsm())
                    {
                        writer.StartSpectrum(psm.SpectrumNumber, psm.Spectrum.RetentionTime, psm.Spectrum.PrecursorMz, psm.Spectrum.PrecursorCharge);
                        writer.WritePSM(psm);
                        writer.EndSpectrum();
                    }

                }

            }

        }


    }
}
