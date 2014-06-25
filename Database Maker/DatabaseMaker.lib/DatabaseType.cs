using System;

namespace Coon.Compass.DatabaseMaker
{
    [Flags]
    public enum DatabaseType
    {
        Target = 1,
        Decoy = 2,
        Concatenated = Target | Decoy
    }
}