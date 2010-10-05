/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

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
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

using Eimu.Core.Devices;

namespace Eimu.Core.CPU
{
    [Serializable]
    public abstract class Processor : IDevice
    {
        public const int STACK_SIZE = 12;
        public const int DELAY_TIME_RATE = 60;
        public const int SOUND_TIMER_RATE = 60;

        protected Memory m_Memory;
        protected bool m_Paused = false;
        protected Stack<ushort> m_Stack;
        private BackgroundWorker m_Worker;
        private EventWaitHandle m_CPUWait;

        // Registers
        protected readonly byte[] m_VRegs = new byte[16];
        protected int m_ProgramCounter;
        protected ushort m_IReg;
        protected byte m_DT;
        protected byte m_ST;

        // CPU timers
        private Thread m_DelayTimer;
        private Thread m_SoundTimer;
        
        // Device Callbacks
        public event EventHandler OnScreenClear;
        public event EventHandler<PixelSetEventArgs> OnPixelSet;
        public event EventHandler OnBeep;

        public Processor()
        {
            ClearRegisters();

            this.m_Worker = new BackgroundWorker();
            this.m_Worker.WorkerSupportsCancellation = true;
            this.m_Worker.DoWork += new DoWorkEventHandler(DoExecution);

            m_Stack = new Stack<ushort>(STACK_SIZE);
        }

        protected void ClearRegisters()
        {
            m_ProgramCounter = 0;
            m_IReg = 0;
            m_StackPointer = 0;
        }

        public void SetMemory(Memory memory)
        {
            this.m_Memory = memory;
        }

        private void DoExecution(object sender, DoWorkEventArgs e)
        {
            while (this.m_ProgramCounter <= this.m_Memory.Size)
            {
                if (e.Cancel)
                    break;

                Step();
            }
        }

        public abstract void Step();

        public void StartExecution()
        {
            this.m_Worker.RunWorkerAsync();
        }

        private void DelayThread()
        {
            // The timer thread

            while (m_DT > 0)
            {
                Interlocked.Decrement(m_DT);
            }
            
            m_CPUWait.Set();
        }

        private void SoundThread()
        {
            while (m_ST > 0)
            {
                Interlocked.Decrement(ref m_ST);
            }

            if (OnBeep != null)
                OnBeep(this, new EventArgs());
        }

        public void SetDelayTimer(byte value)
        {
            m_CPUWait.WaitOne();
            m_DT = value;
            m_DelayTimer = new Thread(new ThreadStart(DelayThread));
            m_DelayTimer.Start();
        }

        public void SetSoundTimer(byte value)
        {
            m_ST = value;
            m_SoundTimer = new Thread(new ThreadStart(SoundThread));
            m_SoundTimer.Start();
        }

        public virtual void SetPauseState(bool paused)
        {
            this.m_Paused = paused;
        }

        public virtual void Shutdown()
        {
            if (this.m_Worker.IsBusy)
                this.m_Worker.CancelAsync();
        }

        public virtual void Initialize()
        {
        }

        public void IncrementPC()
        {
            m_ProgramCounter += 2;
        }

        public int ProgramCounter
        {
            get { return this.m_ProgramCounter; }
            set { this.m_ProgramCounter = value; }
        }

        public void SetCollision()
        {
            m_VRegs[0xF] = 1;
        }

        protected void Beep()
        {
            if (OnBeep != null)
                OnBeep(this, new EventArgs());
        }

        protected void PixelSet(int x, int y)
        {
            if (OnPixelSet != null)
                OnPixelSet(this, new PixelSetEventArgs(x, y));
        }

        protected void ScreenClear()
        {
            if (OnScreenClear != null)
                OnScreenClear(this, new EventArgs());
        }
    }
}
