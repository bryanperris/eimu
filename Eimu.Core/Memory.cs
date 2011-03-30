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
        private int m_CurrentAddressOffset;

        public Memory()
        {
            Reset();
        }

        public virtual void WriteByte(int address, byte value)
        {
            FindPage(address).WriteByte(address, value);
        }

        public virtual byte ReadByte(int address)
        {
            return FindPage(address).ReadByte(address);
        }

        public virtual void Reset()
        {
            m_MemBound = 0;
            m_CurrentAddressOffset = 0;
            m_Pages = new Dictionary<int, MemoryPage>();
            m_CurrentPage = null;
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

        private MemoryPage FindPage(int address)
        {
            if (m_CurrentPage != null)
            {
                if (address >= m_CurrentAddressOffset && address <= m_CurrentPage.Size)
                {
                    return m_CurrentPage;
                }
            }

            MemoryPage page;

            foreach (int a in m_Pages.Keys)
            {
                if (m_Pages.TryGetValue(a, out page))
                {
                    if (address >= a && address <= page.Size)
                    {
                        m_CurrentPage = page;
                        m_CurrentAddressOffset = a;
                        return page;
                    }
                }
            }

            return null;
        }

        protected void AddPage(MemoryPage page)
        {
            m_Pages.Add(m_MemBound, page);
            m_MemBound += (page.Size - 1);
        }

        protected void ClearPages()
        {
            m_Pages.Clear();
        }
    }
}
