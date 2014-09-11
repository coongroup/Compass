using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSMSL.Chemistry;
using CSMSL.Proteomics;
using CSMSL.Spectral;
using IMP.PhosphoRS;

namespace Coon.Compass.PhosphoRS
{
    public class ExtendedPeptideSpectrumMatch : PeptideSpectrumMatch
    {
        private static int _currentPsmID = 1;
        private static int _currentSequenceId = 1;
        private static char _currentModId = 'a';
        private static int _isoformId = 1;
        private static readonly Dictionary<IMass, AminoAcidModification> _modificationMappingDictionary;
        private static readonly Dictionary<int, int> _psms; 

        static ExtendedPeptideSpectrumMatch()
        {
            _modificationMappingDictionary = new Dictionary<IMass, AminoAcidModification>();
            _psms = new Dictionary<int, int>();
        }

        public Peptide Peptide { get; set; }
        //public ISpectrum Spectrum { get; set; }
        public Dictionary<int, int[]> Isoforms { get; private set; }
        public string Line { get; set; }
        public int StartResidue { get; set; }
        public string ProteinGroup { get; set; }
        public string Defline { get; set; }
        public double[] QuantData { get; set; }

        public ExtendedPeptideSpectrumMatch(Peptide peptide, ISpectrum spectrum, int charge, DissociationType type, int lineNumber)
            : base(_currentPsmID++, ConvertSpectrumType(type), charge, peptide.ToMz(charge), ConvertSpectrum(spectrum), ConvertPeptide(peptide)) 
        {
            Peptide = peptide;
            //Spectrum = spectrum;
            Isoforms = new Dictionary<int, int[]>();
        }

        public ExtendedPeptideSpectrumMatch(int id, Peptide peptide, ISpectrum spectrum, int charge, SpectrumType type)
            : base(id, type, charge, peptide.ToMz(charge), ConvertSpectrum(spectrum), ConvertPeptide(peptide))
        {
            Peptide = peptide;
            //Spectrum = spectrum;
            Isoforms = new Dictionary<int, int[]>();
        }

        public Tuple<int, List<int>> AddIsoform(int[] combo)
        {
            int id = _isoformId++;
            Isoforms.Add(id, combo);
            _psms.Add(id, ID);
            return new Tuple<int, List<int>>(id, new List<int>(combo));
        }

        public string GetIsoformString(int id)
        {
            int[] combo = Isoforms[id];
            return ConvertString(sequenceString, combo);
        }

        #region Statics

        public static string ConvertString(string baseString, IEnumerable<int> positions)
        {
            char[] seq = baseString.ToCharArray();
            foreach (int pos in positions)
            {
                seq[pos] = char.ToLower(seq[pos]);
            }
            return new string(seq);
        }

        public static Peak[] ConvertSpectrum(ISpectrum spectrum)
        {
            if (spectrum == null)
                return new Peak[0];
            Peak[] peaks = new Peak[spectrum.Count];
            for (int i = 0; i < spectrum.Count; i++)
            {
                peaks[i] = new Peak(spectrum.GetMass(i), spectrum.GetIntensity(i));
            }
            return peaks;
        }

        public static SpectrumType ConvertSpectrumType(DissociationType type)
        {
            SpectrumType spectrumType = SpectrumType.CID_CAD;
            switch (type)
            {
                case DissociationType.CID:
                case DissociationType.PQD:
                    spectrumType = SpectrumType.CID_CAD;
                    break;
                case DissociationType.ETD:
                case DissociationType.ECD:
                    spectrumType = SpectrumType.ECD_ETD;
                    break;
                case DissociationType.HCD:
                    spectrumType = SpectrumType.HCD;
                    break;
            }
            return spectrumType;
        }

        public static AminoAcidSequence ConvertPeptide(Peptide peptide)
        {
            foreach (Modification mod in peptide.GetUniqueModifications<Modification>())
            {
                if (_modificationMappingDictionary.ContainsKey(mod)) 
                    continue;
                AminoAcidModification newMod = new AminoAcidModification(_currentModId++, mod.Name, mod.Name, mod.Name, mod.MonoisotopicMass, mod.MonoisotopicMass, AminoAcidSequence.ParseAASequence(String.Join(String.Empty, mod.Sites.GetActiveSites().Select(site => site.ToString()).ToArray())));
                _modificationMappingDictionary.Add(mod, newMod);
            }
            return AminoAcidSequence.Create(_currentSequenceId++, peptide.Sequence, GetModifications(peptide), ModificationToString(peptide));
        }

        public static void AddModification(IMass mod, AminoAcidModification phosphoRsModification)
        {
            if (!_modificationMappingDictionary.ContainsKey(mod))
            {
                _modificationMappingDictionary.Add(mod, phosphoRsModification);
            }
        }

        private static List<AminoAcidModification> GetModifications(Peptide peptide)
        {
            List<AminoAcidModification> mods = new List<AminoAcidModification>();
            foreach (IMass mod in peptide.GetUniqueModifications<IMass>())
            {
                 AminoAcidModification phosphoRsModification;
                 if (_modificationMappingDictionary.TryGetValue(mod, out phosphoRsModification))
                 {
                     mods.Add(phosphoRsModification);
                 }
            }
            return mods.ToList();
        }

        private static string ModificationToString(Peptide peptide)
        {
            StringBuilder sb = new StringBuilder();
            foreach(IMass mod in peptide.GetModifications()) {
                if (mod == null)
                {
                    sb.Append('0');
                }
                else
                {
                    AminoAcidModification phosphoRsModification;
                    sb.Append(_modificationMappingDictionary.TryGetValue(mod, out phosphoRsModification) ? phosphoRsModification.ID : '0');
                }
            }
            sb.Insert(1, '.');
            sb.Insert(sb.Length - 1, '.');
          
            return sb.ToString();
        }

        public static int GetPsm(int peptideId)
        {
            return _psms[peptideId];
        }

        #endregion

       
    }
}
