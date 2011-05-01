using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipMemory : Memory
    {
        public const int MEMORY_FONT_SIZE = 512;
        public const int MEMORY_CHIP_SIZE = 4096;
        public const int MEMORY_STACK_SIZE =   48;
        public const int MEMORY_CHIPCORE_SIZE = 48;
        public const int MEMORY_VIDEO_SIZE = 256;
        private int m_FontOffset;
        private int m_ChipOffset;
        private int m_StackOffset;
        private int m_ChipCoreOffset;
        private int m_VideoOffset;

        public ChipMemory(Chip8XMachine machine) : base((VirtualMachine)machine)
        {
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

        public int ChipCorePointer
        {
            get { return m_ChipCoreOffset; }
        }

        public int StackPointer
        {
            get { return m_StackOffset; }
        }

        protected override void AllocatePages()
        {
            m_FontOffset = Size;
            AddPage(new MemoryPage(MEMORY_FONT_SIZE));

            m_ChipOffset = Size;
            AddPage(new MemoryPage(MEMORY_CHIP_SIZE));

            m_StackOffset = Size;
            AddPage(new MemoryPage(MEMORY_STACK_SIZE));

            m_ChipCoreOffset = Size;
            AddPage(new ChipCorePage(((Chip8XMachine)ParentMachine).ProcessorCore));

            m_VideoOffset = 0x1300;
            AddManualPage(m_VideoOffset, new VideoPage(((Chip8XMachine)ParentMachine).VideoInterface));
            
        }
    }
}
