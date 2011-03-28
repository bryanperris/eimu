using System;
using System.Text;

namespace Eimu.Core
{
    public static class Tools
    {
        public static ushort Create16(byte high, byte low)
        {
            return (ushort)((ushort)high << 8 | low);
        }

        public static string PrintBits(byte value)
        {
            string a = "";
            a += ((value >> 7) & 0x1).ToString();
            a += ((value >> 6) & 0x1).ToString();
            a += ((value >> 5) & 0x1).ToString();
            a += ((value >> 4) & 0x1).ToString();
            a += ((value >> 3) & 0x1).ToString();
            a += ((value >> 2) & 0x1).ToString();
            a += ((value >> 1) & 0x1).ToString();
            a += (value        & 0x1).ToString();
            return a;
        }
    }
}
