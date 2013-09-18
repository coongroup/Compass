using System.Collections.Generic;

namespace Phosphinator
{
    public class AminoAcidDictionary : Dictionary<char, AminoAcid>
    {
        public AminoAcidDictionary()
            : base()
        {
            Add(new AminoAcid('G', 57.0214637312));
            Add(new AminoAcid('A', 71.0371137952));
            Add(new AminoAcid('S', 87.0320284252));
            Add(new AminoAcid('P', 97.0527638592));
            Add(new AminoAcid('V', 99.0684139232));
            Add(new AminoAcid('T', 101.0476784892));
            Add(new AminoAcid('C', 103.0091844952));
            Add(new AminoAcid('L', 113.0840639872));
            Add(new AminoAcid('I', 113.0840639872));
            Add(new AminoAcid('N', 114.0429274624));
            Add(new AminoAcid('D', 115.0269430552));
            Add(new AminoAcid('Q', 128.0585775264));
            Add(new AminoAcid('K', 128.0949630244));
            Add(new AminoAcid('E', 129.0425931192));
            Add(new AminoAcid('M', 131.0404846232));
            Add(new AminoAcid('H', 137.0589118696));
            Add(new AminoAcid('F', 147.0684139232));
            Add(new AminoAcid('R', 156.1011110348));
            Add(new AminoAcid('Y', 163.0633285532));
            Add(new AminoAcid('W', 186.0793129604));
        }

        public void Add(AminoAcid aminoAcid)
        {
            Add(aminoAcid.Abbreviation, aminoAcid);
        }
    }
}
