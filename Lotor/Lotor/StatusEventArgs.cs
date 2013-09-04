using System;

namespace Lotor
{
    public class StatusEventArgs : EventArgs
    {
        public string Message { get; set; }
        public bool IsError {get;set;}

        public StatusEventArgs(string msg, bool isError = false)
        {
            Message = msg;
            IsError = isError;
        }
    }
}
