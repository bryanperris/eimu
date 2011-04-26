using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X.CodeUtils
{
    public sealed class CodeAnalyzer
    {
        private int m_CodeSize;
        private int m_CodeOffset;
        private Memory m_Memory;
        private CodeEngine m_Engine;
        private Dictionary<int, ChipInstruction[]> m_CodeBlocks;

        public CodeAnalyzer(CodeEngine engine)
        {
            m_Engine = engine;
            m_Memory = engine.Memory;
        }
    }
}
