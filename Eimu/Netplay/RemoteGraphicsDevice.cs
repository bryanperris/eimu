using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;

using Eimu.Core.Devices;

namespace Eimu.Netplay
{
    public class RemoteGraphicsDevice : GraphicsDevice
    {
        //bool isHost = false;
        GraphicsDevice m_RealLocalDevice;
        GraphicsDevice m_ProxyDevice;
        string m_Url = "127.0.0.1";

        public RemoteGraphicsDevice(GraphicsDevice realLocalDevice)
        {
            this.m_RealLocalDevice = realLocalDevice;
            m_ProxyDevice = (GraphicsDevice)RemotingServices.Connect(this.GetType(), m_Url);
        }

        public override void SetPixel(int x, int y)
        {
            m_RealLocalDevice.SetPixel(x, y);
        }

        protected override void OnPixelSet(int x, int y, bool on)
        {
            throw new NotImplementedException();
        }

        protected override void OnScreenClear()
        {
            throw new NotImplementedException();
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
    }
}
