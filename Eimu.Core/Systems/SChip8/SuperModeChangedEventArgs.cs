using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.SChip8
{
    public sealed class SuperModeChangedEventArgs : EventArgs
    {
        private bool m_Enabled;

        public SuperModeChangedEventArgs(bool enabled)
        {
            m_Enabled = enabled;
        }

        public bool Enabled
        {
            get { return m_Enabled; }
        }
    }
}
