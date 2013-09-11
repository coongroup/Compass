using System;

namespace Coon.Compass.DtaGenerator
{
    public class ExceptionEventArgs : EventArgs
    {
        private Exception exception;

        public Exception Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        public ExceptionEventArgs(Exception exception)
        {
            this.exception = exception;
        }
    }
}
