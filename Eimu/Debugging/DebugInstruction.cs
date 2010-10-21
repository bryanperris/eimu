using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core.CPU;

namespace Eimu.Debugging
{
    public class DebugInstruction
    {
        private int m_Address;
        private ChipOpcodes m_Op;

        public DebugInstruction(int address, ChipOpcodes opcode)
        {
            this.m_Address = address;
            this.m_Op = opcode;
        }

        public override string ToString()
        {
            return m_Address.ToString() + ": " + m_Op.ToString();
        }
    }
}
