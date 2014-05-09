using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Compass.Coondornator
{
    public class OmssaParameterLine
    {
        // TODO make it so only valid commands are captured
        private static readonly Regex omssaRegex = new Regex(@"-(\w+)(?:\s([\d.-]+))?(?:\s|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex validateOmssaLine = new Regex("^(" + omssaRegex + ")+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Name { get; private set; }

        public List<OmssaParameter> Parameters;

        public OmssaParameterLine(string name, IEnumerable<OmssaParameter> parameters = null)
        {
            Name = name;
            Parameters = new List<OmssaParameter>(parameters);
        }

        public OmssaParameterLine(string name, string line)
        {
            Name = name;
            Parameters = new List<OmssaParameter>();
            if (!ParseLine(line))
            {
                throw new ArgumentException("The following line cannot be parsed: " + line);
            }
        }

        private bool ParseLine(string line)
        {
            var matches = omssaRegex.Matches(line);
           
            foreach (Match match in matches)
            {
                if (!match.Success)
                    return false;

                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                OmssaParameter parameter = new OmssaParameter(key, value);
                Parameters.Add(parameter);
            }
            return true;
        }

        public override string ToString()
        {
            return string.Join(" ", Parameters.OrderBy(p => p.Name));
        }

        public static bool Validate(string line)
        {
            return validateOmssaLine.IsMatch(line);
        }

        #region LoadFromFile

        private static readonly string UserPath;
        private static readonly Dictionary<string, OmssaParameterLine> ParameterLines;

        static OmssaParameterLine()
        {
            UserPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"COMPASS\Coondornator\OmssaParameters.xml");
            ParameterLines = new Dictionary<string, OmssaParameterLine>();
          
            // Load the default modification file
            Load();
        }

        public static IEnumerable<OmssaParameterLine> GetAllParameterLines()
        {
            return ParameterLines.Values;
        } 

        public static void Load()
        {
            // Create file if it doesn't exist
            if (!System.IO.File.Exists(UserPath))
            {
                RestoreDefaults();
            }

            Load(UserPath);
        }

        public static void Load(string filePath)
        {
            try
            {
                var modsXml = new XmlDocument();
                modsXml.Load(filePath);

                foreach (XmlNode parameterLine in modsXml.SelectNodes("//Coondornator/OmssaParameterLines/OmssaParameterLine"))
                {
                    string name = parameterLine.Attributes["name"].Value;

                    List<OmssaParameter> parameters = new List<OmssaParameter>();
                    foreach (XmlNode parameterNode in parameterLine.SelectNodes("OmssaParameter"))
                    {
                        string key = parameterNode.Attributes["name"].Value;
                        string value = parameterNode.InnerText;
                        parameters.Add(new OmssaParameter(key, value));
                    }

                    OmssaParameterLine line = new OmssaParameterLine(name, parameters);
                    ParameterLines[name] = line;
                }
                OnChanged(false);

            }
            catch (XmlException)
            {

                //RestoreDefaults();
            }
        }

        public static void Save()
        {
            SaveTo(UserPath);
        }

        /// <summary>
        /// Saves the current modifications and isotopologues
        /// </summary>
        public static void SaveTo(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("Coondornator");
                writer.WriteStartElement("OmssaParameterLines");

                foreach (var parameterLine in ParameterLines.Values)
                {
                    writer.WriteStartElement("OmssaParameterLine");
                    writer.WriteAttributeString("name", parameterLine.Name);


                    foreach (var parameter in parameterLine.Parameters)
                    {
                        writer.WriteStartElement("OmssaParameter");
                        writer.WriteAttributeString("name", parameter.Name);

                        if (!string.IsNullOrWhiteSpace(parameter.Value))
                        {
                            writer.WriteString(parameter.Value);
                        }

                        writer.WriteEndElement(); // OmssaParameter
                    }
                    
                    writer.WriteEndElement(); // OmssaParameterLine
                }

                writer.WriteEndElement(); // OmssaParameterLines
                writer.WriteEndElement(); // Coondornator

                writer.WriteEndDocument();
            }
        }

        public static void AddLine(string name, string line, bool saveToDisk = true)
        {
            AddLine(new OmssaParameterLine(name, line), saveToDisk);
        }

        public static void AddLine(OmssaParameterLine line, bool saveToDisk = true)
        {
            ParameterLines[line.Name] = line;

            OnChanged(saveToDisk);
        }

        public static bool RemoveLine(string name)
        {
            if (!ParameterLines.Remove(name))
                return false;

            OnChanged();
            return true;
        }

        public static void RestoreDefaults()
        {
            ParameterLines.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            Stream defaultModsStream = assembly.GetManifestResourceStream("Compass.Coondornator.Resources.DefaultOmssaParameters.xml");

            Directory.CreateDirectory(Path.GetDirectoryName(UserPath));
            using (var fileStream = System.IO.File.Create(UserPath))
            {
                if (defaultModsStream != null) defaultModsStream.CopyTo(fileStream);
            }

            Load();
        }

        private static void OnChanged(bool saveToDisk = true)
        {
            // Flush to disk
            if (saveToDisk)
                Save();

            var handler = Changed;
            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }

        public static event EventHandler Changed;

        #endregion
    }
}
