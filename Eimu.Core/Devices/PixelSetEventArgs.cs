using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Devices
{
    public class PixelSetEventArgs : EventArgs
    {
        int m_X;
        int m_Y;

        public PixelSetEventArgs(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        public int X
        {
            get { return this.m_X; }
        }

        public int Y
        {
            get { return this.m_Y; }
        }
    }
}
