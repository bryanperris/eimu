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
using Eimu.Core.Systems.Chip8X;
using Eimu.Core.Systems.Chip8X.Interfaces;

namespace Eimu.Devices
{
    public sealed class NullGraphicsDevice : Renderer, IWinFormAttachment
    {


        #region IWinFormAttachment Members

        public void SetPanelHandle(IntPtr handle)
        {
        }

        public void SetWindowHandle(IntPtr handle)
        {
        }

        public bool UseDoubleBugger
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        #endregion

        public override void Initialize()
        {
            
        }

        public override void Shutdown()
        {
            
        }

        public override void SetPause(bool paused)
        {
            
        }

        public override void Update(VideoInterface currentInterface)
        {
        }
    }
}
