using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Plugins;
using Eimu.Core.Devices;

namespace Eimu.NullDevices
{
    [PluginInfo("Null Graphics Plugin", "1.0", "Omegadox", "skips video drawing, but still supports collision dectection")]
    public sealed class NullGraphicsDevice : GraphicsDevice, IPlugin
    {
        private bool[] m_FakeVideoBuffer;

        public NullGraphicsDevice()
        {
            m_FakeVideoBuffer = new bool[NullGraphicsDevice.RESOLUTION_WIDTH * NullGraphicsDevice.RESOLUTION_HEIGHT];
        }

        public override void SetPixel(int x, int y)
        {
            if (!(m_FakeVideoBuffer[x * (y + 1)] ^= true))
                SetCollision();
        }

        public override void ClearScreen()
        {
            m_FakeVideoBuffer.Initialize();
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
