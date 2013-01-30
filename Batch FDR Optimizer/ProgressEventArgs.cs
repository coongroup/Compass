using System;

namespace BatchFdrOptimizer
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
            Progress = progress;
            if (progress > 100) Progress = 100;
        }
    }
}
