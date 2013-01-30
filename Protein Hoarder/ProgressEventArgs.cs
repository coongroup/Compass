using System;

namespace Protein_Hoarder
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