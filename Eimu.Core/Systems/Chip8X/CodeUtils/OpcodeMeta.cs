using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X.CodeUtils
{
    public sealed class OpcodeMeta : Attribute
    {
        private int m_OpNum;
        private string m_DebugString;
    }
}
