using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL;
using CSMSL.IO;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace Coon.Compass.Lotor
{
    public class PSM
    {
        public int ScanNumber;
        public int Charge;
        public Peptide BasePeptide;
        public HashSet<PeptideIsoform> PeptideIsoforms;
        public double IsolationMZ { get; set; }
        public double[,] LocalizationResult;


        public MassSpectrum Spectrum
        {
            get { return DataScan.MassSpectrum; }
        }

        private MsnDataScan _dataScan = null;
        public MsnDataScan DataScan
        {
            get
            {
                if (_dataScan == null)
                {
                    _dataScan = DataFile[ScanNumber] as MsnDataScan;
                    if (_dataScan == null)
                    {
                        throw new ArgumentException("Not an MS/MS spectrum!");
                    }
                    IsolationMZ = _dataScan.IsolationRange.Mean;
                }
                return _dataScan;
            }
            set
            {
                _dataScan = value;
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

        public MSDataFile DataFile { get; set; }

        public PSM(int scannumber, MSDataFile dataFile) 
        {
            ScanNumber = scannumber;
            DataFile = dataFile;
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
                case 1: // one isoform, localized, no ascore calculated
                    return double.MaxValue;
                case 2: // two isoforms, skip finding best isoforms, make sure value is positive
                    return 0;
                default:
                    return _calcAscore();
            }
        }

        public double[,] Calc(FragmentTypes type, double pvalue) 
        {
            LocalizationResult = Calc(type, pvalue, this.PeptideIsoforms.ToArray());
            return LocalizationResult;
        }

        public void DeleteScan()
        {
            if (_dataScan != null)
            {
                _dataScan = null;
            }
        }

        private double[,] Calc(FragmentTypes type, double pvalue, params PeptideIsoform[] isoforms)
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

                    foreach (Fragment sdf in isoforms[i].GetSiteDeterminingFragments(isoforms[j], type))
                    {                        
                        if (isoforms[i].SpectralMatch.Contains(sdf)) i_count++; 
                        if (isoforms[j].SpectralMatch.Contains(sdf)) j_count++;                         
                        num_sdfs++;
                    }
                    
                    double ascore = CSMSL.Util.Combinatorics.AScore(i_count, j_count, pvalue, num_sdfs);
                    
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

        public void MatchIsofroms(FragmentTypes type, MassTolerance tolerance)
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
