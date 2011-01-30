using System;

namespace Eimu.Core
{
    public static class Tools
    {
        public static ushort Create16(byte high, byte low)
        {
            return (ushort)((ushort)high << 8 | low);
        }
    }
}
