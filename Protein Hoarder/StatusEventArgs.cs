using System;

namespace Protein_Hoarder
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