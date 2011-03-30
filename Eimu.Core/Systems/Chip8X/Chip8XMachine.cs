/*  
Eimu - Chip-8X Emulator
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
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Eimu.Core.Systems.Chip8X.Engines;
using Eimu.Core.Systems.CDP1802;
using Eimu.Core.Dynarec;

namespace Eimu.Core.Systems.Chip8X
{
    [Serializable]
    public sealed class Chip8XMachine : VirtualMachine, IDisposable
    {
        public const int FONT_SIZE = 5;
        public const int PROGRAM_ENTRY_POINT = 0x200;
        private ResourceManager m_ResManager;

        #region State Members

        private bool m_Paused = false;
        private bool m_RequestCPUStop;
        private bool m_UseHybridDynarec;
        private int m_ExtraCycles;
        private int m_CoreSpeed = 10;
        private CDP1802Mode m_1802Mode;
        private CodeEngine m_CodeEngine;
        private ILDynarec<ILEmitter1802> m_HybridDynarec;
        private VideoInterface m_VideoInterface;
        private AudioInterface m_AudioInterface;
        private Stream m_FontSource;
        private Stream m_SFontSource;

        #endregion
        #region Threading Members

        private EventWaitHandle m_CPUPause;
        private EventWaitHandle m_KeyWait;
        private EventWaitHandle m_CPUFinishWait;
        private Thread m_ThreadCPU;

        #endregion

        public Chip8XMachine() : base()
        {
            m_VideoInterface = new VideoInterface();
            m_AudioInterface = new AudioInterface();
            m_CodeEngine = new Interpreter(this);
            m_HybridDynarec = new ILDynarec<ILEmitter1802>();
            m_ResManager = new ChipResources(this);
        }

        #region Execution Control

        protected override void OnStateChanged(RunState state)
        {
            if (state == RunState.Stopped)
            {
                m_CPUPause.Set();
                m_KeyWait.Set();
                m_RequestCPUStop = true;
                m_CPUFinishWait.WaitOne();
                m_CodeEngine.Shutdown();
                m_VideoInterface.Shutdown();
                m_AudioInterface.Shutdown();
                Console.WriteLine("Stopped...");
                return;
            }

            IsPaused = (state == RunState.Paused);
            m_AudioInterface.IsPaused = (state == RunState.Paused);
            m_VideoInterface.IsPaused = (state == RunState.Paused);

        }

        public void Step(int cycles)
        {
            while (cycles > 0)
            {
                cycles--;
                byte a = SystemMemory.ReadByte(m_CodeEngine.PC);
                byte b = SystemMemory.ReadByte(m_CodeEngine.PC + 1);
                ushort data = Tools.Create16(a, b);
                ChipOpCode opcode = Disassembler.DecodeInstruction(data);
                ChipInstruction inst = new ChipInstruction(data, opcode);
                inst.Address = m_CodeEngine.PC;
                m_CodeEngine.IncrementPC();

                if (opcode == ChipOpCode.Unknown)
                {
                    if (data != 0)
                    {
                        Console.WriteLine("Syscall: " + inst.NNN.ToString("x"));

                        if (m_UseHybridDynarec)
                        {
                            if (inst.NNN < SystemMemory.Size)
                                m_HybridDynarec.Execute(inst.NNN, m_CodeEngine);
                            else
                                Console.WriteLine("1802 Call is beyond memory bounds! (" + inst.NNN.ToString("X4") + ")");
                        }
                        else
                            Console.WriteLine("Syscall Emitter Disabled!");
                    }
                }
                else
                {
                    m_CodeEngine.Call(inst);
                }
            }
        }

        protected override bool Boot()
        {
            Console.WriteLine("Booting...");

            m_CodeEngine.PC = PROGRAM_ENTRY_POINT;
            m_CPUPause = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_KeyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_CPUFinishWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RequestCPUStop = false;
            m_Paused = false;

            m_CodeEngine.Initialize(this);
            m_VideoInterface.Initialize(ChipMode.Chip8);

            if (!Resources.LoadResources())
            {
                Console.WriteLine("Resource load error!");
                return false;
            }

            m_ThreadCPU = new Thread(new ThreadStart(StartExecutionCycle));
            m_ThreadCPU.IsBackground = true;
            m_ThreadCPU.Start();
            Console.WriteLine("Running...");

            return true;
        }

        #endregion

        #region Properties

        public HexKey PressedKey
        {
            get { return (HexKey)m_CodeEngine.PressedKey; }
            set { m_CodeEngine.PressedKey = (byte)value; }
        }

        public CodeEngine ProcessorCore
        {
            get { return this.m_CodeEngine; }
        }

        public int ExtraCycleSpeed
        {
            get { return m_ExtraCycles; }
            set { m_ExtraCycles = value; }
        }

        public CDP1802Mode MachineMode
        {
            get { return this.m_1802Mode; }
            set { this.m_1802Mode = value; }
        }

        public bool IsHybridDynarecEnabled
        {
            get { return this.m_UseHybridDynarec; }
            set { this.m_UseHybridDynarec = value; }
        }

        public Stream Chip8FontSource
        {
            get { return m_FontSource; }
            set { m_FontSource = value; }
        }

        public Stream SuperChipFontSource
        {
            get { return m_SFontSource; }
            set { m_SFontSource = value; }
        }

        public bool IsPaused
        {
            get { return this.m_Paused; }
            set
            {
                this.m_Paused = value;

                if (!m_Paused)
                    m_CPUPause.Set();
            }
        }

        public VideoInterface VideoInterface
        {
            get { return m_VideoInterface; }
        }

        public AudioInterface AudioInterface
        {
            get { return m_AudioInterface; }
        }

        public override ResourceManager Resources
        {
            get { return m_ResManager; }
        }

        public override Memory SystemMemory
        {
            get { return (Memory)m_CodeEngine.Memory; }
        }

        #endregion

        #region Private

        private void StartExecutionCycle()
        {
            Thread.CurrentThread.Name = "CPU Thread";

            while (m_CodeEngine.PC < SystemMemory.Size)
            {
                if (!m_RequestCPUStop)
                {
                    if (m_Paused)
                        m_CPUPause.WaitOne();

                    Step(m_CoreSpeed + m_ExtraCycles);
                }
                else
                {
                    break;
                }
            }

            m_CPUFinishWait.Set();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnStateChanged(RunState.Stopped);
            }

            m_CPUPause.Close();
            m_CPUFinishWait.Close();
            m_KeyWait.Close();
        }

        #endregion
    }
}
