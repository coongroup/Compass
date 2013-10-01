using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL;
using CSMSL.Chemistry;
using CSMSL.IO;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Util;

namespace Coon.Compass.Lotor
{
    public class PSM
    {
        public int ScanNumber;
        public int Charge;
        public Peptide BasePeptide;
        public HashSet<PeptideIsoform> PeptideIsoforms;
        public double IsolationMZ {
            get { return DataScan.IsolationRange.Mean; }
        }
        public double[,] LocalizationResult;
        public int[,] NumSiteDeterminingFragments;
        public int[,] BestSiteDeterminingFragments;
        public int[,] SecondBestSiteDeterminingFragments;

        public MassSpectrum Spectrum
        {
            get { return DataScan.MassSpectrum; }
        }
        
        public MsnDataScan DataScan
        {
            get; private set;
        }

        public int StartResidue = 1;

        public int NumberOfSharingProteinGroups { get; set; }

        public string Filename = "";

        public string ProteinGroup { get; set; }

        public string Defline { get; set; }

        public bool IsProteinNTerm
        {
            get
            {
                return StartResidue == 1;
            }
        }

        public PSM(int scannumber, MSDataFile dataFile) 
        {
            ScanNumber = scannumber;
            DataScan = dataFile[scannumber] as MsnDataScan;
        }

        /// <summary>
        /// The number of isoforms this PSM has
        /// </summary>
        public int Isoforms
        {
            get
            {
                return PeptideIsoforms != null ? PeptideIsoforms.Count : 0;
            }
        }

        public List<Modification> VariabledModifications;
     
        public double[,] Calc(double pvalue)
        {
            return LocalizationResult = Calc(pvalue, PeptideIsoforms.ToArray());
        }

        private double[,] Calc(double pvalue, params PeptideIsoform[] isoforms)
        {
            int count = isoforms.Count();
            double[,] res = new double[count, count];
            NumSiteDeterminingFragments = new int[count, count];
            BestSiteDeterminingFragments = new int[count,count];
            for (int i = 0; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }                        
              
                    int i_count = 0;
                    int j_count = 0;
                    int num_sdfs = 0;

                    foreach (Fragment sdf in GetSiteDeterminingFragments(isoforms[i], isoforms[j]))
                    {    
                        if (sdf.Parent == isoforms[i] && isoforms[i].SpectralMatch.Contains(sdf)) i_count++; 
                        if (sdf.Parent == isoforms[j] && isoforms[j].SpectralMatch.Contains(sdf)) j_count++;                         
                        num_sdfs++;
                    }
                    num_sdfs /= 2;
                    BestSiteDeterminingFragments[i, j] = i_count;
                    BestSiteDeterminingFragments[j, i] = j_count;
                  
                    NumSiteDeterminingFragments[i, j] = num_sdfs;
                    NumSiteDeterminingFragments[j, i] = num_sdfs;

                    double ascore = Combinatorics.AScore(i_count, j_count, pvalue, num_sdfs);
                    
                    // Symmetry Rules
                    res[i, j] = ascore;
                    res[j, i] = -ascore;
                }
            }
            return res;
        }

        private IEnumerable<Fragment> GetSiteDeterminingFragments(PeptideIsoform pep1, PeptideIsoform pep2)
        {
            HashSet<Fragment> aFrags = new HashSet<Fragment>(pep1.Fragments);
            aFrags.SymmetricExceptWith(pep2.Fragments);
            return aFrags;
        }

        public void MatchIsofroms(FragmentTypes type, MassTolerance tolerance, double cutoffThreshold, params int[] chargeStates)
        {
            foreach (PeptideIsoform isoform in PeptideIsoforms)
            {
                isoform.MatchSpectrum(type, tolerance, cutoffThreshold, chargeStates);
            }
        }

        public int GenerateIsoforms(bool ignoreCTerminalMods = false)
        {
            PeptideIsoforms = new HashSet<PeptideIsoform>();
            foreach (PeptideIsoform isoform in BasePeptide.GenerateIsoforms(VariabledModifications.ToArray()).Select(pep => new PeptideIsoform(pep, Spectrum, Charge)))
            {
                if (ignoreCTerminalMods)
                {
                    IMass[] mods = isoform.GetModifications();
                    var cTerminalMod = mods[mods.Length - 2];
                    if (cTerminalMod is ModificationCollection)
                    {
                        var modCollection = cTerminalMod as ModificationCollection;
                        if (modCollection.Any(mod => Lotor.QuantifiedModifications.Contains(mod)))
                            continue;
                    }
                    else
                    {
                        if (Lotor.QuantifiedModifications.Contains(cTerminalMod))
                            continue;
                    }
                }
                PeptideIsoforms.Add(isoform);
            }
            return Isoforms;
        }

    }

}
