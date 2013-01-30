using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace FdrOptimizer
{
    public class ModificationDictionary : SortedDictionary<string, Modification>
    {
        public ModificationDictionary() : base() { }

        public ModificationDictionary(string modsXmlFilepath)
            : base()
        {
            if(File.Exists(modsXmlFilepath))
            {
                ReadModificationsFromXmlFile(modsXmlFilepath, false);
            }
        }

        public void ReadModificationsFromXmlFile(string modsXmlFilepath, bool userModifications)
        {
            XmlDocument mods_xml = new XmlDocument();
            mods_xml.Load(modsXmlFilepath);

            XmlNamespaceManager mods_xml_ns = new XmlNamespaceManager(mods_xml.NameTable);
            mods_xml_ns.AddNamespace("omssa", mods_xml.ChildNodes[1].Attributes["xmlns"].Value);

            foreach(XmlNode mod_node in mods_xml.SelectNodes("/omssa:MSModSpecSet/omssa:MSModSpec", mods_xml_ns))
            {
                string mod_name = mod_node.SelectSingleNode("./omssa:MSModSpec_name", mods_xml_ns).FirstChild.Value;
                ModificationType mod_type = (ModificationType)int.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_type", mods_xml_ns).FirstChild.FirstChild.Value);
                double mod_monomass = double.Parse(mod_node.SelectSingleNode("./omssa:MSModSpec_monomass", mods_xml_ns).FirstChild.Value);

                XmlNodeList amino_acid_residues = mod_node.SelectNodes("./omssa:MSModSpec_residues/omssa:MSModSpec_residues_E", mods_xml_ns);
                List<char> mod_amino_acid_residues = amino_acid_residues.Count > 0 ? new List<char>(amino_acid_residues.Count) : null;
                foreach(XmlNode amino_acid_residue in amino_acid_residues)
                {
                    mod_amino_acid_residues.Add(amino_acid_residue.FirstChild.Value[0]);
                }

                Modification mod = new Modification(mod_name, mod_monomass, mod_type, mod_amino_acid_residues, userModifications);

                if(!ContainsKey(mod_name))
                {
                    Add(mod);
                }
                else
                {
                    this[mod_name] = mod;
                }
            }
        }

        private void Add(Modification modification)
        {
            Add(modification.Name, modification);
        }
    }
}
