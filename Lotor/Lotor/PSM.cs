using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coon;
using Coon.Spectra;
using CoonThermo.IO;

namespace Lotor
{
    public class PSM
    {
        public int ScanNumber;
        public int Charge;
        public Peptide BasePeptide;
        public HashSet<PeptideIsoform> PeptideIsoforms;
        public double IsolationMZ { get; set; }
        public double[,] LocalizationResult;


        public Spectrum Spectrum
        {
            get { return RawFileScan.Spectrum; }
        }

        private ThermoRawFileScan rawFileScan = null;
        public ThermoRawFileScan RawFileScan
        {
            get
            {
                if (rawFileScan == null)
                {
                    rawFileScan = RawFile[ScanNumber];
                    IsolationMZ = rawFileScan.IsolationRange.MeanValue;
                }
                return rawFileScan;
            }
            set
            {
                rawFileScan = value;
            }
        }
             

        public int StartResidue = 1;

        public string Filename = "";

        public string ProteinGroup = "";

        public bool IsProteinNTerm
        {
            get
            {
                return StartResidue == 1;
            }
        }
           
        public ThermoRawFile RawFile;

        public PSM(int scannumber, ThermoRawFile rawFile) 
        {
            ScanNumber = scannumber;
            RawFile = rawFile;
        }

        public int Isoforms
        {
            get
            {
                if (PeptideIsoforms != null)
                {
                    return PeptideIsoforms.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public List<Modification> Modifications;
     
        public double CalculateAScore()
        {           
            switch (this.Isoforms)
            {
                case 0: // no isoforms, no ascore
                    return 0;
                case 1: // one isoform, localized, no ascore calcualte
                    return double.MaxValue;
                case 2: // two isoforms, skip finding best isoforms, make sure value is positive
                    return 0;
                default:
                    return _calcAscore();
            }
        }

        public double[,] Calc(FragmentType type, double pvalue) 
        {
            LocalizationResult = Calc(type, pvalue, this.PeptideIsoforms.ToArray());
            return LocalizationResult;
        }

        public void DeleteScan()
        {
            if (rawFileScan != null)
            {
                rawFileScan = null;
            }
        }

        private double[,] Calc(FragmentType type, double pvalue, params PeptideIsoform[] isoforms)
        {
            int count = isoforms.Count();
            double[,] res = new double[count, count];  
            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    if (i == j)
                    {
                        res[i, j] = double.PositiveInfinity;
                        continue;
                    }                        
              
                    int i_count = 0;
                    int j_count = 0;
                    int num_sdfs = 0;

                    foreach (string sdf in isoforms[i].CalculateSDFs(isoforms[j], type))
                    {                        
                        if (isoforms[i].SpectralMatch.Contains(sdf)) i_count++; 
                        if (isoforms[j].SpectralMatch.Contains(sdf)) j_count++;                         
                        num_sdfs++;
                    }
                    
                    double ascore = Coon.Statistics.Statistics.AScore(i_count, j_count, pvalue, num_sdfs);
                    
                    // Symmerty Rules
                    res[i, j] = ascore;
                    res[j, i] = -ascore;
                }
            }
            return res;
        }

        private double _calcAscore()
        {
            return 0;
        }

        public void MatchIsofroms(FragmentType type, Tolerance tolerance)
        {
            foreach (PeptideIsoform isoform in PeptideIsoforms)
            {
                isoform.MatchSpectrum(type, tolerance);
            }
        }

        public int GenerateIsoforms()
        {
            PeptideIsoforms = new HashSet<PeptideIsoform>();
            foreach (Peptide pep in BasePeptide.GenerateIsoforms(Modifications.ToArray()))
            {
                PeptideIsoform isoform = new PeptideIsoform(pep, Spectrum, Charge);
                PeptideIsoforms.Add(isoform);
            }
            return Isoforms;
        }

    }

}
