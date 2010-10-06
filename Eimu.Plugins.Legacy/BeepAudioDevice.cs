using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Eimu;
using Eimu.Core.Devices;
using Eimu.Plugins;

namespace Eimu.Plugins.Legacy
{
    [PluginInfo("Legacy Audio Plugin", "1.0", "Omegadox", "Uses system beep")]
    public class BeepAudioDevice : AudioDevice, IPlugin
    {
        Thread m_Thread;

        public BeepAudioDevice()
        {
            m_Thread = new Thread(new ThreadStart(DoBeep));
            m_Thread.IsBackground = false;
        }

        public override void Beep()
        {
            m_Thread.Start();
        }

        private void DoBeep()
        {
            System.Console.Beep(100, 50);
        }

        #region IPlugin Members

        public void ShowConfigDialog()
        {
            throw new NotImplementedException();
        }

        public string[] GetOptionsList()
        {
            return null;
        }

        public void SetOption(string name, string value)
        {
        }

        public string GetOption(string name)
        {
            return "";
        }

        public override void Initialize()
        {
        }

        public override void Shutdown()
        {
        }

        public override void SetPauseState(bool paused)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
