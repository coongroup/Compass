using System;

namespace OmssaNavigator
{
    public class ExitEventArgs : EventArgs
    {
        public DtaFile DTA { get; set; }

        public ExitEventArgs(DtaFile dtafile)
        {
            DTA = dtafile;
        }
    }
}
