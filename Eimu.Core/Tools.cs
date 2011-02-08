using System;

namespace Eimu.Core
{
    public static class Tools
    {
        public static ushort Create16(byte high, byte low)
        {
            return (ushort)((ushort)high << 8 | low);
        }

        public static void PrintBits(byte value)
        {
            Console.WriteLine();
            Console.Write(((value >> 7) & 0x1).ToString());
            Console.Write(((value >> 6) & 0x1).ToString());
            Console.Write(((value >> 5) & 0x1).ToString());
            Console.Write(((value >> 4) & 0x1).ToString());
            Console.Write(((value >> 3) & 0x1).ToString());
            Console.Write(((value >> 2) & 0x1).ToString());
            Console.Write(((value >> 1) & 0x1).ToString());
        }
    }
}
