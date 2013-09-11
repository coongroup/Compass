using System;

namespace Coon.Compass.TagQuant
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