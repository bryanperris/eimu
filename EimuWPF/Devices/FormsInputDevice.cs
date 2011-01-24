/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  Omegadox, http://code.google.com/p/eimu

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
using System.Windows.Forms;
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;

namespace EimuWPF.Devices
{
    [PluginInfo("Default Input", "1.0", "Omegadox", "Uses keys QWE\nASD\nZXC\nRFV\nTGB for input")]
    public class FormsInputDevice : InputDevice, IPlugin
    {
        private Control m_Control;

        protected override void OnInit()
        {
            m_Control = (Control)Form.FromHandle(PluginManager.RenderContext);
            m_Control.KeyDown += new KeyEventHandler(m_Window_KeyDown);
            m_Control.KeyUp += new KeyEventHandler(m_Window_KeyUp);
        }

        void m_Window_KeyUp(object sender, KeyEventArgs e)
        {
            KeyPress(ChipKeys.None);
        }

        void m_Window_KeyDown(object sender, KeyEventArgs e)
        {
            ChipKeys key = ChipKeys.None;

            switch (e.KeyCode)
            {
                case Keys.Q: key = ChipKeys.One; break;
                case Keys.W: key = ChipKeys.Two; break;
                case Keys.E: key = ChipKeys.Three; break;
                case Keys.R: key = ChipKeys.A; break;
                case Keys.T: key = ChipKeys.D; break;
                case Keys.A: key = ChipKeys.Four; break;
                case Keys.S: key = ChipKeys.Five; break;
                case Keys.D: key = ChipKeys.Six; break;
                case Keys.G: key = ChipKeys.E; break;
                case Keys.F: key = ChipKeys.B; break;
                case Keys.Z: key = ChipKeys.Seven; break;
                case Keys.X: key = ChipKeys.Eight; break;
                case Keys.C: key = ChipKeys.Nine; break;
                case Keys.V: key = ChipKeys.C; break;
                case Keys.B: key = ChipKeys.F; break;
                case Keys.Space: key = ChipKeys.Zero; break;
                default: break;
            }

            KeyPress(key);
        }

        protected override void OnShutdown()
        {
        }

        protected override void OnPauseStateChange(bool paused)
        {
        }

        public void ShowConfigDialog()
        {
            throw new NotImplementedException();
        }

        public string[] GetOptionsList()
        {
            throw new NotImplementedException();
        }

        public void SetOption(string name, string value)
        {
            throw new NotImplementedException();
        }

        public string GetOption(string name)
        {
            throw new NotImplementedException();
        }
    }
}
