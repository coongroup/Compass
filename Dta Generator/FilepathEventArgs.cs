using System;

namespace DtaGenerator
{
    public class FilepathEventArgs : EventArgs
    {
        private string _filepath;

        public string Filepath
        {
            get { return _filepath; }
            set { _filepath = value; }
        }

        public FilepathEventArgs(string filepath)
        {
            _filepath = filepath;
        }
    }
}
