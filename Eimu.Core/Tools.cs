using System;

namespace Eimu.Core
{
    public static class Tools
    {
        public static ushort MakeShort(byte a, byte b)
        {
            return (ushort)((ushort)a << 8 | b);
        }
    }
}
