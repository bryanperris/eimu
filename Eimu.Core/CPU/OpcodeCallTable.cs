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
        private Dictionary<ChipOpcodes, InstructionCall> m_MethodCallTable;

        public OpcodeCallTable()
        {
            m_MethodCallTable = new Dictionary<ChipOpcodes, InstructionCall>();
        }

        public void LoadMethods(Type type)
        {
            MethodInfo[] infos = type.GetMethods(BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Static);

            foreach (MethodInfo info in infos)
            {
                object[] attrs = info.GetCustomAttributes(typeof(OpcodeTag), false);

                if (attrs != null)
                {
                    OpcodeTag tag = ((OpcodeTag)attrs[0]);
                    this.m_MethodCallTable.Add(tag.Opcode, (InstructionCall)Delegate.CreateDelegate(typeof(InstructionCall), info));
                }
            }
        }

        public bool CallMethod(ChipOpcodes opcode, ChipInstruction instruction)
        {
            InstructionCall call;

            if (this.m_MethodCallTable.TryGetValue(opcode, out call))
            {
                call(instruction);
                return true;
            }

            return false;
        }
    }
}
