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

    public delegate void MachineCall(CodeEngine engine);

    public static class SystemHLE
    {


    }
}
