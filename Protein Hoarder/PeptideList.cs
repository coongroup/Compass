
using System.Collections.Generic;

namespace Protein_Hoarder
{
    public class PeptideList : IEnumerable<Peptide>
    {
        private List<Peptide> peptides { get; set; }

        public int Count { get { return peptides.Count; } }

        public double LowestPvalue { get; private set; }

        public double CumulativePValue { get; private set; }

        public Peptide this[int index]
        {
            get
            {
                return peptides[index];
            }
        }

        public string Sequence
        {
           get 
           {
               return peptides[0].LeucineSequence;
           }
        }

        public PeptideList(Peptide pep)
        {
            peptides = new List<Peptide>();
            LowestPvalue = double.MaxValue;
            CumulativePValue = 1;
            Add(pep);
        }

        public void Add(Peptide peptide)
        {
            peptides.Add(peptide);
            if (peptide.PValue < LowestPvalue)
            {
                LowestPvalue = peptide.PValue;
            }
            CumulativePValue *= peptide.PValue;
        }

        public override string ToString()
        {
            return "Count = " + Count.ToString();
        }

        public IEnumerator<Peptide> GetEnumerator()
        {
            return peptides.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return peptides.GetEnumerator();
        }
    }
}
