using System;

namespace DtaGenerator
{
    public class ProgressEventArgs : EventArgs
    {
        public double Progress { get; set; }
        public RawFile RawFile { get; set; }

        public ProgressEventArgs(double progress, RawFile rawfile)
        {
            Progress = progress;
            RawFile = rawfile;
        }
    }
}
