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
            foreach (Tuple<OmssaModification, int> mod in OmssaModification.ParseModificationLine(variableMods))
            {
                Peptide.SetModification(mod.Item1, mod.Item2);
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
