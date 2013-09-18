using System.Collections.Generic;

namespace Phosphinator
{
    public class ProteinSiteCounter : Dictionary<string, Dictionary<string, int>>
    {
        public ProteinSiteCounter() : base() { }

        public int Proteins
        {
            get { return Count; }
        }

        public int Sites
        {
            get
            {
                int sites = 0;
                foreach(Dictionary<string, int> value in Values)
                {
                    sites += value.Count;
                }
                return sites;
            }
        }
    }
}
