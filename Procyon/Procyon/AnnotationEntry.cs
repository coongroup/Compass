using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coon.Compass.Procyon
{
    public class AnnotationEntry
    {
        public string Name;
        public string UniqueID;
        public AnnotationType Type;

        public AnnotationEntry(string uniqueID, string name, AnnotationType type)
        {
            UniqueID = uniqueID;
            Name = name;
            Type = type;
        }
    }
}
