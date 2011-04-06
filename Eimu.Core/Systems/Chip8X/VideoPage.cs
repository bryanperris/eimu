using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            int index = address * 8;
            byte value = 0;

            for (int i = index; i < (index + 7); i++)
            {
                value <<= 1;

                if (m_VideoInterface.Pixels[i])
                    value |= 1;

            }

            return value;
        }

        public override void WriteByte(int address, byte value)
        {
            int index = address * 8;

            for (int i = index; i < (index + 7); i++)
            {
                m_VideoInterface.Pixels[i] = ((value << (i - index)) & 0x80) == 1 ? true : false;
            }

            m_VideoInterface.RenderWait();
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
