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
    [Serializable]
    public abstract class Memory
    {
        private int m_MemBound;
        private Dictionary<int, MemoryPage> m_Pages;
        private MemoryPage m_CurrentPage;
        private int m_CurrentAddressOffset = 0;
        private int m_CurrentAddressBound = 0;
        private VirtualMachine m_ParentMachine;
        private bool m_Debug1802Access;

        public Memory(VirtualMachine machine)
        {
            m_ParentMachine = machine;
            m_MemBound = 0;
            m_CurrentAddressOffset = 0;
            m_CurrentAddressBound = 0;
            m_Pages = new Dictionary<int, MemoryPage>();
            m_CurrentPage = null;
            AllocatePages();
        }

        protected abstract void AllocatePages();

        public virtual void WriteByte(int address, byte value)
        {
            lock (this)
            {
                if (m_Debug1802Access)
                    Console.WriteLine("CDP1802 Address Write " + address.ToString("X4") + " " + value.ToString("X2"));

                FindPage(address).WriteByte(address - m_CurrentAddressOffset, value);
            }
        }

        public virtual byte ReadByte(int address)
        {
            lock (this)
            {
                if (m_Debug1802Access)
                    Console.WriteLine("CDP1802 Address Read " + address.ToString("X4"));

                return FindPage(address).ReadByte(address - m_CurrentAddressOffset);
            }
        }

        public void Reset()
        {
            foreach (MemoryPage page in m_Pages.Values)
            {
                page.Clear();
            }
        }

        public virtual int Size
        {
            get { return m_MemBound; }
        }

        public virtual byte this[int index]
        {
            get
            {
                return ReadByte(index);
            }
            set
            {
                WriteByte(index, value);
            }
        }

        public MemoryPage FindPage(int address)
        {
            // If the address isn't out of bounds, use the same page
            if (address >= m_CurrentAddressOffset && address < m_CurrentAddressBound)
            {
                return m_CurrentPage;
            }

            // Else we need to figure out the new page to access
            MemoryPage page = null;

            // Check each offset address and see where address falls in range with
            foreach (int offset in m_Pages.Keys)
            {
                // if address is >= of that key, it is a possible page
                if (address >= offset)
                {
                    // Get the reference of the page
                    if (m_Pages.TryGetValue(offset, out page))
                    {
                        // if the address within bounds of the page, then get it, else keep searching
                        if (address < (offset + page.Size))
                        {
                            m_CurrentPage = page;
                            m_CurrentAddressBound = offset + page.Size;
                            m_CurrentAddressOffset = offset;
                            return page;
                        }
                    }
                }
            }

            return page;
        }

        protected void AddPage(MemoryPage page)
        {
            m_Pages.Add(m_MemBound, page);
            m_MemBound += page.Size;
        }

        protected void ClearPages()
        {
            m_Pages.Clear();
        }

        public VirtualMachine ParentMachine
        {
            get { return this.m_ParentMachine; }
        }

        public bool IsCDP1802AccessDebugEnabled
        {
            get { return m_Debug1802Access; }
            set { m_Debug1802Access = value; }
        }
    }
}
