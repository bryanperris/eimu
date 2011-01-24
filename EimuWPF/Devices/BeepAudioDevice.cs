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
using System.Threading;
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;

namespace EimuWPF.Devices
{
    [PluginInfo("Default Beep", "1.0", "Omegadox", "Uses system beep to make noise")]
    public class BeepAudioDevice : AudioDevice, IPlugin
    {
        Thread m_Thread;
        private int m_Duration;

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
            System.Console.Beep(260, m_Duration + 100);
        }

        public void ShowConfigDialog()
        {
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

        protected override void OnInit()
        {
        }

        protected override void OnShutdown()
        {
        }

        protected override void OnPauseStateChange(bool paused)
        {
        }
    }
}
