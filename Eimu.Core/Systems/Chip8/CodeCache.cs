using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8
{
    public class CodeCache
    {
        private Dictionary<int, CodeBlock> m_Blocks;

        public CodeCache()
        {
            m_Blocks = new Dictionary<int, CodeBlock>();
        }

        public void CacheBlock(CodeBlock block)
        {
            m_Blocks.Add(block.StartAddress, block);
        }

        public CodeBlock GetCodeBlock(int address)
        {
            CodeBlock block;

            if (m_Blocks.TryGetValue(address, out block))
            {
                return block;
            }
            else
            {
                return null;
            }
        }
    }
}
