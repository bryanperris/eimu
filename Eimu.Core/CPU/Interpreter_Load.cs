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
        [OpcodeTag(ChipOpcodes.Ld_6)]
        void Load_6(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = inst.KK;
        }

        [OpcodeTag(ChipOpcodes.Ld_8)]
        void Load_8(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = this.m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Ld_A)]
        void Load_A(ChipInstruction inst)
        {
            this.m_IReg = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Ld_F_07)]
        void Load_F07(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = (byte)this.m_DT;
        }

        [OpcodeTag(ChipOpcodes.Ld_F_0A)]
        void Load_F0A(ChipInstruction inst)
        {
            WaitForKey();
            m_VRegs[inst.X] = m_LastKey;
        }

        [OpcodeTag(ChipOpcodes.Ld_DT)]
        void Load_DT(ChipInstruction inst)
        {
            SetDelayTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Ld_ST)]
        void Load_ST(ChipInstruction inst)
        {
            SetSoundTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_29)]
        void Load_F29(ChipInstruction inst)
        {
            m_IReg = (ushort)(m_VRegs[inst.X] * VirtualMachine.FONT_SIZE);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_33)]
        void Load_F33(ChipInstruction inst)
        {
            byte val = m_VRegs[inst.X];
            m_Memory[m_IReg] = (byte)(val / 100);
            m_Memory[m_IReg + 1] = (byte)((val % 100) / 10);
            m_Memory[m_IReg + 2] = (byte)((val % 100) % 10);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_55)]
        void Load_F55(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                m_Memory[m_IReg + i] = m_VRegs[i];
            }
        }

        [OpcodeTag(ChipOpcodes.Ld_F_65)]
        void Load_F65(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
               m_VRegs[i] = m_Memory[m_IReg + i];
            }
        }

        [OpcodeTag(ChipOpcodes.Ld_F_75)]
        void Load_F75(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_ERegs[i] = m_VRegs[i];
        }

        [OpcodeTag(ChipOpcodes.Ld_F_85)]
        void Load_F85(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_VRegs[i] = m_ERegs[i];
        }
    }
}