using System;

namespace Coon.Compass.DtaGenerator
{
    public class ProgressEventArgs : EventArgs
    {
        private int _progress;

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public ProgressEventArgs(int progress)
        {
            _progress = progress;
        }
    }
}
