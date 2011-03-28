using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class VideoFrameUpdate : EventArgs
    {
        private int m_BufferSizeWidth;
        private int m_BufferSizeHeight;
        private bool[] m_Buffer;

        public VideoFrameUpdate(int sizeX, int sizeY, bool[] data)
        {
            m_BufferSizeWidth = sizeX;
            m_BufferSizeHeight = sizeY;
            m_Buffer = new bool[data.Length];
            Array.Copy(data, m_Buffer, m_Buffer.Length);
        }

        public  int GetBufferPosition(int x, int y)
        {
            int val = (y * m_BufferSizeWidth) + x;

            if (val < m_Buffer.Length)
                return (val);
            else
                return 0;
        }

        public int FrameWidth
        {
            get { return this.m_BufferSizeWidth; }
        }

        public int FrameHeight
        {
            get { return this.m_BufferSizeHeight; }
        }

        public bool[] FrameData
        {
            get { return this.m_Buffer; }
        }
    }
}
