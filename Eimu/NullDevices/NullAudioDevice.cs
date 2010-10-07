using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu;
using Eimu.Core.Devices;
using Eimu.Plugins;

namespace Eimu.NullDevices
{
    [PluginInfo("Null Audio Plugin", "1.0", "Omegadox", "Skips audio output")]
    public class NullAudioDevice : AudioDevice, IPlugin
    {
        public override void Beep(int duration)
        {
            return;
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
            return;
        }

        public string GetOption(string name)
        {
            return "";
        }

        #endregion

        public override void Initialize()
        {
        }

        public override void Shutdown()
        {
        }

        public override void SetPauseState(bool paused)
        {
        }
    }
}
