using System.Collections.Generic;
using System.Text;

namespace Compass.ProteinHoarder
{
    public class Quantitation
    {
        public int Plex { get; set; }

        public int PSMs { get; private set; }

        public HashSet<Peptide> Peptides { get; set; }

        private double[] data;

        public Quantitation(int plex, params double[] data)
        {
            Plex = plex;
            Peptides = new HashSet<Peptide>();
            this.data = new double[(Plex * 4) + 1];
            PSMs = 1;
            for (int i = 0; i < data.Length; i++)
            {
                this.data[i] = data[i];
            }
        }

        public void AddData(params double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.data[i] += data[i];
            }
            PSMs++;
        }

        public string ToOutput()
        {
            StringBuilder sb = new StringBuilder();
            foreach (double datum in data)
            {
                sb.Append(datum);
                sb.Append(',');
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
    }
}