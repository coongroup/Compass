using System;

namespace Coon.Compass.PhosphoRS
{
    public class MessageEvent : EventArgs
    {
        public string Message { get; set; }
        public MessageEvent(string message) { Message = message; }
    }
}
