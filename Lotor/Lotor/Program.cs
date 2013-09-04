using System;                   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coon;
using Coon.Statistics;
using System.Diagnostics;
using Coon.Utilities;
using Coon.IO;
using System.IO;
using CoonThermo.IO;
using System.Windows.Forms;

namespace Lotor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainForm());
            return;

            Modification phosphorylation = new Modification("Phospho", new Mass(79.999329), ModificationSite.S | ModificationSite.T | ModificationSite.Y);

            string filename = @"C:\Users\Derek\Desktop\target_phospho.csv";
            string outname = @"C:\Users\Derek\Desktop\results.csv";
            string rawfile1 = @"C:\Users\Derek\Desktop\Phos1.raw";
            string rawfile2 = @"C:\Users\Derek\Desktop\Phos2.raw";
            Dictionary<string, ThermoRawFile> RawFiles = new Dictionary<string, ThermoRawFile>();
            RawFiles.Add("Phos1", new ThermoRawFile(rawfile1));
            RawFiles.Add("Phos2", new ThermoRawFile(rawfile2));

            Tolerance PROD_TOLERANCE = new Tolerance(0.01, ToleranceType.DA);
            
            Dictionary<Peptide, List<PSM>> Data = new Dictionary<Peptide, List<PSM>>();
            List<PSM> psms = null;
            using (OmssaReader reader = new OmssaReader(filename))
            {
                using (StreamWriter writer = new StreamWriter(outname))
                {
                    foreach (OmssaLine line in reader.ReadNextLine())
                    {
                        PSM psm = new PSM(5, null);
                        psm.BasePeptide = new Peptide(line.Sequence);
                        string rawName = line.FileName.Split('.')[0];
                        ThermoRawFile rawFile = RawFiles[rawName];
                        ThermoRawFileScan scan = rawFile[line.SpectrumNumber];
                        psm.Charge = line.Charge;
                        psm.ScanNumber = line.SpectrumNumber;                      
                        psm.Modifications = Modification.ParseModificationLine(line.Mods).ToList();
                        //psm.Spectrum = scan.Spectrum;
                        psm.GenerateIsoforms();

                        double pvalue = scan.Spectrum.Count * 2 * PROD_TOLERANCE.Value / scan.Spectrum.MzRange.Width; // TODO FIX

                        if (Data.TryGetValue(psm.BasePeptide, out psms))
                        {
                            psms.Add(psm);
                        }
                        else
                        {
                            psms = new List<PSM>();
                            psms.Add(psm);
                            Data.Add(psm.BasePeptide, psms);
                        }
                        psm.MatchIsofroms(FragmentType.b | FragmentType.y, PROD_TOLERANCE);
                        List<PeptideIsoform> isoforms = psm.PeptideIsoforms.ToList();
                        //isoforms.Sort();
                        //isoforms.Reverse();
                        //foreach (PeptideIsoform isoform in isoforms)
                        //{
                        //    writer.WriteLine(isoform + "," + isoform.SpectralMatch.Matches + "," + isoform.SpectralMatch.AverageError + "," + isoform.SpectralMatch.NTerminalMatches + "," + isoform.SpectralMatch.CTerminalMatches);
                          
                        //}
                        double[,] res = psm.Calc(FragmentType.b | FragmentType.y, pvalue);
                        for (int i = 0; i < res.GetLength(0); i++)
                        {
                            double minvalue = double.MaxValue;
                            for (int j = 0; j < res.GetLength(0); j++)
                            {
                                if (i == j) continue;
                                if (res[i, j] < 0)
                                {
                                    minvalue = 0;
                                    break;
                                }
                                if (res[i, j] <= minvalue)
                                {
                                    minvalue = res[i, j];
                                }
                            }
                            if (minvalue >= 13)
                            {
                                PeptideIsoform isoform = isoforms[i];
                                writer.WriteLine(psm.ScanNumber+","+isoform + "," + isoform.SpectralMatch.Matches + "," + isoform.SpectralMatch.AverageError + "," + isoform.SpectralMatch.NTerminalMatches + "," + isoform.SpectralMatch.CTerminalMatches + "," + minvalue);
                            }
                        }
                    }
                }
            }          
        }          


        // Read in all peptides
        // If peptides only have one isoform, put into a special list, already localized
        // Peptides that two isoforms are a special case as well, since we don't have to see which one is better scoring (just make sure we keep the AScore positive! since we won't know which one is the better isoform)
        // Peptides > two isoforms need to match the spectrum first to figure out which two isoforms are the best.



    }
}
