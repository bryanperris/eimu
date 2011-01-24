using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Eimu.Core.Systems.Chip8
{
    public class C8VMachine : VirtualMachine
    {
        public const int FONT_SIZE = 5;
        public const int MEMORY_SIZE = 4096;
        public const int CODE_OFFSET = 0x200;
        public const int PROGRAM_ENTRY_POINT = 0x200;
        private bool m_Paused = false;
        private bool m_RequestCPUStop;
        private Stream m_FontSource;
        private EventWaitHandle m_CPUWait;
        private EventWaitHandle m_KeyWait;
        private EventWaitHandle m_CPUEndWait;
        private Thread m_ThreadCPU;
        private Memory m_Memory;
        private AudioDevice m_AudioDevice;
        private GraphicsDevice m_GraphicsDevice;
        private InputDevice m_InputDevice;
        private CodeEngine m_CodeEngine;

        public C8VMachine()
        {
            SystemMemory = new Memory(MEMORY_SIZE);
        }

        // ----------------------------
        // Machine Control
        // ----------------------------

        private void Shutdown()
        {
            m_CodeEngine.Shutdown();
            m_CodeEngine.Beep -= new EventHandler<BeepEventArgs>(m_CodeEngine_OnBeep);
            m_CodeEngine.PixelSet -= new EventHandler<PixelSetEventArgs>(m_CodeEngine_OnPixelSet);
            m_CodeEngine.ScreenClear -= new EventHandler(m_CodeEngine_OnScreenClear);
            m_CodeEngine.KeyPressWait -= new EventHandler(m_CodeEngine_OnKeyPressWait);
            m_CPUWait.Set();
            m_KeyWait.Set();
            m_RequestCPUStop = true;
        }

        private void Initialize()
        {
            m_Profiler = new Profiler();
            m_CodeEngine.Beep += new EventHandler<BeepEventArgs>(m_CodeEngine_OnBeep);
            m_CodeEngine.PixelSet += new EventHandler<PixelSetEventArgs>(m_CodeEngine_OnPixelSet);
            m_CodeEngine.ScreenClear += new EventHandler(m_CodeEngine_OnScreenClear);
            m_CodeEngine.KeyPressWait += new EventHandler(m_CodeEngine_OnKeyPressWait);
            m_CodeEngine.Init();
            m_CPUWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_KeyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_CPUEndWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RequestCPUStop = false;
            m_Paused = false;
            m_ThreadCPU = new Thread(new ThreadStart(StartExecutionCycle));
            m_ThreadCPU.IsBackground = true;
        }

        protected override void OnMachineState(RunState state)
        {
            switch (state)
            {
                case RunState.Stopped:
                    {
                        Shutdown();
                        CurrentInputDevice.Shutdown();
                        CurrentAudioDevice.Shutdown();
                        CurrentGraphicsDevice.Shutdown();
                        break;
                    }
                case RunState.Paused:
                    {
                        CurrentAudioDevice.SetPauseState(true);
                        CurrentGraphicsDevice.SetPauseState(true);
                        CurrentInputDevice.SetPauseState(true);
                        SetPauseState(true);
                        break;
                    }
                case RunState.Running:
                    {
                        CurrentAudioDevice.SetPauseState(false);
                        CurrentGraphicsDevice.SetPauseState(false);
                        CurrentInputDevice.SetPauseState(false);
                       // m_Processor.SetPauseState(false);
                        break;
                    }

                default: break;
            }
        }


        // ----------------------------
        // Setters
        // ----------------------------

        public void SetFontResource(Stream source)
        {
            m_FontSource = source;
        }

        public void SetCodeEngineType<TCodeEngine>() where TCodeEngine : CodeEngine
        {
            m_Engine = (CodeEngine)Activator.CreateInstance(typeof(TCodeEngine), new object[] { SystemMemory });
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

        private void SetKeyPress(ChipKeys key)
        {
            if (key != ChipKeys.None)
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

        public void Step()
        {
            byte a = m_Memory.GetByte(m_CodeEngine.PC);
            byte b = m_Memory.GetByte(m_CodeEngine.PC + 1);
            ushort data = Tools.MakeShort(a, b);
            ChipOpcodes opcode = Disassembler.DecodeInstruction(data);
            ChipInstruction inst = new ChipInstruction(data, opcode);
            inst.Address = m_CodeEngine.PC;
            m_CodeEngine.IncrementPC();
            m_CodeEngine.Call(inst);
        }

        public void Run(int entryAddress)
        {
            m_CodeEngine.PC = entryAddress;
            m_ThreadCPU.Start();
        }

        private void StartExecutionCycle()
        {
            Thread.CurrentThread.Name = "CPU Thread";
            Thread.BeginThreadAffinity();

            while (m_CodeEngine.PC < m_Memory.Size)
            {
                if (!m_RequestCPUStop)
                {
                    if (m_Paused)
                    {
                        m_CPUWait.WaitOne();
                    }

                    Thread.BeginCriticalRegion();

                    Step();

                    Thread.EndCriticalRegion();
                }
                else
                {
                    break;
                }


                Thread.Sleep(2);
            }

            Thread.EndThreadAffinity();

            m_CPUEndWait.Set();
        }

        protected override bool Boot()
        {
            Console.WriteLine("Booting...");

            if (CurrentAudioDevice == null)
                throw new NullReferenceException();

            if (CurrentGraphicsDevice == null)
                throw new NullReferenceException();

            if (CurrentInputDevice == null)
                throw new NullReferenceException();

            m_Processor = new Processor(m_Engine);

            AttachDeviceCallbacks();

            CurrentAudioDevice.Initialize();
            CurrentGraphicsDevice.Initialize();
            CurrentInputDevice.Initialize();
            m_Processor.Initialize();

            if (!LoadFont())
                return false;

            if (!LoadRom())
                return false;

            m_Processor.Run(CODE_OFFSET);

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
            int pos = CODE_OFFSET;

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

        public event EventHandler ScreenClear
        {
            add { m_CodeEngine.ScreenClear += value; }
            remove { m_CodeEngine.ScreenClear -= value; }
        }

        public event EventHandler<PixelSetEventArgs> PixelSet
        {
            add { m_CodeEngine.PixelSet += value; }
            remove { m_CodeEngine.PixelSet -= value; }
        }

        public event EventHandler<BeepEventArgs> Beep
        {
            add { m_CodeEngine.Beep += value; }
            remove { m_CodeEngine.Beep -= value; }
        }

        public event EventHandler KeyPressWait
        {
            add { m_CodeEngine.KeyPressWait += value; }
            remove { m_CodeEngine.KeyPressWait -= value; }
        }

        public AudioDevice CurrentAudioDevice
        {
            get { return this.m_AudioDevice; }
            set { this.m_AudioDevice = value; }
        }

        public InputDevice CurrentInputDevice
        {
            get { return this.m_InputDevice; }
            set { this.m_InputDevice = value; }
        }

        public GraphicsDevice CurrentGraphicsDevice
        {
            get { return this.m_GraphicsDevice; }
            set { this.m_GraphicsDevice = value; }
        }


        // ----------------------------
        // Internal Methods
        // ----------------------------

        private void AttachDeviceCallbacks()
        {
            m_Processor.Beep -= new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            m_Processor.PixelSet -= new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            m_Processor.ScreenClear -= new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision -= new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress -= new KeyStateHandler(CurrentInputDevice_OnKeyPress);

            m_Processor.Beep += new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            m_Processor.PixelSet += new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            m_Processor.ScreenClear += new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision += new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress += new KeyStateHandler(CurrentInputDevice_OnKeyPress);
        }

        private void CurrentInputDevice_OnKeyPress(object sender, ChipKeys key)
        {
            m_Processor.SetKeyPress(key);
        }

        private void CurrentGraphicsDevice_OnPixelCollision(object sender, EventArgs e)
        {
            m_Processor.SetCollision();
        }

        private void CurrentProcessor_OnScreenClear(object sender, EventArgs e)
        {
            this.CurrentGraphicsDevice.ClearScreen();
        }

        private void CurrentProcessor_OnPixelSet(object sender, PixelSetEventArgs e)
        {
            CurrentGraphicsDevice.SetPixel(e.X, e.Y);
        }

        private void m_CodeEngine_OnKeyPressWait(object sender, EventArgs e)
        {
            this.m_KeyWait.WaitOne();
        } 
    }
}
