/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
            ScreenClear();
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
                        PixelSet(x + i, y + j);
                    }
                }
            }
        }
    }
}
