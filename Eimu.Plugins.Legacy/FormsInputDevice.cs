using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Eimu.Core.Devices;
using Eimu.Plugins;

namespace Eimu
{
    [PluginInfo("Simple Input Plugin", "1.0", "Omegadox", "Uses keys QWERTYASDFGHZXCVB for input")]
    public class FormsInputDevice : InputDevice, IPlugin
    {
        private Form m_Window;

        public override void Initialize()
        {
            m_Window = (Form)Form.FromHandle(PluginManager.WindowHandle);
            m_Window.KeyDown += new KeyEventHandler(m_Window_KeyDown);
            m_Window.KeyUp += new KeyEventHandler(m_Window_KeyUp);
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
                case Keys.Q: key = ChipKeys.Zero; break;
                case Keys.W: key = ChipKeys.One; break;
                case Keys.E: key = ChipKeys.Two; break;
                case Keys.R: key = ChipKeys.Three; break;
                case Keys.T: key = ChipKeys.Four; break;
                case Keys.Y: key = ChipKeys.Five; break;
                case Keys.A: key = ChipKeys.Six; break;
                case Keys.S: key = ChipKeys.Seven; break;
                case Keys.D: key = ChipKeys.Eight; break;
                case Keys.G: key = ChipKeys.Nine; break;
                case Keys.H: key = ChipKeys.Ten; break;
                case Keys.F: key = ChipKeys.A; break;
                case Keys.Z: key = ChipKeys.B; break;
                case Keys.X: key = ChipKeys.C; break;
                case Keys.C: key = ChipKeys.D; break;
                case Keys.V: key = ChipKeys.E; break;
                case Keys.B: key = ChipKeys.F; break;
                default: break;
            }

            KeyPress(key);
        }

        public override void Shutdown()
        {
        }

        public override void SetPauseState(bool paused)
        {
        }

        #region IPlugin Members

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

        #endregion
    }
}
