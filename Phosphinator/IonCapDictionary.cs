using System.Collections.Generic;

namespace Phosphinator
{
    public class IonCapDictionary : Dictionary<FragmentType, IonCap>
    {
        public IonCapDictionary()
            : base()
        {
            Add(FragmentType.b, new IonCap(0.0));
            Add(FragmentType.c, new IonCap(17.0265491015));
            Add(FragmentType.y, new IonCap(18.0105646942));
            Add(FragmentType.zdot, new IonCap(1.991840552567));  // (z = 0.9840155927) + proton mass + electron mass
        }
    }
}
