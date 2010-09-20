﻿/*  
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
using System.ComponentModel;

namespace Eimu.Core.CPU
{
    public sealed partial class Interpreter : Processor
    {
        OpcodeCallTable table;

        public Interpreter()
        {
            table = new OpcodeCallTable();
            table.LoadMethods(this.GetType());
        }

        public override void Step()
        {
            throw new NotImplementedException();
        }

        protected override void Execute(object sender, DoWorkEventArgs e)
        {
            ChipInstruction inst;

            while (this.m_ProgCounter <= this.m_Memory.Size)
            {
                if (e.Cancel)
                    break;

                this.ProgramCounter &= 0x00000FFF;
                inst = new ChipInstruction(this.m_Memory.GetValue(this.ProgramCounter));
                ChipOpcodes opcode = Disassembler.DecodeInstruction(inst);
                this.ProgramCounter += 2;

                table.CallMethod(opcode, inst);

            }
        }
    }
}