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

namespace Eimu.Core
{
    public sealed class Memory : IEnumerable
    {
        public const int MEMORY_SIZE = 4096;
        public const int MEMORY_OFFSET = 0x200;

        private byte[] m_Memory;

        public Memory()
        {
            this.m_Memory = new byte[MEMORY_SIZE];
        }

        public byte GetValue(int address)
        {
            return m_Memory[address];
        }

        public void SetValue(int address, byte value)
        {
            m_Memory[address] = value;
        }

        public int Size
        {
            get { return this.m_Memory.Length; }
        }

        public override bool Equals(object obj)
        {
            return ((Memory)obj).m_Memory.SequenceEqual(this.m_Memory);
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new MemoryEnumerator(this.m_Memory);
        }

        #endregion

        public byte this[int index]
        {
            get
            {
                return m_Memory[index];
            }
            set
            {
                m_Memory[index] = value;
            }
        }
    }
}
