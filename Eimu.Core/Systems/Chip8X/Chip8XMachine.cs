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
using Eimu.Core.Systems.RCA1802;

namespace Eimu.Core.Systems.Chip8X
{
    [ComVisible(true)]
    public sealed class Chip8XMachine : VirtualMachine, IDisposable
    {
        public const int FONT_SIZE = 5;
        public const int MEMORY_SIZE = 4096;
        public const int PROGRAM_ENTRY_POINT = 0x200;
        private bool m_Paused = false;
        private bool m_RequestCPUStop;
        private bool m_Use1802Dynarec;
        private int m_ExtraCycles;
        private int m_CoreSpeed = 10;
        private Stream m_FontSource;
        private Stream m_SFontSource;
        private EventWaitHandle m_CPUPause;
        private EventWaitHandle m_KeyWait;
        private EventWaitHandle m_CPUFinishWait;
        private Thread m_ThreadCPU;
        private AudioDevice m_AudioDevice;
        private Renderer m_Renderer;
        private CodeEngine m_CodeEngine;
        private C1802Mode m_HLMode;
        private C1802Dynarec m_CDPDynarec;
        private VideoInterface m_VideoInterface;

        public Chip8XMachine()
        {
            m_VideoInterface = new VideoInterface();
        }

        protected override void OnMachineState(RunState state)
        {
            CurrentAudioDevice.SetPause((state == RunState.Paused));
            CurrentRenderBackend.SetPause((state == RunState.Paused));
            IsPaused = (state == RunState.Paused);

            if (state == RunState.Stopped)
            {
                m_CPUPause.Set();
                m_KeyWait.Set();
                m_RequestCPUStop = true;
                m_CPUFinishWait.WaitOne();
                m_CodeEngine.Shutdown();
                CurrentAudioDevice.Shutdown();
                CurrentRenderBackend.Shutdown();
                Console.WriteLine("Stopped...");
            }

        }

        public void KeyPress(HexKey key)
        {
            if (key != HexKey.None)
            {
                m_CodeEngine.LastKeyPressed = (byte)key;
                m_KeyWait.Set();
            }
            else
            {
                m_CodeEngine.LastKeyPressed = 17;
            }
        }

        #region Execution Control

        public void Step(int cycles)
        {
            while (cycles > 0)
            {
                cycles--;
                byte a = SystemMemory.GetByte(m_CodeEngine.PC);
                byte b = SystemMemory.GetByte(m_CodeEngine.PC + 1);
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

                        if (m_Use1802Dynarec)
                            m_CDPDynarec.Call(inst.NNN, m_CodeEngine);
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

        public void Run(int entryAddress)
        {
            m_CodeEngine.PC = entryAddress;
            m_ThreadCPU.Start();
        }

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

        protected override bool Boot()
        {
            Console.WriteLine("Booting...");

            if (CurrentAudioDevice == null) throw new NullReferenceException();
            if (CurrentRenderBackend == null) throw new NullReferenceException();
            m_CPUPause = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_KeyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_CPUFinishWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RequestCPUStop = false;
            m_Paused = false;
            m_ThreadCPU = new Thread(new ThreadStart(StartExecutionCycle));
            m_ThreadCPU.IsBackground = true;
            SystemMemory = new Memory(MEMORY_SIZE);
            m_CodeEngine = new Interpreter();
            m_CodeEngine.Init(this);
            m_CDPDynarec = new C1802Dynarec();
            AttachDeviceCallbacks();
            CurrentAudioDevice.Initialize();
            CurrentRenderBackend.Initialize();
            m_VideoInterface.Initialize(ChipMode.Chip8);
            if (!LoadFont()) return false;
            if (!LoadRom()) return false;
            Console.WriteLine("Running...");
            Run(PROGRAM_ENTRY_POINT);
            return true;
        }

        private bool LoadFont()
        {
            try
            {
                int read;
                int pos = 0;
                m_FontSource.Position = 0;
                m_SFontSource.Position = 0;

                while ((read = m_FontSource.ReadByte()) != -1)
                {
                    this.SystemMemory[pos++] = (byte)read;
                }

                while ((read = m_SFontSource.ReadByte()) != -1)
                {
                    this.SystemMemory[pos++] = (byte)read;
                }

                return true;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Font is missing!");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("can't access font!");
                return false;
            }
        }

        private bool LoadRom()
        {

            if (this.MediaSource == null)
                return false;

            if (!this.MediaSource.CanRead)
            {
                Console.WriteLine("Source can't be read!");
                return false;
            }

            this.MediaSource.Position = 0;
            int read;
            int pos = PROGRAM_ENTRY_POINT;

            try
            {
                while ((read = MediaSource.ReadByte()) != -1)
                {
                    SystemMemory[pos++] = (byte)read;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Properties

        public CodeEngine ProcessorCore
        {
            get { return this.m_CodeEngine; }
        }

        public AudioDevice CurrentAudioDevice
        {
            get { return this.m_AudioDevice; }
            set { this.m_AudioDevice = value; }
        }

        public Renderer CurrentRenderBackend
        {
            get { return this.m_Renderer; }
            set { this.m_Renderer = value; }
        }

        public int ExtraCycleSpeed
        {
            get { return m_ExtraCycles; }
            set { m_ExtraCycles = value; }
        }

        public C1802Mode MachineMode
        {
            get { return this.m_HLMode; }
            set { this.m_HLMode = value; }
        }

        public bool Enable1802Dyanrec
        {
            get { return this.m_Use1802Dynarec; }
            set { this.m_Use1802Dynarec = value; }
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

        #endregion

        #region Private

        private void SendBeep()
        {
            m_AudioDevice.Beep();
        }

        private void AttachDeviceCallbacks()
        {
            m_VideoInterface.VideoRefresh -= new EventHandler<VideoFrameUpdate>(m_VideoInterface_VideoRefresh);
            m_VideoInterface.VideoRefresh += new EventHandler<VideoFrameUpdate>(m_VideoInterface_VideoRefresh);
        }

        void m_VideoInterface_VideoRefresh(object sender, VideoFrameUpdate e)
        {
            m_Renderer.Update(e);
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
                OnMachineState(RunState.Stopped);
            }

            m_CPUPause.Close();
            m_CPUFinishWait.Close();
            m_KeyWait.Close();
        }

        #endregion
    }
}
