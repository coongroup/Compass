using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using CSMSL.Chemistry;

namespace Coon.Compass.Lotor
{
    public class PeptideIsoform : Peptide, IComparable<PeptideIsoform>
    {
        private ChemicalFormula H3PO4 = new ChemicalFormula("H3PO4");

        public Spectrum Spectrum;
        public int Charge;
        public SpectrumFragmentsMatch SpectralMatch;
        public int SiteDeterminingFragments;

        public PeptideIsoform(Peptide peptide, Spectrum spectrum, int charge)
            : base(peptide) 
        {           
            Spectrum = spectrum;
            Charge = charge;
        }

        public List<Fragment> Fragments;

        public HashSet<Fragment> MatchedFragments; 

        public void MatchSpectrum(FragmentTypes fragmentTypes, MassTolerance tolerance, double cutoffThreshold, bool phosphoNeutralLoss, params int[] chargeStates)
        {
            Fragments = Fragment(fragmentTypes).OrderBy(f => f.ToString()).ToList();
                      
            if (phosphoNeutralLoss)
            {
                List<Fragment> toAdd = new List<Fragment>();
                foreach (Fragment frag in Fragments)
                {
                    if (frag.Modifications.Contains(lotorForm.Phosphorylation))
                    {
                        var newFrag = new Fragment(frag.Type, frag.Number, frag.MonoisotopicMass - H3PO4.MonoisotopicMass, frag.Parent, frag.Modifications, "Phopsho Neutral loss -H3PO4");
                        newFrag.Modifications.Remove(lotorForm.Phosphorylation);
                        toAdd.Add(newFrag);
                    }
                }
                Fragments.AddRange(toAdd);
            }

            SpectralMatch = new SpectrumFragmentsMatch(Spectrum);
            var matches = SpectralMatch.MatchFragments(Fragments, tolerance, cutoffThreshold, chargeStates);
            MatchedFragments = new HashSet<Fragment>(matches);
        }

        public int CompareTo(PeptideIsoform other)
        {
            if (SpectralMatch == null)
            {
                if (other.SpectralMatch == null) return 0;
                return -1;
            }
            else
            {
                if (other.SpectralMatch == null) return 1;
                return SpectralMatch.Matches.CompareTo(other.SpectralMatch.Matches);
            }
        }
    }
}
