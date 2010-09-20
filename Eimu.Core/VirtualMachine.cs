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
using Eimu.Core.CPU;
using Eimu.Core.Devices;
using System.IO;
using System.Threading;

namespace Eimu.Core
{
    public sealed class VirtualMachine
    {
        // ------------------------------------------
        // Hardware Components
        // ------------------------------------------
        private GraphicsDevice m_DeviceGraphics;
        private InputDevice m_DeviceInput;
        private AudioDevice m_DeviceAudio;
        private Memory m_Memory;
        private Processor m_CPU;

        // ------------------------------------------
        // Machine state control
        // ------------------------------------------
        private RunState m_State;
        private Stream m_ProgramSource;

        // ------------------------------------------
        // Thread runners
        // ------------------------------------------
        private Thread m_ThreadCPU;

        public VirtualMachine(MachineParamaters paramaters)
        {
            this.m_ProgramSource = paramaters.RomSource;
            this.m_DeviceAudio = paramaters.Audio;
            this.m_DeviceGraphics = paramaters.Graphics;
            this.m_DeviceInput = paramaters.Input;
            this.m_State = RunState.Stopped;
            this.m_Memory = new Memory();
            this.m_CPU = paramaters.CPU;
            this.m_CPU.SetMemory(m_Memory);
        }
    }
}
