using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.SChip8
{
    public class CodeBlock
    {
        private int m_Address;
        private int m_Size;
        private List<ChipInstruction> m_Code;
        private bool m_Complete;

        public CodeBlock(int address)
        {
            m_Address = address;
            m_Size = 0;
            m_Code = new List<ChipInstruction>();
            m_Complete = false;
        }

        public void AddInstruction(ChipInstruction inst)
        {
            m_Size++;
            m_Code.Add(inst);
        }

        public int StartAddress
        {
            get { return this.m_Address; }
        }

        public int Size
        {
            get { return this.m_Size; }
        }

        public bool Complete
        {
            get { return m_Complete; }
            set { this.m_Complete = false; }
        }

        public List<ChipInstruction> Code
        {
            get { return this.m_Code; }
        }
    }
}
