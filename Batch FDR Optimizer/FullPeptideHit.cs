using System;
using MSFileReaderLib;

namespace BatchFdrOptimizer
{
    public class FullPeptideHit : PeptideHit
    {
        private bool lineHasExtendedInformation;

        private string line;

        public string Line
        {
            get { return line; }
            set { line = value; }
        }

        public string ExtendedLine
        {
            get { return lineHasExtendedInformation ? line : line + ',' + isolationMZ.ToString("R") + ',' + isolationMass.ToString("R") + ',' + theoreticalNeutralMass.ToString("R") + ',' + experimentalNeutralMass.ToString("R") + ',' + precursorMassError.ToString("R") + ',' + adjustedPrecursorMassError.ToString("R") + ',' + qValue.ToString("R"); }
        }

        public FullPeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool decoy,
            Peptide peptide,
            IXRawfile2 raw, int scanNumber, int firstScanNumber,
            int charge)
            : base(sequence, dynamicModifications, eValueScore, decoy, peptide, raw, scanNumber, firstScanNumber, charge)
        {
            this.line = line;
            lineHasExtendedInformation = false;
        }

        public FullPeptideHit(string line, string sequence, string dynamicModifications, double eValueScore, bool decoy, double isolationMZ, double isolationMass, double theoreticalNeutralMass, double experimentalNeutralMass, double precursorMassError, double adjustedPrecursorMassError, double qValue)
            : base(sequence, dynamicModifications, eValueScore, decoy, isolationMZ, isolationMass, theoreticalNeutralMass, experimentalNeutralMass, precursorMassError, adjustedPrecursorMassError, qValue)
        {
            this.line = line;
            lineHasExtendedInformation = true;
        }
    }
}
