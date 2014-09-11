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

        public double ScanWidth { get; private set; }
        public double IsolationMZ { get; private set; }      
        
        public double[,] LocalizationResult;
        public int[,] NumSiteDeterminingFragments;
        public int[,] BestSiteDeterminingFragments;
        public int[,] SecondBestSiteDeterminingFragments;

        public ISpectrum Spectrum { get; private set; }
         
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

        public string RawFileName { get; private set; }

        public PSM(int scannumber, string rawfileName) 
        {
            ScanNumber = scannumber;
            RawFileName = rawfileName;
        }

        public void SetRawFile(IMSDataFile dataFile)
        {
            IMsnDataScan scan = dataFile[ScanNumber] as IMsnDataScan;
            IsolationMZ = scan.GetIsolationRange().Mean;
            Spectrum = dataFile.GetSpectrum(ScanNumber);
            ScanWidth = scan.MzRange.Width;
        }

        /// <summary>
        /// The number of isoforms this PSM has
        /// </summary>
        public int Isoforms { get; private set; }

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
            BestSiteDeterminingFragments = new int[count, count];
            for (int i = 0; i < count; i++)
            {
                for (int j = i+1; j < count; j++)
                {
                    int i_count = 0;
                    int j_count = 0;
                    int num_sdfs = 0;

                    var isoI = isoforms[i];
                    var isoJ = isoforms[j];

                    foreach (var sdf in GetSiteDeterminingFragments(isoI, isoJ))
                    {
                        if (ReferenceEquals(sdf.Parent, isoI) && isoI.MatchedFragments.Contains(sdf)) i_count++;
                        if (ReferenceEquals(sdf.Parent, isoJ) && isoJ.MatchedFragments.Contains(sdf)) j_count++;                         
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
            List<Fragment> frags1 = pep1.Fragments;
            List<Fragment> frags2 = pep2.Fragments;
           
            int count = frags1.Count;
            for (int i = 0; i < count; i++)
            {
                var frag1 = frags1[i];
                var frag2 = frags2[i];
                double difference = Math.Abs(frag1.MonoisotopicMass - frag2.MonoisotopicMass);
                if (difference > 0.001)
                {
                    yield return frag1;
                    yield return frag2;
                }
            }
        }

        public void MatchIsofroms(FragmentTypes type, Tolerance tolerance, double cutoffThreshold, bool phopshoNeutralLoss, params int[] chargeStates)
        {
            foreach (PeptideIsoform isoform in PeptideIsoforms)
            {
                isoform.MatchSpectrum(type, tolerance, cutoffThreshold,phopshoNeutralLoss, chargeStates);
            }
        }

        public int GenerateIsoforms(bool ignoreCTerminalMods = false)
        {
            PeptideIsoforms = new HashSet<PeptideIsoform>();
            foreach (PeptideIsoform isoform in BasePeptide.GenerateIsoforms(true,VariabledModifications.ToArray()).Select(pep => new PeptideIsoform(pep, Spectrum, Charge)))
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

            Isoforms = PeptideIsoforms.Count;
            return Isoforms;
        }

    }

}
