using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class WorkareaPage : MemoryPage
    {
        CodeEngine m_Engine;

        public WorkareaPage(CodeEngine engine) : base(0)
        {
            m_Engine = engine;
        }

        public override void Clear()
        {
            base.Clear();
        }

        public override byte ReadByte(int address)
        {
            int val = (address & 0x00FF);

            if (val >= 0xF0)
            {
                return m_Engine.VRegisters[val & 0xF];
            }
            else
            {
                return 0;
            }
        }

        public override void WriteByte(int address, byte value)
        {
            int val = (address & 0x00FF);

            if (val >= 0xF0)
            {
                m_Engine.VRegisters[val & 0xF] = value;
            }
        }

        public override int Size
        {
            get
            {
                return 0x2F;
            }
        }
    }
}
