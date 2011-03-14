using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipModeChangedEventArgs : EventArgs
    {
        private bool m_Enabled;

        public ChipModeChangedEventArgs(bool enabled)
        {
            m_Enabled = enabled;
        }

        public bool Enabled
        {
            get { return m_Enabled; }
        }
    }
}
