﻿using System;
using System.Collections.Generic;
using System.Threading;
using Eimu.Core.Systems.Chip8X.Interfaces;
using Eimu.Core.Systems.Chip8X.CodeUtils;

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
        private bool m_Stop;
        private EventWaitHandle m_TimerWait;
        private bool m_DisableTimers;
        private Chip8XMachine m_CurrentMachine;
        private bool m_AntiFlickerHack = false;

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
            m_CurrentMachine = currentMachine;
        }

        public void Initialize(Chip8XMachine machine)
        {
            m_Memory = new ChipMemory(m_CurrentMachine);
            m_1802Regs[0x6] = (ushort)Memory.ChipCorePointer;
            m_1802Regs[0x7] = (ushort)Memory.ChipCorePointer;
            m_1802Regs[0xF] = (ushort)Memory.VideoPointer;
            m_Memory.Reset();
            m_TimerWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_Paused = false;
            m_Stop = false;
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
            OnShutdown();
            m_Rand = null;
            if (m_DelayTimer != null)
            {
                m_DelayTimer.Dispose(m_TimerWait);
                m_TimerWait.WaitOne();
            }
        }

        public void IncrementPC()
        {
            Interlocked.Add(ref m_PC, 2);
        }

        public ushort Read1802Register(int index)
        {
            switch (index)
            {
                case 0: return m_1802Regs[0]; // DMA pointer - let the game picks its DMA pointer, then we try to handle it
                case 1: return m_1802Regs[1]; // the PC of the INTERRUPT, just return w/e assigned value
                //case 2: TODO: Pointer to fake stack memory page
                case 3: return m_RoutineAddress;
                case 4: return m_1802Regs[4]; // A PC register, assuming it must point after the syscall (probably only read with SEP, which is what we want)
                case 5: return (ushort)m_PC;
                // These are VRegister pointers
                case 6: return (ushort)m_1802Regs[6];
                case 7: return (ushort)m_1802Regs[7];
                case 8: return (ushort)((m_DT << 8) & m_ST); // Timer data
                case 9: return m_LastRand; // Return last used random num, probably doesn't matter
                case 10: return m_IReg;
                case 11: return (ushort)Memory.VideoPointer; // pointer to the graphics memory page
                // RC, RD, RE, RF are used as general regs
                default: return m_1802Regs[index];
            }
        }

        public void Set1802Register(int index, ushort value)
        {
            switch (index)
            {
                case 0: m_1802Regs[0] = value; break; // DMA pointer
                case 1: m_1802Regs[1] = value; break; // Interrupt PC
                //case 2: TODO: Set a value in the stack directly
                case 3: m_RoutineAddress = value; break;
                case 4: m_1802Regs[4] = value; break; // Set this regiser to w/e
                case 5: m_PC = (int)value; break;
                case 6: m_1802Regs[6] = value; break; // Set X pointer
                case 7: m_1802Regs[7] = value; break; // Set Y pointer
                case 8: OnSetDelayTimer((byte)((value & 0xFF00) >> 8)); OnSetSoundTimer((byte)(value & 0x00FF)); break; // Set timer events
                case 9: m_Rand.Next(); break; // Randomize our random, ignore the passed in value since its random anyways
                case 10: m_IReg = value; break;
                case 11: break; // Don't let game attempt to change the video pointer

                default:
                    {
                        m_1802Regs[index] = value; break;
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
                // Loop through the sprite bytes by N
                for (byte i = 0; i < inst.N; i++)
                {
                    // Get the next byte of the sprite data
                    read = Memory.ReadByte((int)(m_IReg + i));

                    // Sprite is always 8 in width, loop through each bit
                    for (byte j = 0; j < 8; j++)
                    {
                        // If we hit an on bit then Set a pixel at X, Y
                        if ((read & (0x80 >> j)) != 0)
                        {
                            // If we colliude, set VF
                            if (VideoInterface.SetPixel((x + j), (y + i)))
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
                    ushort data = Tools.Create16(Memory.ReadByte((int)(m_IReg + (k << 1))), 
                        Memory.ReadByte((int)(m_IReg + (k << 1) + 1)));

                    for (int m = 0; m < 0x10; m++)
                    {
                        if ((data & (((int)0x8000) >> m)) != 0)
                        {
                            if (VideoInterface.SetPixel((x + m), (y + k)))
                            {
                                m_VRegs[0xF] = 1;
                            }
                        }
                    }
                }
            }

            if (m_VRegs[0xF] == 1 && m_AntiFlickerHack)
                return;

            VideoInterface.RenderWait();
        }

        protected void OnScreenClear()
        {
            VideoInterface.ClearPixels();
        }

        protected void OnWaitForKey()
        {
            if (KeyPressWait != null)
                KeyPressWait(this, new EventArgs());
        }

        protected void OnPixelScroll(int dir, int length)
        {
            VideoInterface.ScrollPixels(length, dir);
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

        public Random Random
        {
            get { return m_Rand; }
            set { m_Rand = value; }
        }

        public ChipMemory Memory
        {
            get { return m_Memory; }
        }

        public VideoInterface VideoInterface
        {
            get { return m_CurrentMachine.VideoInterface; }
        }

        public Chip8XMachine ParentMachine
        {
            get { return m_CurrentMachine; }
        }

        public bool AntiFlickerHack
        {
            get { return m_AntiFlickerHack; }
            set { m_AntiFlickerHack = value; }
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
