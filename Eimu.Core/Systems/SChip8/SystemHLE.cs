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
        public static void Call(HLEMode mode, ushort address, CodeEngine engine)
        {
            switch (mode)
            {
                case HLEMode.VIP: VIPCall(address, engine); break;
                default: break;
            }
        }

        private static void VIPCall(ushort address, CodeEngine engine)
        {
            switch (address)
            {
                case 0x61e: VIP_ClearRAM(engine); break;
                default: break;
            }
        }

        private static void VIP_ClearRAM(CodeEngine engine)
        {
            /*
            061E:  LDI 39
            0620:  PLO F
            0621:  LDI 00
            0623:  STR A
            0624:  INC A
            0625:  DEC F
            0626:  GLO F
            0627:  BNZ 21
            0629:  SEP 4
             */
            int s = engine.m_IReg;

            for (int i = 0; i < 0x3A; i++)
            {
                engine.CurrentMemory[s + i] = 0;
                engine.m_IReg++;
            }
        }
    }
}
