using System;
using System.Collections.Generic;
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

        public PeptideIsoform(Peptide peptide, MassSpectrum spectrum, int charge)
            : base(peptide) 
        {           
            Spectrum = spectrum;
            Charge = charge;
        }

        private IEnumerable<Fragment> fragments;

        public void MatchSpectrum(FragmentTypes fragmentTypes, MassTolerance tolerance)
        {
            fragments = Fragment(fragmentTypes);
            SpectralMatch = new SpectrumFragmentsMatch(Spectrum);
            SpectralMatch.MatchFragments(fragments, tolerance, 1);
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
