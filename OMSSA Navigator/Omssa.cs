using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using OmssaLib;

namespace OmssaNavigator
{
    public class Omssa
    {
        private Process process;
        public event EventHandler Exited;
        public event DataReceivedEventHandler UpdateStatus;
        public bool IsSearching
        {
            get
            {
                if(process == null) return false;
                return !process.HasExited;
            }
        }

        public Omssa()
        {
            NewProcess();
        }

        public void NewProcess()
        {
            process = new Process();
            process.StartInfo.FileName = "omssacl.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
        }

        public void StartSearch(ArgumentLine argumentFile)
        {
            NewProcess();
            process.StartInfo.Arguments = argumentFile.ToString();
            process.Exited += Exited;
            process.ErrorDataReceived += UpdateStatus;
            process.Disposed += new EventHandler(process_Disposed);
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        public string RunCommand(string command)
        {
            NewProcess();
            process.StartInfo.Arguments = command;
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            process = null;
            return result;
        }

        public Dictionary<string, int> ReadModFile(string filePath)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            using(XmlReader reader = new XmlTextReader(new StreamReader(filePath)))
            {
                while(reader.ReadToFollowing("MSModSpec"))
                {
                    reader.ReadToFollowing("MSModSpec_mod");
                    reader.ReadToFollowing("MSMod");
                    int mod_number = reader.ReadElementContentAsInt();
                    reader.ReadToFollowing("MSModSpec_name");
                    string mod_name = reader.ReadElementContentAsString();
                    dict.Add(mod_name, mod_number);
                }
            }
            return dict;
        }

        public Dictionary<string, int> ReadInDictionary(string command)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            string result = RunCommand(command);
            if(result != null)
            {
                string[] lines = result.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string line in lines)
                {
                    string[] data = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = data[1].Trim();
                    dict.Add(name, int.Parse(data[0]));
                }
            }
            return dict;
        }

        public void Quit()
        {
            if(process != null)
            {
                process.Exited -= Exited;
                process.ErrorDataReceived -= UpdateStatus;
                process.Disposed -= process_Disposed;

                if(!process.HasExited) process.Kill();
            }
            process = null;
        }

        private void process_Disposed(object sender, EventArgs e)
        {
            Exited.Invoke(sender, e);
        }
    }
}