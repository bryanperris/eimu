using System;
using System.Collections.Generic;
using System.Threading;

// Some RPL Register Doc
// Test Program: Schip Test.sc
// RPL0 = Logo X Position
// RPL1 = Logo Y Position
// RPL2 = Number Position

namespace Eimu.Core.Systems.Chip8X
{
    [Serializable]
    public abstract class CodeEngine : IDisposable
    {
        public const int StackMaxSize = 12;
        public const int TimerRate = 17;
        public event EventHandler<ChipModeChangedEventArgs> ModeChange;
        public event EventHandler KeyPressWait;
        public VideoInterface m_VideoInterface;
        private bool m_Stop;
        private EventWaitHandle m_TimerWait;
        private bool m_DisableTimers;

        #region CPU State Fields

        private bool m_Paused;
        private byte[] m_VRegs = new byte[16];
        private byte[] m_RPLFlags = new byte[8];
        private ushort[] m_1802Regs = new ushort[16];
        private Stack<ushort> m_Stack;
        private Random m_Rand;
        private byte m_LastKey;
        private Timer m_DelayTimer;
        private Timer m_SoundTimer;
        private int m_PC;
        private ushort m_IReg;
        private int m_DT;
        private int m_ST;
        private ushort m_RoutineAddress;
        private ushort m_LastRand;
        private ChipMemory m_Memory;

        #endregion
        
        public CodeEngine(Chip8XMachine currentMachine)
        {
            m_Memory = (ChipMemory)currentMachine.SystemMemory;
            m_VideoInterface = currentMachine.VideoInterface;
        }

        public void Initialize(Chip8XMachine machine)
        {
            m_TimerWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_Paused = false;
            m_Stop = false;
            m_PC = 0;
            m_IReg = 0;
            Array.Clear(this.m_VRegs, 0, this.m_VRegs.Length);
            Array.Clear(this.m_RPLFlags, 0, this.m_RPLFlags.Length);
            m_ST = 0;
            m_LastKey = 17;
            m_DT = 0;
            m_Rand = new Random(System.Environment.TickCount);
            m_Stack = new Stack<ushort>(StackMaxSize);
            OnInit();
        }

        public void Shutdown()
        {
            m_Rand = null;
            if (m_DelayTimer != null)
            {
                m_DelayTimer.Dispose(m_TimerWait);
                m_TimerWait.WaitOne();
            }
            OnInit();
        }

        public void IncrementPC()
        {
            Interlocked.Add(ref m_PC, 2);
        }

        public ushort Read1802Register(int index)
        {
            switch (index)
            {
                case 0: return m_1802Regs[0]; // DMA pointer
                //case 1: TODO: Interrupt VIP PC
                //case 2: pointer to stack memory, TOOD: Handle it
                case 3: return m_RoutineAddress;
                //case 4: return 0; // Commonly used as the VIP's Program Counter
                case 5: return (ushort)m_PC;
                //case 6: // TOOD: Pointer to VX memory
                //case 7: // TODO: Pointer to VY memory
                //case 8: // TODO: Sound Timer Value [hi], Timer Tone [low]
                //case 9: return m_LastRand; // Random number (+1 in INTERRUPT routine)
                case 10: return m_IReg;
                case 11: return ChipMemory.MEMORY_VIDEO_OFFSET; // pointer to graphics memory, using a fake address
                // RC, RD, RE, RF are used as general regs
                default: return m_1802Regs[index];
            }
        }

        public void Set1802Register(int index, ushort value)
        {
            switch (index)
            {
                case 0: m_1802Regs[0] = value; break;
                case 3: m_RoutineAddress = value; break;
                //case 4: m_1802Regs[4] = value; break;
                case 5: m_PC = (int)value; break;
                case 10: m_IReg = value; break;

                default:
                    {
                        m_1802Regs[index] = value; break;
                        //Console.WriteLine("Emitter: Unknown reg write! (" + num.ToString("x") + ")"); break;
                    }
            }
        }

        #region Abstract Members
        
        public abstract void OnInit();

        public abstract void OnShutdown();

        public abstract void Call(ChipInstruction inst);

        #endregion

        #region Protected Members

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

        protected void OnChipModeChange(ChipMode mode)
        {
            if (ModeChange != null)
                ModeChange(this, new ChipModeChangedEventArgs(mode));
        }

        protected void OnPixelSet(ChipInstruction inst)
        {
            m_VRegs[0xF] = 0;
            byte x = m_VRegs[inst.X];
            byte y = m_VRegs[inst.Y];
            byte read = 0;

            if (inst.N > 0)
            {
                for (byte i = 0; i < inst.N; i++)
                {
                    read = m_Memory.ReadByte((int)(m_IReg + i));

                    for (byte j = 0; j < 8; j++)
                    {
                        if ((read & (0x80 >> j)) != 0)
                        {
                            if (m_VideoInterface.SetPixel((x + j), (y + i)))
                            {
                                m_VRegs[0xF] = 1;
                            }
                        }
                    }
                }
            }
            else
            {
                for (int k = 0; k < 0x10; k++)
                {
                    ushort data = Tools.Create16(m_Memory.ReadByte((int)(m_IReg + (k << 1))), 
                        m_Memory.ReadByte((int)(m_IReg + (k << 1) + 1)));

                    for (int m = 0; m < 0x10; m++)
                    {
                        if ((data & (((int)0x8000) >> m)) != 0)
                        {
                            if (m_VideoInterface.SetPixel((x + m), (y + k)))
                            {
                                m_VRegs[0xF] = 1;
                            }
                        }
                    }
                }
            }

            Thread.Sleep(8);
        }

        protected void OnScreenClear()
        {
            m_VideoInterface.ClearPixels();
        }

        protected void OnWaitForKey()
        {
            if (KeyPressWait != null)
                KeyPressWait(this, new EventArgs());
        }

        protected void OnPixelScroll(int dir, int length)
        {
            m_VideoInterface.ScrollPixels(length, dir);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }

            m_TimerWait.Close();
        }

        #endregion

        #region Private Members

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


        #endregion

        #region Properties

        public ChipMemory Memory
        {
            get { return this.m_Memory; }
        }

        public int PC
        {
            get { return this.m_PC; }
            set { this.m_PC = value; }
        }

        public ushort LastRandomValue
        {
            get { return m_LastRand; }
            set { m_LastRand = value; }
        }

        public Stack<ushort> Stack
        {
            get { return m_Stack; }
        }

        public ushort RoutineAddress
        {
            get { return m_RoutineAddress; }
            set { m_RoutineAddress = value; }
        }

        public ushort AddressRegister
        {
            get { return m_IReg; }
            set { m_IReg = value; }
        }

        public byte[] RPLRegisters
        {
            get { return m_RPLFlags; }
        }

        public byte[] VRegisters
        {
            get { return m_VRegs; }
        }

        public byte PressedKey
        {
            get { return this.m_LastKey; }
            set { this.m_LastKey = value; }
        }

        public bool IsStopped
        {
            get { return this.m_Stop; }
            set { this.m_Stop = value; }
        }

        public bool IsPaused
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

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
