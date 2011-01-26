using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Eimu.Core.Systems.Chip8
{
    public class C8Machine : VirtualMachine
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
        private AudioDevice m_AudioDevice;
        private GraphicsDevice m_GraphicsDevice;
        private CodeEngine m_CodeEngine;


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
                m_CodeEngine.Shutdown();
                m_CPUWait.Set();
                m_KeyWait.Set();
                m_RequestCPUStop = true;
                CurrentAudioDevice.Shutdown();
                CurrentGraphicsDevice.Shutdown();
            }

        }

        public void InitCore<TCodeEngine>() where TCodeEngine : CodeEngine
        {
            SystemMemory = new Memory(MEMORY_SIZE);
            m_CodeEngine = (CodeEngine)Activator.CreateInstance(typeof(TCodeEngine), new object[] { SystemMemory });
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

        public void SetKeyPress(ChipKeys key)
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
            byte a = SystemMemory.GetByte(m_CodeEngine.PC);
            byte b = SystemMemory.GetByte(m_CodeEngine.PC + 1);
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

            while (m_CodeEngine.PC < SystemMemory.Size)
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

            if (CurrentAudioDevice == null) throw new NullReferenceException();
            if (CurrentGraphicsDevice == null) throw new NullReferenceException();
            m_CPUWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_KeyWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_CPUEndWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RequestCPUStop = false;
            m_Paused = false;
            m_ThreadCPU = new Thread(new ThreadStart(StartExecutionCycle));
            m_ThreadCPU.IsBackground = true;
            m_CodeEngine.Init();
            AttachDeviceCallbacks();
            CurrentAudioDevice.Initialize();
            CurrentGraphicsDevice.Initialize();
            CurrentGraphicsDevice.ClearScreen();
            if (!LoadFont()) return false;
            if (!LoadRom()) return false;
            Run(CODE_OFFSET);
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

        private void AttachDeviceCallbacks()
        {
            m_CodeEngine.Beep -= new EventHandler<BeepEventArgs>(OnBeep);
            m_CodeEngine.Beep += new EventHandler<BeepEventArgs>(OnBeep);

            m_CodeEngine.PixelSet -= new EventHandler<PixelSetEventArgs>(OnPixelSet);
            m_CodeEngine.PixelSet += new EventHandler<PixelSetEventArgs>(OnPixelSet);

            m_CodeEngine.ScreenClear -= new EventHandler(OnScreenClear);
            m_CodeEngine.ScreenClear += new EventHandler(OnScreenClear);

            CurrentGraphicsDevice.OnPixelCollision -= new EventHandler(OnPixelCollision);
            CurrentGraphicsDevice.OnPixelCollision += new EventHandler(OnPixelCollision);
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

        private void OnBeep(object sender, BeepEventArgs e)
        {
            m_AudioDevice.Beep(e.Duration);
        }
    }
}
