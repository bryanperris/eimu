using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    [Serializable]
    public class MemoryPage
    {
        private byte[] m_Memory;

        public MemoryPage(int size)
        {
            m_Memory = new byte[size];
        }

        public virtual byte ReadByte(int address)
        {
            return m_Memory[address];
        }

        public virtual void WriteByte(int address, byte value)
        {
            m_Memory[address] = value;
        }

        public virtual int Size
        {
            get { return m_Memory.Length; }
        }
    }
}
