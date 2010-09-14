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

namespace Eimu.Core.CPU
{


	public struct ChipInstruction
	{
		ushort m_Instruction;
		
		public ChipInstruction (ushort instruction)
		{
			this.m_Instruction = instruction;
		}
		
		public ushort RawInstruction
		{
			get { return this.m_Instruction;}
		}
		
		public byte Opcode
		{
			get { return (byte)((this.m_Instruction & 0xF000) >> 12);}
		}
		
		/// <summary>
		/// Address value
		/// </summary>
		public ushort NNN
		{
			get { return (ushort)(this.m_Instruction & 0x0FFF);}
		}
		
		public byte N
		{
			get { return (byte)(this.m_Instruction & 0x000F);}
		}
		
		public byte X
		{
			get { return (byte)((this.m_Instruction & 0x0F00) >> 8); }
		}
		
		public byte Y
		{
			get { return (byte)((this.m_Instruction & 0x00F0) >> 4); }
		}
		
		public byte KK
		{
			get { return (byte)(this.m_Instruction & 0x00FF);}
		}
	}
}
