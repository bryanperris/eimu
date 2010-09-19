using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core.Devices;

namespace Eimu.Core.CPU
{
    partial class Interpreter
    {
        [OpcodeTag(ChipOpcodes.Clr)]
        private void Clr(ChipInstruction inst)
        {
            //this.GraphicsDeviceCallback(Eimu.Core.Devices.GraphicsCommand.Cls, 0);
        }

        [OpcodeTag(ChipOpcodes.Drw)]
        private void Drw(ChipInstruction inst)
        {
            this.SetRegisterV(0xF, 0);
            byte x = GetRegisterV(inst.X);
            byte y = GetRegisterV(inst.Y);
            byte pixelWidth = GraphicsDevice.SPRITE_WIDTH;
            byte pixelHeight = inst.N;
            byte pixel;

            for (byte i = 0; i < pixelHeight; i++)
            {
                pixel = this.m_Memory.GetValueByRawAddress(this.m_IReg + i);

                for (byte j = 0; j < pixelWidth; j++)
                {
                    if ((pixel & (0x80 >> j)) != 0)
                    {
                        //this.GraphicsDeviceCallback(Eimu.Core.Devices.GraphicsCommand.SetPixel, (ushort)(x + i) 
                    }
                }
            }
        }
    }
}
