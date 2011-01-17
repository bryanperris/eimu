using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eimu.Core.Systems.Chip8
{
    public sealed class C8Machine : VirtualMachine
    {
        public const int FONT_SIZE = 5;
        public const int MEMORY_SIZE = 4096;
        public const int CODE_OFFSET = 0x200;
        public const int PROGRAM_ENTRY_POINT = 0x200;
        public AudioDevice CurrentAudioDevice { get; set; }
        public GraphicsDevice CurrentGraphicsDevice { get; set; }
        public InputDevice CurrentInputDevice { get; set; }
        private Processor m_Processor;
        private Stream m_FontSource;
        private CodeEngine m_Engine;

        public C8Machine()
        {
            SystemMemory = new Memory(MEMORY_SIZE);
        }

        public void SetFontResource(Stream source)
        {
            m_FontSource = source;
        }

        public void SetCodeEngineType<TCodeEngine>() where TCodeEngine : CodeEngine
        {
            m_Engine = (CodeEngine)Activator.CreateInstance(typeof(TCodeEngine), new object[] { SystemMemory });
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

        void CurrentProcessor_OnBeep(object sender, BeepEventArgs e)
        {
            this.CurrentAudioDevice.Beep(e.Duration);
        }

        protected override void OnMachineState(RunState state)
        {
            switch (state)
            {
                case RunState.Stopped:
                    {
                        m_Processor.Shutdown();
                        CurrentInputDevice.Shutdown();
                        CurrentAudioDevice.Shutdown();
                        CurrentGraphicsDevice.Shutdown();

                        if (MediaSource != null)
                            MediaSource.Close();

                        if (m_FontSource != null)
                            m_FontSource.Close();

                        break;
                    }
                case RunState.Paused:
                    {
                        CurrentAudioDevice.SetPauseState(true);
                        CurrentGraphicsDevice.SetPauseState(true);
                        CurrentInputDevice.SetPauseState(true);
                        m_Processor.SetPauseState(true);
                        break;
                    }
                case RunState.Running:
                    {
                        CurrentAudioDevice.SetPauseState(false);
                        CurrentGraphicsDevice.SetPauseState(false);
                        CurrentInputDevice.SetPauseState(false);
                        m_Processor.SetPauseState(false);
                        break;
                    }

                default: break;
            }
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

        public Processor CurrentProcessor
        {
            get { return this.m_Processor; }
        }
    }
}
