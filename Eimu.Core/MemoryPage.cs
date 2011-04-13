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
            m_Memory = new byte[size ];
        }

        public virtual byte ReadByte(int address)
        {
            if (address >= m_Memory.Length)
                return 0;

            return m_Memory[address];
        }

        public virtual void WriteByte(int address, byte value)
        {
            if (address >= m_Memory.Length)
                return;

            m_Memory[address] = value;
        }

        public virtual int Size
        {
            get { return m_Memory.Length; }
        }

        public virtual void Clear()
        {
            Array.Clear(m_Memory, 0, m_Memory.Length);
        }
    }
}
