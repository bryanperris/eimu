using System;
using System.Collections.Generic;
using System.Threading;

// Some RPL Register Doc
// Test Program: Schip Test.sc
// RPL0 = Logo X Position
// RPL1 = Logo Y Position
// RPL2 = Number Position

namespace Eimu.Core.Systems.SChip8
{
    [Serializable]
    public abstract class CodeEngine : IDisposable
    {
        public const int StackMaxSize = 12;
        public const int TimerRate = 17;

        public Random m_Rand;
        public Stack<ushort> m_Stack;
        public byte m_LastKey;
        public byte[] m_VRegs = new byte[16];
        public byte[] m_RPLFlags = new byte[8];
        public int m_PC;
        public ushort m_IReg;
        public int m_DT;
        public int m_ST;
        public Timer m_DelayTimer;
        public Timer m_SoundTimer;
        public Memory m_Memory;
        public bool m_SMode;

        private bool m_Paused;
        private bool m_Stop;
        private EventWaitHandle m_TimerWait;
        public event EventHandler ScreenClear;
        public event EventHandler<PixelSetEventArgs> PixelSet;
        public event EventHandler<SuperModeChangedEventArgs> SuperModeChange;
        public event EventHandler<PixelScrollEventArgs> PixelScroll;
        public event EventHandler KeyPressWait;
        private bool m_DisableTimers;

        public virtual void Init(Memory memory)
        {
            m_Memory = memory;
            m_TimerWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_Paused = false;
            m_Stop = false;
            m_PC= 0;
            m_IReg = 0;
            Array.Clear(this.m_VRegs, 0, this.m_VRegs.Length);
            Array.Clear(this.m_RPLFlags, 0, this.m_RPLFlags.Length);
            //m_RPLFlags[0] = 32;
            //m_RPLFlags[1] = 16;
            //m_RPLFlags[2] = 64;
            m_ST = 0;
            m_LastKey = 17;
            m_DT = 0;
            m_Rand = new Random(System.Environment.TickCount);
            m_Stack = new Stack<ushort>(StackMaxSize);
        }

        public virtual void Shutdown()
        {
            m_Rand = null;
            if (m_DelayTimer != null)
            {
                m_DelayTimer.Dispose(m_TimerWait);
                m_TimerWait.WaitOne();
            }
        }

        public abstract void Call(ChipInstruction inst);

        public void IncrementPC()
        {
            Interlocked.Add(ref m_PC, 2);
        }

        public Memory CurrentMemory
        {
            get { return this.m_Memory; }
        }

        public ushort ReadReg(byte num)
        {
            switch (num)
            {
                case 0: return 0;
                case 1: return 0;
                case 2: return (ushort)m_Stack.Count;
                default: return 0;
            }
        }


        public int PC
        {
            get { return this.m_PC; }
            set { this.m_PC = value; }
        }

        public byte[] VRegisters
        {
            get
            {
                return this.m_VRegs;
            }
        }

        protected void OnSuperModeChange(bool enabled)
        {
            m_SMode = enabled;

            if (SuperModeChange != null)
                SuperModeChange(this, new SuperModeChangedEventArgs(enabled));
        }

        protected void OnPixelSet(ChipInstruction inst)
        {
            if (PixelSet != null)
            {
                m_VRegs[0xF] = 0;
                byte x = m_VRegs[inst.X];
                byte y = m_VRegs[inst.Y];
                byte read = 0;

                if (inst.N > 0)
                {
                    for (byte i = 0; i < inst.N; i++)
                    {
                        read = m_Memory.GetByte(m_IReg + i);

                        for (byte j = 0; j < 8; j++)
                        {
                            if ((read & (0x80 >> j)) != 0)
                            {
                                PixelSet(this, new PixelSetEventArgs((x + j), (y + i)));
                            }
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < 0x10; k++)
                    {
                        ushort data = Tools.Create16(m_Memory.GetByte(m_IReg + (k << 1)), m_Memory.GetByte(m_IReg + (k << 1) + 1));

                        for (int m = 0; m < 0x10; m++)
                        {
                            if ((data & (((int)0x8000) >> m)) != 0)
                            {
                                PixelSet(this, new PixelSetEventArgs((x + m), (y + k)));
                            }
                        }
                    }
                }
            }

            Thread.Sleep(8);
        }

        protected void OnScreenClear()
        {
            if (ScreenClear != null)
                ScreenClear(this, new EventArgs());
        }

        protected void OnWaitForKey()
        {
            if (KeyPressWait != null)
                KeyPressWait(this, new EventArgs());
        }

        protected void OnPixelScroll(int dir, int length)
        {
            if (PixelScroll != null)
                PixelScroll(this, new PixelScrollEventArgs(length, dir));
        }

        private void DelayTimerCallback(object state)
        {
            if (m_DT > 0 && !m_Stop)
            {
                if (!m_Paused)
                {
                    m_DT--;
                }
            }
        }

        private void SoundTimerCallback(object state)
        {
            if (m_ST > 0 && !m_Stop)
            {
                if (!m_Paused)
                {
                    m_ST--;
                }
            }
        }

        protected void OnSetDelayTimer(byte value)
        {
            if (m_DisableTimers)
                return;

            m_DT = value;
            m_DelayTimer = new Timer(new TimerCallback(DelayTimerCallback), this, 0, TimerRate);
        }

        protected void OnSetSoundTimer(byte value)
        {
            if (m_DisableTimers)
                return;

            m_ST = value;
            m_SoundTimer = new Timer(new TimerCallback(SoundTimerCallback), this, 0, TimerRate);
        }

        public byte LastKeyPressed
        {
            get { return this.m_LastKey; }
            set { this.m_LastKey = value; }
        }

        public bool Stopped
        {
            get { return this.m_Stop; }
            set { this.m_Stop = value; }
        }

        public bool Paused
        {
            get { return this.m_Paused; }
            set { this.m_Paused = value; }
        }

        public bool DisableTimers
        {
            get { return this.m_DisableTimers; }
            set { m_DisableTimers = value; }
        }

        public int DelayTimer
        {
            get { return this.m_DT; }
            set { this.m_DT = value; }
        }

        public int SoundTimer
        {
            get { return this.m_ST; }
            set { this.m_ST = value; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               
            }

            m_TimerWait.Close();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
