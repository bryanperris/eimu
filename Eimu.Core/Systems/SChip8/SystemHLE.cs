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
                case 0x0402: VIP_Func402(engine); break;
                case 0x03FF: VIP_Fun03FF(engine); break;
                case 0x0414: VIP_Func414(engine); break;
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

        private static void VIP_Func402(CodeEngine engine)
        {
            //0402:  LDA 5
            //0403:  STR A     
            

            engine.CurrentMemory[engine.m_IReg] = engine.CurrentMemory[engine.PC];
            VIP_Func404(engine);
        }

        private static void VIP_Func404(CodeEngine engine)
        {
            //0404:  SEX 5
            //0405:  GLO A
            //0406:  ADD
            //0407:  PLO A
            //0408:  INC 5
            //0409:  GHI A     
            //040A:  ADC 00                  
            //040C:  PHI A  
            //040D:  SEP 4         

            engine.m_IReg++;
            engine.IncrementPC();
        }

        private static void VIP_Func414(CodeEngine engine)
        {
            //0414:  LDA 5
            //0415:  PLO 6
            //0416:  LDN 6
            //0417:  STR A
            //0418:  BR 04

            engine.CurrentMemory[engine.m_IReg] = engine.CurrentMemory[engine.CurrentMemory[engine.PC]]; // Address of VX or Value of VX
            VIP_Func402(engine); // ugh...

        }

        private static void VIP_Fun03FF(CodeEngine engine)
        {
            // Gets the HI byte of the display page pointer, so using 7F all the time for now (no idea what a DP is :P)
            // 03FF:  GHI B
            // 0400:  PHI A
            // 0401:  SEP 4

            engine.m_IReg &= ((0x7F << 8) & 0xFF);
        }
    }
}
