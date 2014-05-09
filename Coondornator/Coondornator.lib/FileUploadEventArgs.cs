using System;

namespace Compass.Coondornator
{
    public class FileUploadEventArgs : EventArgs
    {
        public string FilePath { get; set; }

        public FileUploadEventArgs(string filePath)
        {
            FilePath = filePath;
        }

    }
}
