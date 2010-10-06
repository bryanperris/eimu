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

namespace Eimu.Core.CPU
{
    public delegate void InstructionCall(ChipInstruction instruction);

    public class OpcodeCallTable
    {
        private Dictionary<ChipOpcodes, MethodInfo> m_MethodCallTable;

        public OpcodeCallTable()
        {
            m_MethodCallTable = new Dictionary<ChipOpcodes, MethodInfo>();
        }

        public void LoadMethods(Type type)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Instance);

            foreach (MethodInfo info in infos)
            {
                object[] attrs = info.GetCustomAttributes(typeof(OpcodeTag), false);

                if (attrs.Length > 0)
                {
                    OpcodeTag tag = ((OpcodeTag)attrs[0]);
                    this.m_MethodCallTable.Add(tag.Opcode, info);
                }
            }
        }

        public bool CallMethod(object sender, ChipOpcodes opcode, ChipInstruction instruction)
        {
            MethodInfo call;

            if (this.m_MethodCallTable.TryGetValue(opcode, out call))
            {
                call.Invoke(sender, new object[] { instruction });
                return true;
            }

            return false;
        }
    }
}
