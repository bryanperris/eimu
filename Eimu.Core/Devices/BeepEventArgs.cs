using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Devices
{
    public class BeepEventArgs : EventArgs
    {
        int m_Duration;

        public BeepEventArgs(int duration)
        {
            this.m_Duration = duration;
        }

        public int Duration
        {
            get { return this.m_Duration; }
        }
    }
}
