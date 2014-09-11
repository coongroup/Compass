using System;

namespace Coon.Compass.PhosphoRS
{
    public class ProgressEvent : EventArgs
    {
        public double Percent { get; set; }
        public ProgressEvent(double percentage) { Percent = percentage; }
    }
}
