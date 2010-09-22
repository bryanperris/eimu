using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Plugins;
using Eimu.Core.Devices;

namespace Eimu.NullDevices
{
    [PluginInfo("Null Input Plugin", "1.0", "Omegadox", "skips input")]
    public sealed class NullInputDevice : InputDevice, IPlugin
    {
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

        public void Initialize()
        {

        }

        public void Shutdown()
        {

        }

        public void SetState(Eimu.Core.RunState state)
        {

        }

        #endregion
    }
}
