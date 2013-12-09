using System.Collections.Generic;
using System.Text;
using System.Linq;
using CSMSL.Chemistry;

namespace Coon.Compass.Lotor
{
    public class Protein
    {
        public string Defline { get; set; }
        public string ProteinGroup { get; set; }

        public Dictionary<string, List<LocalizedHit>> Hits { get; private set; }

        public Protein(string group, string defline)
        {
            ProteinGroup = group;
            Defline = defline;
            Hits = new Dictionary<string, List<LocalizedHit>>();
        }

        public void AddHit(LocalizedHit hit)
        {
            string localizedString = GetLocalizedString(hit);
            List<LocalizedHit> hits = null;
            if (Hits.TryGetValue(localizedString, out hits))
            {
                hits.Add(hit);
            }
            else
            {
                hits = new List<LocalizedHit>();
                hits.Add(hit);
                Hits.Add(localizedString, hits);
            }
        }

        public string Print(int quantStart, int quantEnd)
        {
            bool quant = quantStart >= 0;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, List<LocalizedHit>> kvp in Hits)
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;
                sb.Append(ProteinGroup);
                sb.Append(',');
                sb.Append(Defline);
                sb.Append(',');
                sb.Append(kvp.Key); // Isoform
                int ptms = kvp.Key.Count(c => c.Equals('&')) + 1; //kvp.Key.Length - kvp.Key.Replace("&", "").Length + 1;
                sb.Append(',');
                sb.Append(ptms);
                sb.Append(',');
                sb.Append(kvp.Value.Count);
                sb.Append(',');
                sb.Append(kvp.Value.Count(hit => hit.IsLocalized));
                // quant
                if (quant)
                {
                    
                    double[] quantInfo = new double[quantEnd - quantStart + 1];

                    int numLocalized = 0;
                    foreach (LocalizedHit hit in kvp.Value)
                    {
                        if (hit.IsLocalized)
                        {
                            numLocalized++;
                            for (int i = quantStart; i <= quantEnd; i++)
                            {
                                quantInfo[i - quantStart] += double.Parse(hit.omssapsm[i]);
                            }
                        }
                    }
                    sb.Append(',');
                    for (int i = 0; i < quantInfo.Length; i++)
                    {
                        double data = quantInfo[i];
                        sb.Append(data);
                        sb.Append(',');
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.Append('\n');

            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        private static string GetLocalizedString(LocalizedHit hit)
        {
            StringBuilder sb = new StringBuilder();
            IMass[] mods = hit.LocalizedIsoform.GetModifications();
            for(int resNumber = 1; resNumber < mods.Length-1; resNumber++)
            {
                IMass mod = mods[resNumber];
                if (mod == null || Lotor.FixedModifications.Contains(mod)) 
                    continue;
                int fullResidueNumber = hit.PSM.StartResidue + resNumber - 1;
                char res = hit.LocalizedIsoform.Sequence[resNumber-1];
                sb.Append(res);
                sb.Append(fullResidueNumber);
                sb.Append(" [" + mod + "] &");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

    }
}
