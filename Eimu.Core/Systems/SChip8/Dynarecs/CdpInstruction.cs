using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.SChip8.Dynarecs
{
    public sealed class CdpInstruction
    {
        private byte m_Data;

        public CdpInstruction(byte data)
        {
            m_Data = data;
        }

        public byte Hi
        {
            get { return (byte)((m_Data & 0xF0) >> 4); }
        }

        public byte Low
        {
            get { return (byte)(m_Data & 0x0F); }
        }

        public byte Data
        {
            get { return this.m_Data; }
        }
    }
}
