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
    public abstract class VirtualMachine : IDisposable
    {
        private RunState m_State;
        private Memory m_Memory;
        private Stream m_MediaSource;
        private bool m_Booted = false;
        public event RunStateChangedEvent RunStateChanged;
        public event EventHandler MachineEnded;

        
        protected abstract bool Boot();

        public void SetMediaSource(Stream source)
        {
            this.m_MediaSource = source;
        }

        protected Stream MediaSource
        {
            get { return this.m_MediaSource;}
        }

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

        private void SetRunState(RunState state)
        {
            m_State = state;

            if (m_State == RunState.Running && !m_Booted)
            {
                bool successful = Boot();

                if (!successful)
                {
                    m_Booted = false;
                    m_State = RunState.Stopped;
                    Console.WriteLine("Booting failed!");
                    Dispose();
                    return;
                }
                else
                {
                    m_Booted = true;
                }
            }

            if (m_State == RunState.Stopped)
            {
                m_Booted = false;
            }
                
            if (RunStateChanged != null)
                RunStateChanged(this, m_State);

            OnMachineState(m_State);
        }

        protected abstract void OnMachineState(RunState state);

        public RunState CurrentRunState
        {
            get { return this.m_State;}
        }

        public Memory SystemMemory
        {
            get { return this.m_Memory;}
            set
            {
                this.m_Memory = value;
            }
        }

        public void Dispose()
        {
            if (m_State == RunState.Paused || m_State == RunState.Running)
            {
               m_State = RunState.Stopped;
               OnMachineState(RunState.Stopped);
            }

            m_Memory = null;
            m_MediaSource = null;

            if (MachineEnded != null)
                MachineEnded(this, new EventArgs());

            GC.SuppressFinalize(this);
        }
    }
}
