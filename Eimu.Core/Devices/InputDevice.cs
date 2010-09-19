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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Devices
{
    public enum ChipKeys : byte
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        A = 11,
        B = 12,
        C = 13,
        D = 14,
        E = 15,
        F = 16
    }

    public delegate void KeyStateHandler (object sender, ChipKeys key);

    public abstract class InputDevice
    {
        public event KeyStateHandler OnKeyPress;
        public event KeyStateHandler OnKeyRelease;

        protected void KeyPress(ChipKeys key)
        {
            if (OnKeyPress != null)
                OnKeyPress(this, key);
        }

        protected void KeyRelease(ChipKeys key)
        {
            if (OnKeyRelease != null)
                OnKeyRelease(this, key);
        }
    }
}
