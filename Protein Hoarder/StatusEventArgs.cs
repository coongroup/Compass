using System;

namespace Compass.ProteinHoarder
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