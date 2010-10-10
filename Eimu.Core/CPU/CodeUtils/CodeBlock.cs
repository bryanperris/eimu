using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.CPU.CodeUtils
{
    public sealed class CodeBlock
    {
        private int m_BeginAddress;
        private int m_EndAddress;

        public CodeBlock()
        {
            m_BeginAddress = 0;
            m_EndAddress = 0;
        }

        public CodeBlock(int begin, int end)
        {
            m_BeginAddress = begin;
            m_EndAddress = end;
        }

        public int BeginAddress
        {
            get { return this.m_BeginAddress; }
            set { this.m_BeginAddress = value; }
        }

        public int EndAddress
        {
            get { return this.m_EndAddress; }
            set { this.m_EndAddress = value; }
        }
    }
}
