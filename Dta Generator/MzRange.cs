using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DtaGenerator
{
    public struct MzRange
    {
        public double Min;
        public double Max;

        public MzRange(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("[{0:0.000}-{1:0.000}]", Min, Max);
        }
    }
}
