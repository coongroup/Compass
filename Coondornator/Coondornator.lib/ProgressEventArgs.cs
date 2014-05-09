using System;

namespace Compass.Coondornator
{
    public class ProgressEventArgs : EventArgs
    {
        public long Position { get; set; }
        public long Length { get; set; }

        public double Percent { get { return (double)Position / Length; } }

        public ProgressEventArgs(long position, long length)
        {
            Position = position;
            Length = length;
        }
    
    }
}
