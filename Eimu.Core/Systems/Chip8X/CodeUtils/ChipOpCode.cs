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

namespace Eimu.Core.Systems.Chip8X.CodeUtils
{
	public enum ChipOpCode : ushort
	{

		Unknown = 0, // Uknown Opcode 
		Sys,         // Call 1802 subroutine at address NNN
		Clr,         // Clear the display pixels
		Ret,         // Goto address at top of stack, and then pop it
		Jp_1,        // Jump to address NNN
		Call,        // Call Subroutine at NNN, push next address on top of stack
		Se_3,        // If Vx == Kk, skip next instruction
		Sne_4,       // If Vx != Kk, skip next instruction
		Se_5,        // If register Vx = Vy, if equal, increments PC by 2 
		Ld_6,        // Load value Kk into register Vx
		Add_7,       // Vx += kk, uses carry
		Ld_8,        // Stores Vy value in register Vx 
		Or,          // Vx |= Vy 
		And,         // Vx &= Vy
		Xor,         // Vx ^= Vy 
		Add_8,       // Vx += Vy, uses carry flag
		Sub,         // Vx -= Vy, Set VF = Not borrow if Vx > Vy 
		Shr,         // Vx >>= 1 
		Subn,        // Vx = Vy - Vx, set VF = Not Borrow, if Yv > Vx
		Shl,         // Vx <<= 1 
		Sne_9,       // Skip next if Vx != Yy, if not equal, PC is increased by 2
		Ld_A,        // Set I = NNN 
		Jp_B,        // Jump to location NNN + V0, PC is set to (nnn + V0) 
		Rnd,         // Random value of 255 than ANDed by Kk, results stored in Vx. 
		Drw,         // Display n-byte sprite starting at memory location I at (Vx, Vy), set VF as collision flag
		Skp,         // Skip next instruction if the key with the value of Vx is pressed, if true, then increment the PC by 2.
		Sknp,        // Skip the next instruction if the key with the value of Vx is not pressed, if true, then increment the PC by 2. 
		Ld_F_07,     // Set Vx = delay timer value 
		Ld_F_0A,     // Wait for a key press, store key value in Vx, this pauses execution 
		Ld_DT,       // Set Delay Timer = Vx 
		Ld_ST,       // Set sound timer = Vx 
		Add_F,       // I += Vx, uses carry flag
		Ld_F_29,     // Set I to location of sprite for digit Vx 
		Ld_F_33,     // Store BCD representation of Vx in memory locations I, I+1, I+2
		Ld_F_55,     // Store registers V0 through Vx in memory starting at location I 
		Ld_F_65,     // Read registers V0 through Vx from memory starting at location I 
		Ld_F_75,     // Loads all V regsters to HP84 RPL Flags
		Ld_F_85,     // Load all HP84 RPL flags to V registers
		Ld_F_30,     // Points I to 10-byte sprite for the digit in VX (0..9)
		exit,        // Exit from S-CHIP environment
		extOff,      // Turn off extended mode
		extOn,       // Turn on extended mode
		scrollN,     // Scroll display N lines down
		scrollR,     // Scroll display 4 pixels right
		scrollL,     // Scroll display 4 pixels left
	}
}
