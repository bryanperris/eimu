using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Chip16
{
    public sealed class Memory
    {
        public const int MEMORY_SIZE = 0xFFFF;
        public const int STACK_SIZE = 32;

        public const int MAP_TEXT_BEGIN = 0x0;
        public const int MAP_STACK_BEGIN = 0xFDF0;
        public const int MAP_IO_BEGIN = 0xFFF0;

        private byte[] m_Memory;
    }
}
