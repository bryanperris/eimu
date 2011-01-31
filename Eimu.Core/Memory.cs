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
using System.Runtime.InteropServices;

namespace Eimu.Core
{
    [Serializable]
    public sealed class Memory
    {
        private byte[] m_Memory;

        public Memory(int size)
        {
            this.m_Memory = new byte[size];
        }

        public byte GetByte(int address)
        {
            if (address < m_Memory.Length)
                return m_Memory[address];
            else
                return 0;
        }

        public void SetByte(int address, byte value)
        {
            m_Memory[address] = value;
        }

        public void SetBytes(byte[] buffer, int offset, int size)
        {
            for (int i = 0; i < size; i++)
            {
                m_Memory[i + offset] = buffer[i];
            }
        }

        public void GetBytes(byte[] buffer, int offset, int size)
        {
            for (int i = 0; i < size; i++)
            {
                buffer[i] = m_Memory[i + offset];
            }
        }

        public int Size
        {
            get { return this.m_Memory.Length; }
        }

        public override bool Equals(object obj)
        {
            return ((Memory)obj).m_Memory.SequenceEqual(this.m_Memory);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public byte this[int index]
        {
            get { return m_Memory[index]; }
            set { m_Memory[index] = value; }
        }
    }
}
