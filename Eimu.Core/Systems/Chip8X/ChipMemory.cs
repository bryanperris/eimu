using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipMemory : Memory
    {
        public const int MEMORY_FONT_SIZE = 512;
        public const int MEMORY_CHIP_SIZE   =  4096;
        public const int MEMORY_STACK_SIZE =   48;
        public const int MEMORY_WORKAREA_SIZE = 255;
        public const int MEMORY_VIDEO_SIZE = 6144;
        private int m_FontOffset;
        private int m_ChipOffset;
        private int m_VideoOffset;

        public ChipMemory() : base()
        {
        }

        public override void Reset()
        {
            base.Reset();
            m_FontOffset = Size;
            AddPage(new MemoryPage(MEMORY_FONT_SIZE));
            m_ChipOffset = Size;
            AddPage(new MemoryPage(MEMORY_CHIP_SIZE));
            m_VideoOffset = Size;
            AddPage(new MemoryPage(MEMORY_VIDEO_SIZE));
        }

        public int FontPointer
        {
            get { return m_FontOffset; }
        }

        public int ChipPointer
        {
            get { return m_ChipOffset; }
        }

        public int VideoPointer
        {
            get { return m_VideoOffset; }
        }
    }
}
