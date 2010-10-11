/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using Eimu.Core.CPU;
using Eimu.Core.Devices;

namespace Eimu.Core
{
    /// <summary>
    /// A Chip-8 virtual machine which provides managment of the system and inter-communication between devices and CPU.
    /// </summary>
    [Serializable]
    public sealed class VirtualMachine
    {
        public const int FONT_SIZE = 5;

        public static readonly byte[] FONTROM = {
         0xF0, 0x90, 0x90, 0x90, 0xF0,   //0
	     0x20, 0x60, 0x20, 0x20, 0x70,   //1
	     0xF0, 0x10, 0xF0, 0x80, 0xF0,   //2
	     0xF0, 0x10, 0xF0, 0x10, 0xF0,   //3
	     0x90, 0x90, 0xF0, 0x10, 0x10,   //4
	     0xF0, 0x80, 0xF0, 0x10, 0xF0,   //5
	     0xF0, 0x80, 0xF0, 0x90, 0xF0,   //6
	     0xF0, 0x10, 0x20, 0x40, 0x40,   //7
	     0xF0, 0x90, 0xF0, 0x90, 0xF0,   //8
	     0xF0, 0x90, 0xF0, 0x10, 0xF0,   //9
	     0xF0, 0x90, 0xF0, 0x90, 0x90,   //A
	     0xE0, 0x90, 0xE0, 0x90, 0xE0,   //B
	     0xF0, 0x80, 0x80, 0x80, 0xF0,   //C
	     0xE0, 0x90, 0x90, 0x90, 0xE0,   //D
	     0xF0, 0x80, 0xF0, 0x80, 0xF0,   //E
	     0xF0, 0x80, 0xF0, 0x80, 0x80,}; //F

        private RunState m_State;

        public VirtualMachine()
        {
            MachineMemory = new Memory();
            LoadFont();
        }

        public void LoadROM(Stream source)
        {
            if (!source.CanRead)
                throw new IOException("source not readable!");

            if (source.Length > MachineMemory.Size - 0x1FF)
                //throw new ArgumentException("source is bigger than memory size!");

            source.Position = 0;
            int read = -1;

            // Read source into RAM
            for (int i = 0x200; i < MachineMemory.Size; i++)
            {
                if (-1 != (read = source.ReadByte()))
                {
                    MachineMemory[i] = (byte)read;
                }
                else
                {
                    MachineMemory[i] = 0;
                }
            }
        }

        private void LoadFont()
        {
            for (int i = 0; i < 0x1FF; i++)
            {
                if (i < FONTROM.Length)
                    this.MachineMemory[i] = FONTROM[i];
                else
                    this.MachineMemory[i] = 0;
            }
        }

        public void Start()
        {
            if (CurrentAudioDevice == null)
                throw new NullReferenceException();

            if (CurrentGraphicsDevice == null)
                throw new NullReferenceException();

            if (CurrentInputDevice == null)
                throw new NullReferenceException();

            if (CurrentProcessor == null)
                throw new NullReferenceException();

            if (MachineMemory == null)
                throw new NullReferenceException();

            CurrentProcessor.SetMemory(this.MachineMemory);

            AttachDeviceCallbacks();

            ((IDevice)CurrentAudioDevice).Initialize();
            ((IDevice)CurrentGraphicsDevice).Initialize();
            ((IDevice)CurrentInputDevice).Initialize();
            ((IDevice)CurrentProcessor).Initialize();

            CurrentProcessor.StartExecution();

            m_State = RunState.Running;
        }

        public void Stop()
        {
            if (m_State != RunState.Stopped)
            {
                m_State = RunState.Stopped;

                ((IDevice)CurrentProcessor).Shutdown();
                ((IDevice)CurrentInputDevice).Shutdown();
                ((IDevice)CurrentAudioDevice).Shutdown();
                ((IDevice)CurrentGraphicsDevice).Shutdown();

                DetachDeviceCallbacks();
            }
        }

        public void Restart()
        {
            Thread.Sleep(100);
            Stop();
            Start();
        }

        public void SetPause(bool paused)
        {
            if (m_State == RunState.Running || m_State == RunState.Paused)
            {
                ((IDevice)CurrentProcessor).SetPauseState(paused);
                ((IDevice)CurrentAudioDevice).SetPauseState(paused);
                ((IDevice)CurrentGraphicsDevice).SetPauseState(paused);
                ((IDevice)CurrentInputDevice).SetPauseState(paused);

                if (paused)
                    m_State = RunState.Paused;
                else
                    m_State = RunState.Running;
            }
        }

        public RunState MachineRunState
        {
            get { return this.m_State; }
        }

        public AudioDevice CurrentAudioDevice { get; set; }

        public GraphicsDevice CurrentGraphicsDevice { get; set; }

        public InputDevice CurrentInputDevice { get; set; }

        public Processor CurrentProcessor { get; set; }

        public Memory MachineMemory { get; set; }

        private void AttachDeviceCallbacks()
        {
            CurrentProcessor.OnBeep += new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            CurrentProcessor.OnPixelSet += new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            CurrentProcessor.OnScreenClear += new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision += new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress += new KeyStateHandler(CurrentInputDevice_OnKeyPress);
        }

        private void DetachDeviceCallbacks()
        {
            CurrentProcessor.OnBeep -= new EventHandler<BeepEventArgs>(CurrentProcessor_OnBeep);
            CurrentProcessor.OnPixelSet -= new EventHandler<PixelSetEventArgs>(CurrentProcessor_OnPixelSet);
            CurrentProcessor.OnScreenClear -= new EventHandler(CurrentProcessor_OnScreenClear);
            CurrentGraphicsDevice.OnPixelCollision -= new EventHandler(CurrentGraphicsDevice_OnPixelCollision);
            CurrentInputDevice.OnKeyPress -= new KeyStateHandler(CurrentInputDevice_OnKeyPress);
        }

        void CurrentInputDevice_OnKeyPress(object sender, ChipKeys key)
        {
            this.CurrentProcessor.SetKeyPress(key);
        }

        void CurrentGraphicsDevice_OnPixelCollision(object sender, EventArgs e)
        {
            this.CurrentProcessor.SetCollision();
        }

        void CurrentProcessor_OnScreenClear(object sender, EventArgs e)
        {
            this.CurrentGraphicsDevice.ClearScreen();
        }

        void CurrentProcessor_OnPixelSet(object sender, PixelSetEventArgs e)
        {
            this.CurrentGraphicsDevice.SetPixel(e.X, e.Y);
        }

        void CurrentProcessor_OnBeep(object sender, BeepEventArgs e)
        {
            this.CurrentAudioDevice.Beep(e.Duration);
        }
    }
}
