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
        [OpcodeTag(ChipOpcodes.Add_7)]
        void Add_7(ChipInstruction inst)
        {
            m_VRegs[inst.X] += inst.KK;
        }

        [OpcodeTag(ChipOpcodes.Add_8)]
        void Add_8(ChipInstruction inst)
        {
            ushort val = (ushort)(m_VRegs[inst.X] + m_VRegs[inst.Y]);
            m_VRegs[0xF] = (byte)((val > 255) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(val & 0x00FF);
        }

        [OpcodeTag(ChipOpcodes.Add_F)]
        void Add_F(ChipInstruction inst)
        {
            if (((int)m_IReg + (int)m_VRegs[inst.X]) >= Memory.MEMORY_SIZE)
            {
                m_IReg = Memory.MEMORY_SIZE;
                m_VRegs[0xF] = 1;
            }
            else
                m_IReg += m_VRegs[inst.X];
        }

        [OpcodeTag(ChipOpcodes.Or)]
        void Or(ChipInstruction inst)
        {
            m_VRegs[inst.X] |= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.And)]
        void And(ChipInstruction inst)
        {
            m_VRegs[inst.X] &= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Xor)]
        void Xor(ChipInstruction inst)
        {
            m_VRegs[inst.X] ^= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Sub)]
        void Sub(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] >= m_VRegs[inst.Y]) ? 1 : 0);
            m_VRegs[inst.X] -= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Shr)]
        void Shr(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)(((m_VRegs[inst.X] & 1) == 1) ? 1 : 0);
            m_VRegs[inst.X] /= 2;
        }

        [OpcodeTag(ChipOpcodes.Subn)]
        void Subn(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.Y] >= m_VRegs[inst.X]) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(m_VRegs[inst.Y] - m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Shl)]
        void Shl(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] & 0x80) >> 7);
            m_VRegs[inst.X] *= 2;
        }

        [OpcodeTag(ChipOpcodes.Rnd)]
        void Rnd(ChipInstruction inst)
        {
            m_VRegs[inst.X] = (byte)(m_Rand.Next(255) & inst.KK);
        }
    }
}