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
    public enum ChipOpcodes
    {
		/// <summary>
		/// Uknown Opcode 
		/// </summary>
        Unknown = 0,
		
		/// <summary>
		/// Jump to a machine code routine at nnn.  Ignored opcode. 
		/// </summary>
		Sys,
		
		/// <summary>
		/// Clear the display 
		/// </summary>
        Clr,
		
		/// <summary>
		/// Return from a subroutine. PC is set to the top value in the stack and decrements the stack pointer. 
		/// </summary>
		Ret,
		
		/// <summary>
		/// Jump to location nnn. PC is set to nnn 
		/// </summary>
		Jp_1,
		
		/// <summary>
		/// Call Subroutine at nnn.  Increments PC then pushes PC value on stack, then sets PC to nnn.
		/// </summary>
		Call,
		
		/// <summary>
		/// Skip next instruction if register Vx == Kk, if equal, increments PC by 2
		/// </summary>
		Se_3,
		
		/// <summary>
		/// Skip next instruction if register Vx != Kk, if not equal, increments PC by 2
		/// </summary>
		Sne_4,
		
		/// <summary>
		/// Skip next instruction if register Vx = Vy, if equal, increments PC by 2 
		/// </summary>
		Se_5,
		
		/// <summary>
		/// Load value Kk into register Vx
		/// </summary>
		Ld_6,
		
		/// <summary>
		/// Vx += kk
		/// </summary>
		Add_7,
		
		/// <summary>
		/// Stores Vy value in register Vx 
		/// </summary>
		Ld_8,
		
		/// <summary>
		///Vx |= Vy 
		/// </summary>
		Or,
		
		/// <summary>
		/// Vx &= Vy
		/// </summary>
		And,
		
		/// <summary>
		/// Vx ^= Vy 
		/// </summary>
		Xor,
		
		/// <summary>
		/// Vx += Vy, Sets VF = Carry if value is greater than 1 byte 
		/// </summary>
		Add_8,
		
		/// <summary>
		/// Vx -= Vy, Set VF = Not borrow if Vx > Vy 
		/// </summary>
		Sub,
		
		/// <summary>
		/// Vx >>= 1 
		/// </summary>
		Shr,
		
		/// <summary>
		/// Vx = Vy - Vx, set VF = Not Borrow, if Yv > Vx
		/// </summary>
		Subn,
		
		/// <summary>
		/// Vx <<= 1 
		/// </summary>
		Shl,
		
		/// <summary>
		/// Skip next if Vx != Yy, if not equal, PC is increased by 2 
		/// </summary>
		Sne_9,
		
		/// <summary>
		/// Set I = nnn 
		/// </summary>
		Ld_A,
		
		/// <summary>
		/// Jump to location nnn + V0, PC is set to (nnn + V0) 
		/// </summary>
		Jp_B,
		
		/// <summary>
		/// Random value of 255 than ANDed by Kk, results stored in Vx. 
		/// </summary>
		Rnd,
		
		/// <summary>
		///Display n-byte sprite starting at memory location I at (Vx, Vy), set VF = collision.  Sprites are XOR'd onto screen. If any pixels are erased
		// then VF is set to 1, else 0. Also sprites can wrap on the screen edges.
		/// </summary>
		Drw,
		
		/// <summary>
		/// Skip next instruction if the key with the value of Vx is pressed, if true, then increment the PC by 2. 
		/// </summary>
		Skp,
		
		/// <summary>
		/// Skip the next instruction if the key with the value of Vx is not pressed, if true, then increment the PC by 2. 
		/// </summary>
		Sknp,
		
		/// <summary>
		///Set Vx = delay timer value 
		/// </summary>
		Ld_F_07,
		
		/// <summary>
		/// Wait for a key press, store key value in Vx, this pauses execution 
		/// </summary>
		Ld_F_0A,
		
		/// <summary>
		/// Set Delay Timer = Vx 
		/// </summary>
		Ld_DT,
		
		/// <summary>
		/// Set sound timer = Vx 
		/// </summary>
		Ld_ST,
		
		/// <summary>
		/// I += Vx 
		/// </summary>
		Add_F,
		
		/// <summary>
		/// Set I to location of sprite for digit Vx 
		/// </summary>
		Ld_F_29,
		
		/// <summary>
		/// Store BCD representation of Vx in memory locations I, I+1, I+2
		/// I is the hundreds digit, I+1 is tens, I+2 is ones 
		/// </summary>
		Ld_F_33,
		
		/// <summary>
		/// Store registers V0 through Vx in memory starting at location I 
		/// </summary>
		Ld_F_55,
		
		/// <summary>
		/// Read registers V0 through Vx from memory starting at location I 
		/// </summary>
		Ld_F_65
    }
}
