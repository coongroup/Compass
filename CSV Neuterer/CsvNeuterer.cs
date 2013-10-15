using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CsvNeuterer
{
    class CsvNeuterer
    {
        private IList<string> csvFilepaths;
        private IEnumerable<Modification> lightFixedModifications;
        private IEnumerable<Modification> mediumFixedModifications;
        private IEnumerable<Modification> heavyFixedModifications;

        public CsvNeuterer(IList<string> csvFilepaths,
            IEnumerable<Modification> lightFixedModifications, IEnumerable<Modification> mediumFixedModifications, IEnumerable<Modification> heavyFixedModifications)
        {
            this.csvFilepaths = csvFilepaths;
            this.lightFixedModifications = lightFixedModifications;
            this.mediumFixedModifications = mediumFixedModifications;
            this.heavyFixedModifications = heavyFixedModifications;
        }

        public void Neuter()
        {
            Dictionary<string, List<string>> files = new Dictionary<string, List<string>>();
            foreach(string csv_filepath in csvFilepaths)
            {
                string base_csv_filepath = Path.Combine(Path.GetDirectoryName(csv_filepath), Path.GetFileNameWithoutExtension(csv_filepath).Substring(0, Path.GetFileNameWithoutExtension(csv_filepath).Length - 2) + Path.GetExtension(csv_filepath));
                if(!files.ContainsKey(base_csv_filepath))
                {
                    files.Add(base_csv_filepath, new List<string>());
                }
                files[base_csv_filepath].Add(csv_filepath);
            }

            foreach(KeyValuePair<string, List<string>> kvp in files)
            {
                using(StreamWriter output = new StreamWriter(kvp.Key))
                {
                    output.WriteLine("Spectrum number, Filename/id, Peptide, E-value, Mass, gi, Accession, Start, Stop, Defline, Mods, Charge, Theo Mass, P-value, NIST score");

                    foreach(string csv_filepath in kvp.Value)
                    {
                        IEnumerable<Modification> fixed_mods = null;
                        switch(char.ToUpper(Path.GetFileNameWithoutExtension(csv_filepath)[Path.GetFileNameWithoutExtension(csv_filepath).Length - 1]))
                        {
                            case 'L':
                                fixed_mods = lightFixedModifications;
                                break;
                            case 'M':
                                fixed_mods = mediumFixedModifications;
                                break;
                            case 'H':
                                fixed_mods = heavyFixedModifications;
                                break;
                        }

                        using(StreamReader csv = new StreamReader(csv_filepath))
                        {
                            string header = csv.ReadLine();

                            while(csv.Peek() != -1)
                            {
                                string line = csv.ReadLine();
                                string[] fields = Regex.Split(line, @",(?!(?<=(?:^|,)\s*\x22(?:[^\x22]|\x22\x22|\\\x22)*,)(?:[^\x22]|\x22\x22|\\\x22)*\x22\s*(?:,|$))");  // crazy regex to parse CSV with internal double quotes from http://regexlib.com/REDetails.aspx?regexp_id=621

                                string peptide = fields[2];
                                string variable_mods = fields[10];
                                variable_mods = variable_mods.Trim('"');

                                foreach(Modification modification in fixed_mods)
                                {
                                    switch(modification.ModificationType)
                                    {
                                        case ModificationType.AminoAcidResidue:
                                            foreach(char amino_acid_residue in modification.AminoAcidResidues)
                                            {
                                                for(int c = 0; c < peptide.Length; c++)
                                                {
                                                    if(char.ToUpper(peptide[c]) == amino_acid_residue)
                                                    {
                                                        //char residue = peptide[c];
                                                        //peptide = peptide.Remove(c, 1);
                                                        //peptide = peptide.Insert(c, char.ToLower(residue).ToString());

                                                        if(variable_mods.Length > 0)
                                                        {
                                                            variable_mods += " ,";
                                                        }
                                                        variable_mods += modification.Name + ':' + (c + 1).ToString();
                                                    }
                                                }
                                            }
                                            break;
                                        case ModificationType.ProteinNTerminus:
                                            throw new NotSupportedException("Fixed modifications of protein N-terminii are not currently supported.");
                                        case ModificationType.ProteinNTerminusAminoAcidResidue:
                                            throw new NotSupportedException("Fixed modifications of protein N-terminii at specific amino acid residues are not currently supported.");
                                        case ModificationType.ProteinCTerminus:
                                            throw new NotSupportedException("Fixed modifications of protein C-terminii are not currently supported.");
                                        case ModificationType.ProteinCTerminusAminoAcidResidue:
                                            throw new NotSupportedException("Fixed modifications of protein C-terminii at specific amino acid residues are not currently supported.");
                                        case ModificationType.PeptideNTerminus:
                                            if(variable_mods.Length > 0)
                                            {
                                                variable_mods += " ,";
                                            }
                                            variable_mods += modification.Name + ":0";
                                            break;
                                        case ModificationType.PeptideNTerminusAminoAcidResidue:
                                            foreach(char amino_acid_residue in modification.AminoAcidResidues)
                                            {
                                                if(char.ToUpper(peptide[0]) == amino_acid_residue)
                                                {
                                                    if(variable_mods.Length > 0)
                                                    {
                                                        variable_mods += " ,";
                                                    }
                                                    variable_mods += modification.Name + ":0";
                                                }
                                            }
                                            break;
                                        case ModificationType.PeptideCTerminus:
                                            if(variable_mods.Length > 0)
                                            {
                                                variable_mods += " ,";
                                            }
                                            variable_mods += modification.Name + ':' + (peptide.Length + 1).ToString();
                                            break;
                                        case ModificationType.PeptideCTerminusAminoAcidResidue:
                                            foreach(char amino_acid_residue in modification.AminoAcidResidues)
                                            {
                                                if(char.ToUpper(peptide[peptide.Length - 1]) == amino_acid_residue)
                                                {
                                                    if(variable_mods.Length > 0)
                                                    {
                                                        variable_mods += " ,";
                                                    }
                                                    variable_mods += modification.Name + ':' + (peptide.Length + 1).ToString();
                                                }
                                            }
                                            break;
                                    }
                                }

                                fields[2] = peptide;
                                fields[10] = variable_mods;
                                if(fields[10].Contains(","))
                                {
                                    fields[10] = '"' + fields[10] + '"';
                                }

                                output.WriteLine(string.Join(",", fields));
                            }
                        }
                    }
                }
            }
        }
    }
}
