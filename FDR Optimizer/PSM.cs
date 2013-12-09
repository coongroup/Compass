using System;
using System.Collections.Generic;
using CSMSL;
using CSMSL.Analysis.Identification;
using CSMSL.Chemistry;
using CSMSL.IO.OMSSA;
using CSMSL.Proteomics;

namespace Coon.Compass.FdrOptimizer
{
    public class PSM : IFalseDiscovery<double>, IComparable<PSM>, IMass
    {
        public int SpectrumNumber { get; private set; }
        public string FileName { get; set; }
        public double Score { get; set; }
        public int Charge { get; set; }
        public double IsolationMz { get; set; }

        public string Modificationstring { get; set; }

        public int IsotopeSelected { get; set; }

        public double AdjustedIsolationMass { get; set; }

        public double PrecursorMassError { get; set; }

        public double CorrectedPrecursorMassError { get; set; }
        
        public CSMSL.Proteomics.Peptide Peptide { get; private set; }

        public PSM(int spectrumNumber)
        {
            SpectrumNumber = spectrumNumber;
        }

        public void SetSequenceAndMods(string sequence, IList<Modification> fixedMods, string variableMods)
        {
            Peptide = new CSMSL.Proteomics.Peptide(sequence);
            Peptide.SetModifications(fixedMods);
            Modificationstring = variableMods;
            foreach (Tuple<Modification, int> modTuple in OmssaModification.ParseModificationLine(variableMods))
            {
                Modification mod = modTuple.Item1;
                int site = modTuple.Item2;
                if (site == 1 && mod.Sites.HasFlag(ModificationSites.NPep))
                {
                    Peptide.AddModification(mod, Terminus.N);
                }
                else if (site == Peptide.Length && mod.Sites.HasFlag(ModificationSites.PepC))
                {
                    Peptide.AddModification(mod, Terminus.C);
                }
                else
                {
                    Peptide.AddModification(mod, site);
                }
            }
        }

        public bool IsDecoy { get; set; }

        public double QValue { get; set; }

        public double FdrScoreMetric
        {
            get { return Score; }
        }

        public double MonoisotopicMass
        {
            get { return Peptide.MonoisotopicMass; }
        }

        public int CompareTo(PSM other)
        {
            return Score.CompareTo(other.Score);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1:f2} {2})", Peptide.SequenceWithModifications, Score, IsDecoy ? "Decoy" : "");
        }
    }
}
