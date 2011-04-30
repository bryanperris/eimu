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

namespace Eimu.Core.Systems.Chip8X.CodeUtils
{
	public static class Disassembler
	{
		private static Dictionary<ushort, ChipOpCode> s_Lookup;
		
		static Disassembler()
		{
			s_Lookup = new Dictionary<ushort, ChipOpCode>();

			s_Lookup.Add(0x00E0, ChipOpCode.Clr);
			s_Lookup.Add(0x00EE, ChipOpCode.Ret);
			s_Lookup.Add(0x1000, ChipOpCode.Jp_1);
			s_Lookup.Add(0x2000, ChipOpCode.Call);
			s_Lookup.Add(0x3000, ChipOpCode.Se_3);
			s_Lookup.Add(0x4000, ChipOpCode.Sne_4);
			s_Lookup.Add(0x5000, ChipOpCode.Se_5);
			s_Lookup.Add(0x6000, ChipOpCode.Ld_6);
			s_Lookup.Add(0x7000, ChipOpCode.Add_7);
			s_Lookup.Add(0x8000, ChipOpCode.Ld_8);
			s_Lookup.Add(0x8001, ChipOpCode.Or);
			s_Lookup.Add(0x8002, ChipOpCode.And);
			s_Lookup.Add(0x8003, ChipOpCode.Xor);
			s_Lookup.Add(0x8004, ChipOpCode.Add_8);
			s_Lookup.Add(0x8005, ChipOpCode.Sub);
			s_Lookup.Add(0x8006, ChipOpCode.Shr);
			s_Lookup.Add(0x8007, ChipOpCode.Subn);
			s_Lookup.Add(0x800E, ChipOpCode.Shl);
			s_Lookup.Add(0x9000, ChipOpCode.Sne_9);
			s_Lookup.Add(0xA000, ChipOpCode.Ld_A);
			s_Lookup.Add(0xB000, ChipOpCode.Jp_B);
			s_Lookup.Add(0xC000, ChipOpCode.Rnd);
			s_Lookup.Add(0xD000, ChipOpCode.Drw);
			s_Lookup.Add(0xE09E, ChipOpCode.Skp);
			s_Lookup.Add(0xE0A1, ChipOpCode.Sknp);
			s_Lookup.Add(0xF007, ChipOpCode.Ld_F_07);
			s_Lookup.Add(0xF00A, ChipOpCode.Ld_F_0A);
			s_Lookup.Add(0xF015, ChipOpCode.Ld_DT);
			s_Lookup.Add(0xF018, ChipOpCode.Ld_ST);
			s_Lookup.Add(0xF01E, ChipOpCode.Add_F);
			s_Lookup.Add(0xF029, ChipOpCode.Ld_F_29);
			s_Lookup.Add(0xF033, ChipOpCode.Ld_F_33);
			s_Lookup.Add(0xF055, ChipOpCode.Ld_F_55);
			s_Lookup.Add(0xF065, ChipOpCode.Ld_F_65);
			s_Lookup.Add(0xF075, ChipOpCode.Ld_F_75);
			s_Lookup.Add(0xF085, ChipOpCode.Ld_F_85);
			s_Lookup.Add(0xF030, ChipOpCode.Ld_F_30);
			s_Lookup.Add(0x00FD, ChipOpCode.exit);
			s_Lookup.Add(0x00FE, ChipOpCode.extOff);
			s_Lookup.Add(0x00FF, ChipOpCode.extOn);
			s_Lookup.Add(0x00C0, ChipOpCode.scrollN);
			s_Lookup.Add(0x00FB, ChipOpCode.scrollR);
			s_Lookup.Add(0x00FC, ChipOpCode.scrollL);
		}

		public static ChipOpCode DecodeInstruction(ushort instruction)
		{
			ChipOpCode opcode;

			if (s_Lookup.TryGetValue((ushort)(instruction & GetOpcodeMask(instruction)), out opcode))
				return opcode;
			else
				return ChipOpCode.Unknown;
		}

		public static ushort GetOpcodeMask(ushort instruction)
		{
			ushort mask = 0xF000;
			byte opcode = (byte)((instruction & 0xF000) >> 12);

			switch (opcode)
			{
				case 0x0:
					{
						if (instruction > 0)
						{
							if ((instruction & 0xFFF0) == 0x00C0)
							{
								mask = 0xFFF0;
							}
							else
							{
								mask = 0xFFFF;
							}
						}
						break;
					}
				case 0xF:
				case 0xE: mask = 0xF0FF; break;
				case 0x8: mask = 0xF00F; break;
				default: mask = 0xF000; break;
			}

			return mask;
		}
	}
}
