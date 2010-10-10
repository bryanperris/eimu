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
    [PluginInfo("Sytem Beep Plugin (Legacy)", "1.0", "Omegadox", "Uses system beep to make noise")]
    public class BeepAudioDevice : AudioDevice, IPlugin
    {
        Thread m_Thread;
        private int m_Duration;

        public BeepAudioDevice()
        {

        }

        public override void Beep(int duruation)
        {
            m_Duration = duruation;
            m_Thread = new Thread(new ThreadStart(DoBeep));
            m_Thread.Name = "Beep thread";
            m_Thread.IsBackground = false;
            m_Thread.Start();
        }

        private void DoBeep()
        {
            System.Console.Beep(250, m_Duration + 100);
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
        }

        #endregion
    }
}
