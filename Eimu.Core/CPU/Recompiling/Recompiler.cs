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
using System.Reflection;
using System.Reflection.Emit;
using Eimu.Core.Devices;

namespace Eimu.Core.CPU.Recompiling
{
    public sealed partial class Recompiler : Processor
    {
        private ILGenerator m_Emitter;
        string progName = "ChipProgram";
        DynamicMethod program;
        OpcodeCallTable m_Table;
        

        public Recompiler()
            : base()
        {
  
            program = new DynamicMethod(progName, MethodAttributes.Private, CallingConventions.Standard, typeof(void), null, this.GetType(), false);
            m_Emitter = program.GetILGenerator();
        }
        
        private void GenerateChipProgram()
        {
            byte[] mem = this.m_Memory.MemoryBuffer;
			
			for (int i = 0; i < mem.Length; i+=4)
			{
				ChipInstruction inst = new ChipInstruction((ushort)((mem[i] << 8) | mem[i+2]));
				ChipOpcodes opcode = Disassembler.DecodeInstruction(inst);

				
			}
        }

        public override void Step()
        {
            throw new NotImplementedException();
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            GenerateChipProgram();
            program.Invoke(this, new object[0]);
        }
		
        [OpcodeTag(ChipOpcodes.Ret)]
		private void Ret()
		{
			//m_Emitter.Emit(OpCodes.
		}
    }
}
