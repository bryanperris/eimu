using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipCorePage : MemoryPage
    {
        CodeEngine m_Engine;

        public ChipCorePage(CodeEngine engine) : base(0)
        {
            m_Engine = engine;
        }

        public override void Clear()
        {
            Array.Clear(m_Engine.VRegisters, 0, m_Engine.VRegisters.Length);
        }

        public override byte ReadByte(int address)
        {
            if (address >= 0x10F0)
                return m_Engine.VRegisters[address - 0x10F0];
            else
                return 0;
        }

        public override void WriteByte(int address, byte value)
        {
            if (address >= 0x10F0)
                m_Engine.VRegisters[address - 0x10F0] = value;
        }

        public override int Size
        {
            get
            {
                return 47; // 47 size?
            }
        }
    }
}
