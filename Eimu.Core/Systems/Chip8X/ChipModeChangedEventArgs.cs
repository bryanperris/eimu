using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipModeChangedEventArgs : EventArgs
    {
        private ChipMode m_Mode;

        public ChipModeChangedEventArgs(ChipMode mode)
        {
            m_Mode = mode;
        }

        public ChipMode Mode
        {
            get { return m_Mode; }
        }
    }
}
