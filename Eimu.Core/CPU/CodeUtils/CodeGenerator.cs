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
using System.Collections;
using System.Reflection.Emit;
using System.Reflection;

namespace Eimu.Core.CPU.CodeUtils
{
    public sealed class CodeGenerator
    {
        private Memory m_Memory;
        private int m_PC = 0x200;
        private Stack<ushort> m_Stack;
        private Dictionary<int, DynamicMethod> m_GeneratedMethodLookup;

        public CodeGenerator(Memory memory)
        {
            m_GeneratedMethodLookup = new Dictionary<int, DynamicMethod>();
            this.m_Memory = memory;
            m_Stack = new Stack<ushort>(12);
        }

        public MethodInfo GenerateProgram()
        {
            int entryPoint = m_PC;
            GenerateMethod(entryPoint);
            return GetGeneratedMethod(entryPoint).GetBaseDefinition();
        }

        private void GenerateMethod(int address)
        {
            // Create a Dynamic Method
            DynamicMethod meth = new DynamicMethod("Func_" + address.ToString("x"), typeof(void), null);

            // Add it the generated method collection
            m_GeneratedMethodLookup.Add(m_PC, meth);

            while (m_PC < m_Memory.Size)
            {
                // Get the current opcode and incrment PC
                byte b1 = m_Memory[m_PC];
                byte b2 = m_Memory[m_PC + 1];
                m_PC += 2;
                ChipInstruction inst = new ChipInstruction((ushort)((ushort)b1 << 8 | b2));
                ChipOpcodes opcode = Disassembler.DecodeInstruction(inst);


                // Emit the ret code
                if (opcode == ChipOpcodes.Ret)
                {
                    EmitFunctionReturn(meth.GetILGenerator());
                    m_PC = (int)m_Stack.Pop(); 
                    return;
                }
                else if (opcode == ChipOpcodes.Call) // Emit the call function code
                {
                    m_Stack.Push((ushort)m_PC);
                    m_PC = (int)inst.NNN;
                    int functionAddress = m_PC;

                    if (!GeneratedMethodExists(functionAddress))
                        GenerateMethod(functionAddress);

                    EmitFunctionCall(meth.GetILGenerator(), GetGeneratedMethod(functionAddress).GetBaseDefinition());
                }
                else
                {
                    continue;
                }
            }
        }

        private bool GeneratedMethodExists(int address)
        {
            DynamicMethod meth;
            return m_GeneratedMethodLookup.TryGetValue(address, out meth);
        }

        private DynamicMethod GetGeneratedMethod(int address)
        {
            DynamicMethod meth;
            m_GeneratedMethodLookup.TryGetValue(address, out meth);
            return meth;
        }

        private void EmitFunctionCall(ILGenerator emitter, MethodInfo meth)
        {
            emitter.EmitCall(OpCodes.Call, meth, null);
            emitter.Emit(OpCodes.Nop);
        }

        private void EmitFunctionReturn(ILGenerator emitter)
        {
            emitter.Emit(OpCodes.Ret);
        }
    }
}
