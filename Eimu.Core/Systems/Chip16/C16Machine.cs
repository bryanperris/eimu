using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip16
{
    public sealed class C16Machine
    {
        public const int MemoryMaxSize = 0xFFFF;
        public const int StackMaxSize = 32;
        public const int MemoryMapTextOffset = 0x0;
        public const int MemoryMapStackOffset = 0xFDF0;
        public const int MemoryMapIOOffset = 0xFFF0;

        private C16Machine()
        {

        }
    }
}
