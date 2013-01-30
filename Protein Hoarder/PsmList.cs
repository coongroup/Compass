using System.Collections.Generic;

namespace Protein_Hoarder
{
    public class PsmList : IEnumerable<PSM>
    {
        private List<PSM> psms { get; set; }

        public int Count { get { return psms.Count; } }

        public double LowestPvalue { get; private set; }

        public double CumulativePValue { get; private set; }

        public PSM this[int index]
        {
            get
            {
                return psms[index];
            }
        }

        public PsmList()
        {
            psms = new List<PSM>();
            LowestPvalue = double.MaxValue;
            CumulativePValue = 1;
        }

        public PsmList(PSM psm)
            :this()
        {           
            Add(psm);
        }

        public void Add(PSM psm)
        {
            psms.Add(psm);
            if (psm.PValue < LowestPvalue)
            {
                LowestPvalue = psm.PValue;
            }
            CumulativePValue *= psm.PValue;
        }

        public override string ToString()
        {
            return "Count = " + Count.ToString();
        }

        public IEnumerator<PSM> GetEnumerator()
        {
            return psms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return psms.GetEnumerator();
        }
    }
}