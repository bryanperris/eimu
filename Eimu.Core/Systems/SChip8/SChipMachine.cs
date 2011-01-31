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
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using Eimu.Core.Systems.SChip8.Engines;

namespace Eimu.Core.Systems.SChip8
{
    [ComVisible(true)]
    public sealed class SChipMachine : VirtualMachine, IDisposable
    {
        public const int FONT_SIZE = 5;
        public const int MEMORY_SIZE = 4096;
        public const int PROGRAM_ENTRY_POINT = 0x200;
        private bool m_Paused = false;
        private bool m_RequestCPUStop;
        private Stream m_FontSource;
        private EventWaitHandle m_CPUWait;
        private EventWaitHandle m_KeyWait;
        private EventWaitHandle m_CPUEndWait;
        private Thread m_ThreadCPU;
        private AudioDevice m_AudioDevice;
        private GraphicsDevice m_GraphicsDevice;
        private CodeEngine m_CodeEngine;
        private bool m_SoundLooping;


        // ----------------------------
        // Machine Control
        // ----------------------------

        protected override void OnMachineState(RunState state)
        {
            CurrentAudioDevice.SetPauseState((state == RunState.Paused));
            CurrentGraphicsDevice.SetPauseState((state == RunState.Paused));
            SetPauseState((state == RunState.Paused));

            if (state == RunState.Stopped)
            {
                if (m_SoundLooping)
                {
                    m_SoundLooping = false;
                    m_AudioDevice.LoopEnd();
                }
                m_CodeEngine.Shutdown();
                m_CPUWait.Set();
                m_KeyWait.Set();
                m_RequestCPUStop = true;
                CurrentAudioDevice.Shutdown();
                CurrentGraphicsDevice.Shutdown();
            }

        }

        // ----------------------------
        // Setters
        // ----------------------------

        public void SetFontResource(Stream source)
        {
            m_FontSource = source;
        }

        public void SetCollision()
        {
            m_CodeEngine.VRegisters[0xF] = 1;
        }

        private void SetPauseState(bool paused)
        {
            this.m_Paused = paused;

            if (!m_Paused)
                m_CPUWait.Set();
        }

        public void SetKeyPress(ChipKey key)
        {
            if (key != ChipKey.None)
            {
                m_CodeEngine.LastKeyPressed = (byte)key;
                m_KeyWait.Set();
            }
            else
            {
                m_CodeEngine.LastKeyPressed = 17;
            }
        }


        // ----------------------------
        // Program Control
        // ----------------------------

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
                m_CodeEngine.Call(inst);

                if (m_CodeEngine.DelayTimer > 0) m_CodeEngine.DelayTimer--;

                if (m_CodeEngine.SoundTimer > 0)
                {
                    m_CodeEngine.SoundTimer--;

                    //if (!m_SoundLooping && m_CodeEngine.SoundTimer > 1)
                    //{
                    //    m_SoundLooping = true;
                    //    m_AudioDevice.LoopBegin();
                    //}
                    //else
                    {
                        m_AudioDevice.Beep();
                    }
                }

                if (m_CodeEngine.SoundTimer <= 0 && m_SoundLooping)
                {
                    m_SoundLooping = false;
                    m_AudioDevice.LoopEnd();
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

            while (m_CodeEngine.PC < SystemMemory.Size) {
                if (!m_RequestCPUStop) {
                    if (m_Paused)
                        m_CPUWait.WaitOne();

                    int speed = 1;
                    int cycles = (((speed * 100) + 1) / 60);
                    Step(cycles);
                    Thread.Sleep(2);
                }    
            }

            m_CPUEndWait.Set();
        }

        protected override bool Boot()
        {
            Console.WriteLine("Booting...");

            m_SoundLooping = false;
            if (CurrentAudioDevice == null) throw new NullReferenceException();
            if (CurrentGraphicsDevice == null) throw new NullReferenceException();
            m_CPUWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_KeyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_CPUEndWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RequestCPUStop = false;
            m_Paused = false;
            m_ThreadCPU = new Thread(new ThreadStart(StartExecutionCycle));
            m_ThreadCPU.IsBackground = true;
            SystemMemory = new Memory(MEMORY_SIZE);
            m_CodeEngine = new Interpreter();
            m_CodeEngine.Init(this.SystemMemory);
            AttachDeviceCallbacks();
            CurrentAudioDevice.Initialize();
            CurrentGraphicsDevice.Initialize();
            CurrentGraphicsDevice.ClearScreen();
            if (!LoadFont()) return false;
            if (!LoadRom()) return false;
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

                while ((read = m_FontSource.ReadByte()) != -1)
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


        // ----------------------------
        // Machine Properties
        // ----------------------------

        public CodeEngine CodeEngineCore
        {
            get { return this.m_CodeEngine; }
        }

        public AudioDevice CurrentAudioDevice
        {
            get { return this.m_AudioDevice; }
            set { this.m_AudioDevice = value; }
        }

        public GraphicsDevice CurrentGraphicsDevice
        {
            get { return this.m_GraphicsDevice; }
            set { this.m_GraphicsDevice = value; }
        }


        // ----------------------------
        // Internal Calls
        // ----------------------------

        private void SendBeep()
        {
            m_AudioDevice.Beep();
        }

        private void AttachDeviceCallbacks()
        {
            m_CodeEngine.PixelSet -= new EventHandler<PixelSetEventArgs>(OnPixelSet);
            m_CodeEngine.PixelSet += new EventHandler<PixelSetEventArgs>(OnPixelSet);

            m_CodeEngine.ScreenClear -= new EventHandler(OnScreenClear);
            m_CodeEngine.ScreenClear += new EventHandler(OnScreenClear);

            CurrentGraphicsDevice.OnPixelCollision -= new EventHandler(OnPixelCollision);
            CurrentGraphicsDevice.OnPixelCollision += new EventHandler(OnPixelCollision);

            m_CodeEngine.SuperModeChange -= new EventHandler<SuperModeChangedEventArgs>(m_CodeEngine_SuperModeChange);
            m_CodeEngine.SuperModeChange += new EventHandler<SuperModeChangedEventArgs>(m_CodeEngine_SuperModeChange);
        }

        private void m_CodeEngine_SuperModeChange(object sender, SuperModeChangedEventArgs e)
        {
            m_GraphicsDevice.SetSuperMode(e.Enabled);
        }

        private void OnPixelCollision(object sender, EventArgs e)
        {
            SetCollision();
        }

        private void OnScreenClear(object sender, EventArgs e)
        {
            m_GraphicsDevice.ClearScreen();
        }

        private void OnPixelSet(object sender, PixelSetEventArgs e)
        {
            m_GraphicsDevice.SetPixel(e.X, e.Y);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                OnMachineState(RunState.Stopped);
            }

            m_CPUWait.Close();
            m_CPUEndWait.Close();
            m_KeyWait.Close();
        }

        #endregion
    }
}
