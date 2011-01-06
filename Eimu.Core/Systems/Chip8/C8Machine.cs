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
        public const string FONT_PATH = "./sys/c8fnt.bin";
        public const int PROGRAM_ENTRY_POINT = 0x200;
        public AudioDevice CurrentAudioDevice { get; set; }
        public GraphicsDevice CurrentGraphicsDevice { get; set; }
        public InputDevice CurrentInputDevice { get; set; }
        public Processor CurrentProcessor { get; set; }

        protected override bool Boot()
        {
            Console.WriteLine("Booting...");

            if (CurrentAudioDevice == null)
                throw new NullReferenceException();

            if (CurrentGraphicsDevice == null)
                throw new NullReferenceException();

            if (CurrentInputDevice == null)
                throw new NullReferenceException();

            if (CurrentProcessor == null)
                throw new NullReferenceException();

            AttachDeviceCallbacks();

            CurrentAudioDevice.Initialize();
            CurrentGraphicsDevice.Initialize();
            CurrentInputDevice.Initialize();
            CurrentProcessor.Initialize();

            if (!LoadFont())
                return false;

            if (!LoadRom())
                return false;

            CurrentProcessor.Run();

            return true;
        }

        private void AttachDeviceCallbacks()
        {
            CurrentProcessor.Beep -= new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            CurrentProcessor.PixelSet -= new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            CurrentProcessor.ScreenClear -= new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision -= new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress -= new KeyStateHandler(CurrentInputDevice_OnKeyPress);

            CurrentProcessor.Beep += new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            CurrentProcessor.PixelSet += new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            CurrentProcessor.ScreenClear += new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision += new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress += new KeyStateHandler(CurrentInputDevice_OnKeyPress);
        }

        private void CurrentInputDevice_OnKeyPress(object sender, ChipKeys key)
        {
            this.CurrentProcessor.SetKeyPress(key);
        }

        private void CurrentGraphicsDevice_OnPixelCollision(object sender, EventArgs e)
        {
            this.CurrentProcessor.SetCollision();
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
                        CurrentProcessor.Shutdown();
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
                        CurrentProcessor.SetPauseState(true);
                        break;
                    }
                case RunState.Running:
                    {
                        CurrentAudioDevice.SetPauseState(false);
                        CurrentGraphicsDevice.SetPauseState(false);
                        CurrentInputDevice.SetPauseState(false);
                        CurrentProcessor.SetPauseState(false);
                        break;
                    }

                default: break;
            }
        }

        private bool LoadFont()
        {
            try
            {
                FileStream fontfile = new FileStream(FONT_PATH, FileMode.Open, FileAccess.Read, FileShare.Read);

                int read;
                int pos = 0;

                while ((read = fontfile.ReadByte()) != -1)
                {
                    this.SystemMemory[pos++] = (byte)read;
                }

                fontfile.Close();

                return true;
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Font file is missing!");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("can't access font file!");
                return false;
            }
        }

        private bool LoadRom()
        {

            if (this.MediaSource == null)
                return false;

            if (!this.MediaSource.CanRead)
                return false;

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
    }
}
