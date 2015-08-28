using System;

namespace BlocksWorld
{
    [Flags]
    public enum TraceOptions
    {
        None = 0,
        IgnoreDynamic = 1,
        IgnoreStatic = 2
    }
}