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
    public sealed class MemoryEnumerator : IEnumerator
    {
        private byte[] m_Buffer;
        private int m_Position = -1;

        public MemoryEnumerator(byte[] buffer)
        {
            m_Buffer = buffer;
        }

        #region IEnumerator Members

        public object Current
        {
            get
            {
                try
                {
                    return m_Buffer[m_Position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public bool MoveNext()
        {
            m_Position++;
            return (m_Position < m_Buffer.Length);
        }

        public void Reset()
        {
            m_Position = -1;
        }

        #endregion
    }
}
