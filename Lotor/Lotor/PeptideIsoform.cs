using System;
using System.Collections.Generic;
using System.Linq;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Proteomics;
using CSMSL.Spectral;

namespace Coon.Compass.Lotor
{
    public class PeptideIsoform : Peptide, IComparable<PeptideIsoform>
    {
        public MassSpectrum Spectrum;
        public int Charge;
        public SpectrumFragmentsMatch SpectralMatch;
        public int SiteDeterminingFragments;

        public PeptideIsoform(Peptide peptide, MassSpectrum spectrum, int charge)
            : base(peptide) 
        {           
            Spectrum = spectrum;
            Charge = charge;
        }

        public List<Fragment> Fragments;

        public HashSet<Fragment> MatchedFragments; 

        public void MatchSpectrum(FragmentTypes fragmentTypes, MassTolerance tolerance, double cutoffThreshold, params int[] chargeStates)
        {
            Fragments = Fragment(fragmentTypes).OrderBy(f => f.ToString()).ToList();
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
