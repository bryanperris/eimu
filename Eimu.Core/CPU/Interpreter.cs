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
using System.Threading;
using System.ComponentModel;

using Eimu.Core.Devices;

namespace Eimu.Core.CPU
{
    public sealed partial class Interpreter : Processor
    {
        OpcodeCallTable table;

        public Interpreter() : base()
        {
            table = new OpcodeCallTable();
            table.LoadMethods(this.GetType());
        }

        public override void Step()
        {
            byte rbyte1 = this.m_Memory.GetValue(m_ProgramCounter);
            Interlocked.Increment(ref m_ProgramCounter);
            byte rbyte2 = this.m_Memory.GetValue(m_ProgramCounter);
            Interlocked.Increment(ref m_ProgramCounter);

            ChipInstruction inst = new ChipInstruction((ushort)((ushort)rbyte1 << 8 | rbyte2));
            ChipOpcodes opcode = Disassembler.DecodeInstruction(inst);
            //Console.WriteLine("Opcode: " + opcode.ToString());
            table.CallMethod(this, opcode, inst);
        }


    }
}
