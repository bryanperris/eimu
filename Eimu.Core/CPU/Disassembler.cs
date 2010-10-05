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

namespace Eimu.Core.CPU
{
    public static class Disassembler
    {
        private static Dictionary<ushort, ChipOpcodes> s_Lookup;
		
		static Disassembler()
		{
            s_Lookup = new Dictionary<ushort, ChipOpcodes>();

            s_Lookup.Add(0, ChipOpcodes.Sys);
            s_Lookup.Add(0x00E0, ChipOpcodes.Clr);
            s_Lookup.Add(0x00EE, ChipOpcodes.Ret);
            s_Lookup.Add(0x1000, ChipOpcodes.Jp_1);
            s_Lookup.Add(0x2000, ChipOpcodes.Call);
            s_Lookup.Add(0x3000, ChipOpcodes.Se_3);
            s_Lookup.Add(0x4000, ChipOpcodes.Sne_4);
            s_Lookup.Add(0x5000, ChipOpcodes.Se_5);
            s_Lookup.Add(0x6000, ChipOpcodes.Ld_6);
            s_Lookup.Add(0x7000, ChipOpcodes.Add_7);
            s_Lookup.Add(0x8000, ChipOpcodes.Ld_8);
            s_Lookup.Add(0x8001, ChipOpcodes.Or);
            s_Lookup.Add(0x8002, ChipOpcodes.And);
            s_Lookup.Add(0x8003, ChipOpcodes.Xor);
            s_Lookup.Add(0x8004, ChipOpcodes.Add_8);
            s_Lookup.Add(0x8005, ChipOpcodes.Sub);
            s_Lookup.Add(0x8006, ChipOpcodes.Shr);
            s_Lookup.Add(0x8007, ChipOpcodes.Subn);
            s_Lookup.Add(0x800E, ChipOpcodes.Shl);
            s_Lookup.Add(0x9000, ChipOpcodes.Sne_9);
            s_Lookup.Add(0xA000, ChipOpcodes.Ld_A);
            s_Lookup.Add(0xB000, ChipOpcodes.Jp_B);
            s_Lookup.Add(0xC000, ChipOpcodes.Rnd);
            s_Lookup.Add(0xD000, ChipOpcodes.Drw);
            s_Lookup.Add(0xE09E, ChipOpcodes.Skp);
            s_Lookup.Add(0xE0A1, ChipOpcodes.Sknp);
            s_Lookup.Add(0xF007, ChipOpcodes.Ld_F_07);
            s_Lookup.Add(0xF00A, ChipOpcodes.Ld_F_0A);
            s_Lookup.Add(0xF015, ChipOpcodes.Ld_DT);
            s_Lookup.Add(0xF018, ChipOpcodes.Ld_ST);
            s_Lookup.Add(0xF01E, ChipOpcodes.Add_F);
            s_Lookup.Add(0xF029, ChipOpcodes.Ld_F_29);
            s_Lookup.Add(0xF033, ChipOpcodes.Ld_F_33);
            s_Lookup.Add(0xF055, ChipOpcodes.Ld_F_55);
            s_Lookup.Add(0xF065, ChipOpcodes.Ld_F_65);
		}

        public static ChipOpcodes DecodeInstruction(ChipInstruction instruction)
		{
            ChipOpcodes opcode;

			if (s_Lookup.TryGetValue((ushort)(instruction.RawInstruction & GetOpcodeMask(instruction)), out opcode))
				return opcode;
			else
				return ChipOpcodes.Unknown;
		}

        public static ushort GetOpcodeMask(ChipInstruction instruction)
        {
            ushort mask = 0xF000;

            switch (instruction.Opcode)
            {
                case 0x0: if (instruction.RawInstruction == 0x00E0 || instruction.RawInstruction == 0x00EE) mask = 0xFFFF; break;
                case 0xF:
                case 0xE: mask = 0xF0FF; break;
                case 0x8: mask = 0xF00F; break;
                default: mask = 0xF000; break;
            }

            return mask;
        }
    }
}
