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
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace Eimu.Core.Systems.Chip8
{
    [Serializable]
    public sealed class Processor : Device
    {
        private bool m_Paused = false;
        private EventWaitHandle m_CPUWait;
        private EventWaitHandle m_KeyWait;
        private EventWaitHandle m_CPUEndWait;
        private CodeEngine m_CodeEngine;
        private bool m_RequestCPUStop;
        private Thread m_ThreadCPU;
        private Memory m_Memory;

        public Processor(CodeEngine codeEngine)
        {
            this.m_CodeEngine = codeEngine;
            this.m_Memory = codeEngine.CurrentMemory;
            Initialize();
        }

        public void Step()
        {
            byte a = m_Memory.GetByte(m_CodeEngine.PC);
            byte b = m_Memory.GetByte(m_CodeEngine.PC + 1);
            ushort data = Tools.MakeShort(a, b);
            ChipOpcodes opcode = Disassembler.DecodeInstruction(data);
            ChipInstruction inst = new ChipInstruction(data, opcode);
            m_CodeEngine.IncrementPC();
            m_CodeEngine.Call(inst);
        }

        public void Run()
        {
            m_ThreadCPU.Start();
        }

        public override void Shutdown()
        {
            m_CodeEngine.Shutdown();
            m_CodeEngine.Beep -= new EventHandler<BeepEventArgs>(m_CodeEngine_OnBeep);
            m_CodeEngine.PixelSet -= new EventHandler<PixelSetEventArgs>(m_CodeEngine_OnPixelSet);
            m_CodeEngine.ScreenClear -= new EventHandler(m_CodeEngine_OnScreenClear);
            m_CodeEngine.KeyPressWait -= new EventHandler(m_CodeEngine_OnKeyPressWait);
            m_CPUWait.Set();
            m_KeyWait.Set();
            m_RequestCPUStop = true;
            m_CPUEndWait.WaitOne();
            m_CPUEndWait.Reset();
        }

        public override void Initialize()
        {
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

        public void SetCollision()
        {
            m_CodeEngine.VRegisters[0xF] = 1;
        }

        public override void SetPauseState(bool paused)
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

        private void StartExecutionCycle()
        {
            Thread.CurrentThread.Name = "CPU Thread";

            while (m_CodeEngine.PC < m_Memory.Size)
            {
                if (!m_RequestCPUStop)
                {
                    if (m_Paused)
                    {
                        m_CPUWait.WaitOne();
                    }

                    Step();
                }
                else
                {
                    break;
                }

                Thread.Sleep(2);
            }

            m_CPUEndWait.Set();
        }

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
            add { m_CodeEngine.Beep+= value; }
            remove { m_CodeEngine.Beep -= value; }
        }

        public event EventHandler KeyPressWait
        {
            add { m_CodeEngine.KeyPressWait += value; }
            remove { m_CodeEngine.KeyPressWait -= value; }
        }

        private void m_CodeEngine_OnScreenClear(object sender, EventArgs e)
        {

        }

        private void m_CodeEngine_OnPixelSet(object sender, PixelSetEventArgs e)
        {

        }

        private void m_CodeEngine_OnBeep(object sender, BeepEventArgs e)
        {
        }

        private void m_CodeEngine_OnKeyPressWait(object sender, EventArgs e)
        {
            this.m_KeyWait.WaitOne();
        } 
    }
}
