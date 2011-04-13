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
using System.Windows.Media;
using Eimu.Core.Systems.Chip8X;

namespace Eimu.Devices
{
    [Serializable]
    public abstract class Renderer : Device
    {
        private Color m_BackColor;
        private Color m_ForeColor;

        public abstract void Update(VideoInterface currentInterface);

        public Color BackgroundColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        public Color ForegroundColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }
    }
}
