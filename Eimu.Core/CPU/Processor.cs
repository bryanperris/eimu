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
using System.Linq;
using System.Text;
using System.Timers;
using Eimu.Core.Devices;
using System.ComponentModel;

namespace Eimu.Core.CPU
{
    public abstract class Processor
    {
        public const int STACK_SIZE = 100;
        protected readonly byte[] m_VRegs = new byte[16];
        protected ushort[] m_Stack = new ushort[STACK_SIZE];
        protected int m_ProgCounter;
        protected ushort m_IReg;
        protected int m_StackPointer;
        protected Timer m_DelayTimer;
        protected Timer m_SoundTimer;
        protected Memory m_Memory;
        protected bool m_Paused = false;
        private BackgroundWorker m_Worker;

        public Processor()
        {
            this.m_Worker = new BackgroundWorker();
            this.m_Worker.WorkerSupportsCancellation = true;
            this.m_Worker.DoWork += new DoWorkEventHandler(Execute);
            m_ProgCounter = 0;
            m_IReg = 0;
            m_StackPointer = 0;
            m_DelayTimer = new Timer();
            m_SoundTimer = new Timer();
        }

        public void SetMemory(Memory memory)
        {
            this.m_Memory = memory;
        }

        protected abstract void Execute(object sender, DoWorkEventArgs e);

        public void Run()
        {
            this.m_Worker.RunWorkerAsync();
        }

        public virtual void SetPauseState(bool paused)
        {
            this.m_Paused = paused;
        }

        public void Shutdown()
        {
            if (this.m_Worker.IsBusy)
                this.m_Worker.CancelAsync();
        }

        public void SetRegisterV(int number, byte value)
        {
            m_VRegs[number] = value;
        }

        public byte GetRegisterV(int number)
        {
            return m_VRegs[number];
        }

        public void StackPushValue(byte value)
        {
            m_Stack[m_StackPointer] = value;
            m_StackPointer++;
        }

        public ushort StackPopValue()
        {
            ushort val = m_Stack[m_StackPointer];
            m_Stack[m_StackPointer--] = 0;
            return val;
        }

        public void IncrementPC()
        {
            ++m_ProgCounter;
        }

        public abstract void Step();

        public int ProgramCounter
        {
            get { return this.m_ProgCounter; }
            set { this.m_ProgCounter = value; }
        }

        public ushort RegisterI
        {
            get { return (ushort)(this.m_IReg & 0x0FFF); }
            set { this.m_IReg = (ushort)(value & 0x0FFF); }
        }

        public bool IsPaused
        {
            get { return this.m_Paused; }
        }

        public ushort[] Stack
        {
            get { return this.m_Stack; }
        }

        public int StackIndex
        {
            get { return this.m_StackPointer; }
        }
    }
}
