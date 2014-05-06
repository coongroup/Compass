using System.Collections.Generic;
using System.Text;
using System.Linq;
using CSMSL.Chemistry;
using CSMSL.Proteomics;

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

        public string GetProteinLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ProteinGroup);
            sb.Append(',');
            sb.Append(Defline);
            sb.Append(",\"");
            int count = 0;
            int psmsLocalized = 0;
            foreach (KeyValuePair<string, List<LocalizedHit>> kvp in Hits.OrderBy(kvp => kvp.Key))
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;

                int localized = kvp.Value.Count(hit => hit.IsLocalized);
                if (localized == 0)
                    continue;

                sb.Append(kvp.Key);
                sb.Append(",");
                psmsLocalized += localized;
                count++;
            }
            if (count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("\",");
            sb.Append(count);
            sb.Append(',');
            sb.Append(psmsLocalized);
            return sb.ToString();
        }

        public string Print(int quantStart, int quantEnd)
        {
            bool quant = quantStart >= 0;
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, List<LocalizedHit>> kvp in Hits.OrderBy(kvp => kvp.Key))
            {
                if (string.IsNullOrEmpty(kvp.Key))
                    continue;

                int psmsLocalized = kvp.Value.Count(hit => hit.IsLocalized);
                if (psmsLocalized == 0)
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
                sb.Append(psmsLocalized);
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
            bool first = false;
            for(int resNumber = 1; resNumber < mods.Length-1; resNumber++)
            {
                IMass mod = mods[resNumber];
                if (mod == null) 
                    continue;
                              
                ModificationCollection modCollection = mod as ModificationCollection;
                if (modCollection != null)
                {
                    IMass trueMod = mod;
                    bool passes = false;
                    foreach (IMass mod2 in modCollection)
                    {

                        if (Lotor.QuantifiedModifications.Contains(mod2))
                        {
                            trueMod = mod2;
                            passes = true;
                            break;  
                        }
                    }
                    if (!passes)
                        continue;
                    mod = trueMod;
                }
                else
                {
                    if (Lotor.FixedModifications.Contains(mod) || !Lotor.QuantifiedModifications.Contains(mod))
                        continue;                  
                }

                int fullResidueNumber = hit.PSM.StartResidue + resNumber - 1;
                char res = hit.LocalizedIsoform.Sequence[resNumber-1];
                if (first)
                {
                    sb.Append(" & ");              
                }
                sb.Append(res);
                sb.Append(fullResidueNumber);
                sb.Append("[" + mod + "]");
                first = true;
            }          
            return sb.ToString();
        }

    }
}
