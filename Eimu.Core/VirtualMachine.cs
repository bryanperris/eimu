/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu
Dedicated to Monarch.

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Eimu.Core
{
    [Serializable]
    public abstract class VirtualMachine
    {
        private RunState m_State;
        private bool m_Booted = false;
        private IDebugger m_Debugger;

        // Events
        public event EventHandler<RunStateChangedArgs> RunningStateChanged;
        public event EventHandler MachineAborted;
        public event EventHandler MachineStarted;

        #region Abstract Methods

        protected abstract bool Boot();

        #endregion

        #region Machine API

        public void Run()
        {
            SetRunState(RunState.Running);
        }

        public void Pause()
        {
            SetRunState(RunState.Paused);
        }

        public void Stop()
        {
            SetRunState(RunState.Stopped);
        }

        public void Restart()
        {
            Stop();
            Thread.Sleep(100);
            Run();
        }

        public void AttachDebugger(IDebugger debugger)
        {
            m_Debugger = debugger;
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnStateChanged(RunState state)
        {
            if (RunningStateChanged != null)
                RunningStateChanged(this, new RunStateChangedArgs(state));
        }

        #endregion

        #region Private Methods

        private void SetRunState(RunState state)
        {
            m_State = state;

            if (state == RunState.Paused)
            {
                if (m_Debugger != null)
                    m_Debugger.Report();
            }

            if (m_State == RunState.Running && !m_Booted)
            {
                if (!Resources.LoadResources())
                {
                    m_Booted = false;
                    m_State = RunState.Stopped;
                    throw new VMException("Resources failed to load!");
                }

                if (!Boot())
                {
                    m_Booted = false;
                    m_State = RunState.Stopped;
                    throw new VMException("Machine has failed to boot!");
                }
                else
                {
                    m_Booted = true;

                    if (m_Debugger != null)
                    {
                        m_Debugger.StartDebugging(this);
                        m_Debugger.Report();
                    }
                }
            }

            if (m_State == RunState.Stopped)
            {
                m_Booted = false;
                if (m_Debugger != null)
                    m_Debugger.StopDebugging();
            }

            if (RunningStateChanged != null)
                RunningStateChanged(this, new RunStateChangedArgs(m_State));

            OnStateChanged(m_State);
        }

        #endregion

        #region Properties

        public RunState CurrentRunState
        {
            get { return m_State; }
        }

        public abstract ResourceManager Resources { get; }

        public abstract Memory SystemMemory { get; }

        public bool IsBooted
        {
            get { return m_Booted; }
        }

        public IDebugger AttachedDebugger
        {
            get { return m_Debugger; }
        }

        #endregion
    }
}
