using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core.Systems.Chip8X.Interfaces;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class VideoPage : MemoryPage
    {
        VideoInterface m_VideoInterface;

        public VideoPage(VideoInterface videoInterface)
            : base(0)
        {
            m_VideoInterface = videoInterface;
        }

        public override void Clear()
        {
            m_VideoInterface.ClearPixels();
        }

        public override byte ReadByte(int address)
        {
            // Get the index of byte equalivent spot
            int index = address * 8;

            byte value = 0;

            for (int i = 0; i < 8; i++)
            {
                value <<= 1;

                if (m_VideoInterface.Pixels[index + i])
                    value ^= 1;

            }

            return value;
        }

        public override void WriteByte(int address, byte value)
        {
            // Get the index of byte equalivent spot
            int index = address * 8;

            // Loop backwards writting the bits
            for (int i = 0; i < 8; i++)
                m_VideoInterface.Pixels[index + i] = ((value << i) & 0x80) == 0 ? false : true;
        }

        public override int Size
        {
            get
            {
                return m_VideoInterface.Pixels.Length / 8;
            }
        }
    }
}
