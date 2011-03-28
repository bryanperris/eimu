using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    public sealed class RunStateChangedArgs : EventArgs
    {
        private RunState m_State;

        public RunStateChangedArgs(RunState state)
        {
            m_State = state;
        }

        public RunState State
        {
            get { return this.m_State; }
        }
    }
}
