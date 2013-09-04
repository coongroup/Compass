using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coon;

namespace Lotor
{
    public class Protein
    {
        public string Name { get; set; }
        public string Defline { get; set; }

        public List<SiteIsoform> Isoforms { get; set; }
        public Dictionary<string, List<LocalizedHit>> Hits { get; private set; }

        public Protein(string name)
        {
            Name = name;
            Isoforms = new List<SiteIsoform>();
            Hits = new Dictionary<string, List<LocalizedHit>>();
        }

        public void AddHit(LocalizedHit hit, List<string> localazingPTMs) 
        {
            string localizedString = GetLocalizedString(hit, localazingPTMs);
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
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, List<LocalizedHit>> kvp in Hits)
            {

                sb.Append(Defline);
                sb.Append(',');
                sb.Append(kvp.Key); // Isoform
                int forms = kvp.Key.Length - kvp.Key.Replace("|", "").Length + 1;
                sb.Append(',');
                sb.Append(forms);
                sb.Append(',');
                sb.Append(kvp.Value.Count);
                sb.Append(',');

                // quant
                double[] quantInfo = new double[quantEnd - quantStart + 1];
                for (int i = 0; i < quantInfo.Length; i++)
                {
                    quantInfo[i] = 0;
                }
                foreach (LocalizedHit hit in kvp.Value)
                {
                    if (!hit.isCTermModified)
                    {
                        for (int i = quantStart; i <= quantEnd; i++)
                        {
                            quantInfo[i - quantStart] += double.Parse(hit.omssapsm[i]);
                        }
                    }
                }
                for (int i = 0; i < quantInfo.Length; i++)
                {
                    sb.Append(quantInfo[i]);
                    sb.Append(',');
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append('\n');
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        private static string GetLocalizedString(LocalizedHit hit, List<string> localizingPTMs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (int resNumber in hit.LocalizedIsoform.GetModifiedSites(localizingPTMs))
            {                
                int fullResidueNumber = hit.PSM.StartResidue + resNumber - 1;
                char res = hit.LocalizedIsoform[resNumber-1].OneLetterAbbreviation;
                sb.Append(res);
                sb.Append(fullResidueNumber);
                sb.Append("|");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

    }
}
