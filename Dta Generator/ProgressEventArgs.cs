using System;

namespace Coon.Compass.DtaGenerator
{
    public class ProgressEventArgs : EventArgs
    {        
        public double Progress { get; private set; }
       
        public ProgressEventArgs(double progress)
        {
            Progress = progress;
        }
    }
}
