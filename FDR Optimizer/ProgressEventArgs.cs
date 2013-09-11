using System;

namespace Coon.Compass.FdrOptimizer
{
    public class ProgressEventArgs : EventArgs
    {
        private int progress;

        public int Progress
        {
            get { return progress; }
            set { progress = value; }
        }

        public ProgressEventArgs(int progress)
        {
            this.progress = progress;
        }
    }
}
