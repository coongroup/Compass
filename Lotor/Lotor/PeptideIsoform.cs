using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coon;
using Coon.Spectra;

namespace Lotor
{
    public class PeptideIsoform : Peptide, IComparable<PeptideIsoform>
    {
        public Spectrum Spectrum;
        public int Charge;
        public SpectralMatch SpectralMatch;
        
        public PeptideIsoform(Peptide peptide, Spectrum spectrum, int charge)
            : base(peptide) 
        {           
            Spectrum = spectrum;
            Charge = charge;
        }

        private FragmentDictionary fragments;

        public void MatchSpectrum(FragmentType fragmentType, Tolerance tolerance)
        {            
            fragments = this.CalculateFragments(fragmentType);
            SpectralMatch = Spectrum.MatchFragments(fragments.Values, tolerance, 1);        
        }

        public int CompareTo(PeptideIsoform other)
        {
            if (this.SpectralMatch == null)
            {
                if (other.SpectralMatch == null) return 0;
                return -1;
            }
            else
            {
                if (other.SpectralMatch == null) return 1;
                return this.SpectralMatch.Matches.CompareTo(other.SpectralMatch.Matches);
            }
        }
    }
}
