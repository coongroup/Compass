using System;

namespace Phosphinator
{
    public class FilepathEventArgs : EventArgs
    {
        private string filepath;

        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        public FilepathEventArgs(string filepath)
        {
            this.filepath = filepath;
        }
    }
}
