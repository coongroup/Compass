using System;

namespace Lotor
{
    public class ProgressEventArgs : EventArgs
    {
        public double Percent { get; set; }

        public ProgressEventArgs(double percent)
        {
            Percent = percent;
        }
    }
}
