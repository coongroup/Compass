using System;

namespace Coon.Compass.FdrOptimizer
{
    public class StatusEventArgs : EventArgs
    {
        public string Message { get; set; }

        public StatusEventArgs(string msg)
        {
            Message = msg;
        }
    }
}