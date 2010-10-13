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
    public abstract class GraphicsDevice : IDevice
    {

        public const int RESOLUTION_WIDTH = 64;
        public const int RESOLUTION_HEIGHT = 32;
        public const int SPRITE_WIDTH = 8;
        public const int SPRITE_HEIGHT = 1;

        private bool[] m_Buffer;

        public GraphicsDevice()
        {
            m_Buffer = new bool[(RESOLUTION_WIDTH + 1) * (RESOLUTION_HEIGHT + 1)];
        }

        public event EventHandler OnPixelCollision;

        public abstract void OnPixelSet(int x, int y, bool on);

        public abstract void OnScreenClear();

        public void ClearScreen()
        {
            Array.Clear(this.m_Buffer, 0, this.m_Buffer.Length);
            OnScreenClear();
        }

        public void SetPixel(int x, int y)
        {
            // Wrapping
            if (x > GraphicsDevice.RESOLUTION_WIDTH)
                x -= GraphicsDevice.RESOLUTION_WIDTH;

            if (x < 0)
                x += GraphicsDevice.RESOLUTION_WIDTH;

            if (y > GraphicsDevice.RESOLUTION_HEIGHT)
                y -= GraphicsDevice.RESOLUTION_HEIGHT;

            if (y < 0)
                y += GraphicsDevice.RESOLUTION_HEIGHT;

            bool on = GetPixel(x, y) ^ true;

            if (!on)
                SetCollision();

            m_Buffer[GetBufferPosition(x,y)] = on;


            OnPixelSet(x, y, on);

            // src 0 ^ 1 = 1 : Fill White
            // src 1 ^ 1 = 0 : Make Black, Set Collision
        }

        public bool GetPixel(int x, int y)
        {
            return m_Buffer[GetBufferPosition(x, y)];
        }

        public int GetBufferPosition(int x, int y)
        {
            return ((y * GraphicsDevice.RESOLUTION_WIDTH) + x);
        }

        protected void SetCollision()
        {
            if (OnPixelCollision != null)
                OnPixelCollision(this, new EventArgs());
        }

        protected bool[] InternalBuffer
        {
            get { return this.m_Buffer; }
        }

        #region IDevice Members

        public abstract void Initialize();

        public abstract void Shutdown();

        public abstract void SetPauseState(bool paused);

        #endregion
    }
}
