using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core.Devices;
using System.Threading;

namespace Eimu
{
    public class BeepAudioDevice : AudioDevice
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
            System.Console.Beep(100, 1);
        }
    }
}
