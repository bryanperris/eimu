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
        [OpcodeTag(ChipOpcodes.Sys)]
        void Sys(ChipInstruction inst)
        {
            // Ignored Opcode
            //Console.WriteLine("Sys call: " + inst.NNN.ToString("x"));
        }

        [OpcodeTag(ChipOpcodes.Jp_1)]
        void Jump_1(ChipInstruction inst)
        {
            m_ProgramCounter = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Call)]
        void Call(ChipInstruction inst)
        {
            m_Stack.Push((ushort)m_ProgramCounter);
            m_ProgramCounter = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Se_3)]
        void Se_3(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Sne_4)]
        void Sne_4(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Se_5)]
        void Se_5(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Sne_9)]
        void Sne_9(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Jp_B)]
        void Jp_B(ChipInstruction inst)
        {
            m_ProgramCounter = (ushort)(inst.NNN + m_VRegs[0]);
        }

        [OpcodeTag(ChipOpcodes.Skp)]
        void Skp(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == m_LastKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key checked for: " + m_VRegs[inst.X].ToString());
            }
        }

        [OpcodeTag(ChipOpcodes.Sknp)]
        void Sknp(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != m_LastKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key not checked for: " + m_VRegs[inst.X].ToString());
            }
        }
    }
}