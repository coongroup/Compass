using System.Collections.Generic;

namespace Compass.ProteinHoarder
{
    public class PsmList : IEnumerable<PSM>
    {
        private List<PSM> Psms { get; set; }

        public int Count { get { return Psms.Count; } }

        public double LowestPvalue { get; private set; }

        public double CumulativePValue { get; private set; }

        public PSM this[int index]
        {
            get
            {
                return Psms[index];
            }
        }

        public PsmList()
        {
            Psms = new List<PSM>();
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
            Psms.Add(psm);
            if (psm.PValue < LowestPvalue)
            {
                LowestPvalue = psm.PValue;
            }
            CumulativePValue *= psm.PValue;
        }

        public override string ToString()
        {
            return "Count = " + Count;
        }

        public IEnumerator<PSM> GetEnumerator()
        {
            return Psms.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Psms.GetEnumerator();
        }
    }
}