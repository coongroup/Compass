using System;

namespace Coon.Compass.Lotor
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
