using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSMSL.IO;

namespace Coon.Compass.DatabaseMaker
{
    public class FastaEvent : EventArgs
    {
        public Fasta Fasta { get; private set; }       

        public FastaEvent(Fasta fasta)
        {
            Fasta = fasta;
        }

    }
}
