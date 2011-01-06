using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip16
{
    public sealed class C16Machine
    {
        public const int MEMORY_SIZE = 0xFFFF;
        public const int STACK_SIZE = 32;
        public const int MAP_TEXT_BEGIN = 0x0;
        public const int MAP_STACK_BEGIN = 0xFDF0;
        public const int MAP_IO_BEGIN = 0xFFF0;
    }
}
