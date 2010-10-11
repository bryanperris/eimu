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
    public class OpLookupTable
    {
        private Dictionary<ChipOpcodes, OpcodeHandler> m_MethodCallTable;

        public OpLookupTable()
        {
            m_MethodCallTable = new Dictionary<ChipOpcodes, OpcodeHandler>();
        }

        public void LoadMethods(Type type, object sender)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (MethodInfo info in infos)
            {
                object[] attrs = info.GetCustomAttributes(typeof(OpcodeTag), false);

                if (attrs.Length > 0)
                {
                    OpcodeTag tag = ((OpcodeTag)attrs[0]);
                    this.m_MethodCallTable.Add(tag.Opcode, (OpcodeHandler)Delegate.CreateDelegate(typeof(OpcodeHandler), sender, info));
                }
            }
        }

        public void CallMethod(object sender, ChipOpcodes opcode, ChipInstruction instruction)
        {
            GetMethod(opcode).Invoke(instruction);
        }

        public OpcodeHandler GetMethod(ChipOpcodes opcode)
        {
            OpcodeHandler handler;
            this.m_MethodCallTable.TryGetValue(opcode, out handler);
            return handler;
        }
    }
}
