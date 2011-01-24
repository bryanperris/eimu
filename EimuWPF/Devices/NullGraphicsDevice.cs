/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

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
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;

namespace EimuWPF.Devices
{
    [PluginInfo("Null Graphics", "1.0", "Omegadox", "skips video drawing, but still supports collision dectection")]
    public sealed class NullGraphicsDevice : GraphicsDevice, IPlugin
    {
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

        protected override void OnInit()
        {
        }

        protected override void OnShutdown()
        {
        }

        protected override void OnPauseStateChange(bool paused)
        {
        }

        protected override void OnPixelSet(int x, int y, bool on)
        {

        }

        protected override void OnScreenClear()
        {
        }
    }
}
