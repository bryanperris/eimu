using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.SChip8
{
    public enum HLEMode
    {
        None = 0,
        VIP,
        ETI660,
        Telmac2000
    }

    public static class SystemHLE
    {
        private static Dictionary<ushort, Delegate> s_CallLookup = new Dictionary<ushort, Delegate>();

        public static void Call(HLEMode mode, ushort address, CodeEngine engine)
        {
        }
    }
}
