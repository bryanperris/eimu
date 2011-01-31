using System;
using System.Collections.Generic;
using System.Threading;

namespace Eimu.Core.Systems.SChip8
{
    [Serializable]
    public abstract class CodeEngine : IDisposable
    {
        public const int StackMaxSize = 12;
        protected Random m_Rand;
        protected Stack<ushort> m_Stack;
        protected byte m_LastKey;
        protected byte[] m_VRegs = new byte[16];
        protected byte[] m_RPLFlags = new byte[8];
        protected int m_PC;
        protected ushort m_IReg;
        protected int m_DT;
        protected int m_ST;
        protected Timer m_DelayTimer;
        protected Memory m_Memory;
        private bool m_Paused;
        private bool m_Stop;
        private EventWaitHandle m_TimerWait;
        public event EventHandler ScreenClear;
        public event EventHandler<PixelSetEventArgs> PixelSet;
        public event EventHandler<SuperModeChangedEventArgs> SuperModeChange;
        public event EventHandler KeyPressWait;
        private bool m_DisableTimers;
        private bool m_SuperMode;

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
            m_SuperMode = enabled;

            if (SuperModeChange != null)
                SuperModeChange(this, new SuperModeChangedEventArgs(enabled));
        }

        protected void OnPixelSet(ChipInstruction inst)
        {
            if (m_SuperMode)
            {
                SuperPixelSet(inst);
                return;
            }

            if (PixelSet != null)
            {
                m_VRegs[0xF] = 0;
                byte x = m_VRegs[inst.X];
                byte y = m_VRegs[inst.Y];
                byte read = 0;

                for (byte i = 0; i < inst.N; i++)
                {
                    read = m_Memory.GetByte(m_IReg + i);

                    for (byte j = 0; j < GraphicsDevice.StandardSpriteSize; j++)
                    {
                        // Keep writing pixels until we hit a 0 bit (width end)
                        if ((read & (0x80 >> j)) != 0)
                        {
                            PixelSet(this, new PixelSetEventArgs((x + j), (y + i)));
                        }
                    }
                }
            }
        }

        private void SuperPixelSet(ChipInstruction inst)
        {
            if (PixelSet != null)
            {
                m_VRegs[0xF] = 0;
                byte x = m_VRegs[inst.X];
                byte y = m_VRegs[inst.Y];
                byte read = 0;

                //Console.WriteLine("");
                //Console.WriteLine("Sprite: " + inst.N.ToString() + "x" + GraphicsDevice.SuperSpriteSize.ToString());

                for (byte i = 0; i < inst.N; i++)
                {
                    read = m_Memory.GetByte(m_IReg + i);

                    
                    //Console.Write(((read >> 7) & 1).ToString());
                    //Console.Write(((read >> 6) & 1).ToString());
                    //Console.Write(((read >> 5) & 1).ToString());
                    //Console.Write(((read >> 4) & 1).ToString());
                    //Console.Write(((read >> 3) & 1).ToString());
                    //Console.Write(((read >> 2) & 1).ToString());
                    //Console.Write(((read >> 1) & 1).ToString());
                    //Console.Write(((read >> 0) & 1).ToString());
                    //Console.WriteLine("");

                    for (byte j = 0; j < GraphicsDevice.SuperSpriteSize; j++)
                    {
                        // Keep writing pixels until we hit a 0 bit (width end)
                        if ((read & (0x80 >> j)) != 0)
                        {
                            PixelSet(this, new PixelSetEventArgs((x + j), (y + i)));
                        }
                    }
                }
            }
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

        protected void OnSetDelayTimer(byte value)
        {
            if (m_DisableTimers)
                return;

            m_DT = value;
            //m_DelayTimer = new Timer(new TimerCallback(DelayTimerCallback), this, 0, 10);
        }

        protected void OnSetSoundTimer(byte value)
        {
            if (m_DisableTimers)
                return;

            m_ST = value;
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
