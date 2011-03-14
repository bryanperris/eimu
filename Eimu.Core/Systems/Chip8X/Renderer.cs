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
using System.Runtime.InteropServices;

namespace Eimu.Core.Systems.Chip8X
{
    [ComVisible(true)]
    public abstract class Renderer : Device
    {
        public const int StandardResolutionX = 64;
        public const int StandardResolutionY = 32;
        public const int SuperResolutionX = 128;
        public const int SuperResolutionY = 64;
        public const int StandardSpriteSize = 8;
        public const int SuperSpriteSize = 16;
        private int m_ResX;
        private int m_ResY;
        private RgbColor m_BackColor;
        private RgbColor m_ForeColor;
        private bool[] m_Buffer;
        public event EventHandler OnPixelCollision;
        private bool m_DisableWrappingX;
        private bool m_DisableWrappingY;
        private bool m_EnableHighres;
        private bool m_EnableAntiFlickerHack;

        private void CreateFakeBuffer()
        {
           m_ResX = StandardResolutionX;
           m_ResY = StandardResolutionY;

            if (m_EnableHighres)
            {
                m_ResX = SuperResolutionX;
                m_ResY = SuperResolutionY;
            }
            m_Buffer = new bool[(m_ResX + 1) * (m_ResY + 1)];
        }

        public virtual void ClearScreen()
        {
            Array.Clear(this.m_Buffer, 0, this.m_Buffer.Length);
            OnScreenClear();
        }

        public virtual void SetPixel(int x, int y)
        {
            if (!m_DisableWrappingX)
                x &= (m_ResX - 1);

            if (!m_DisableWrappingY)
                y &= (m_ResY - 1);

            bool on = GetPixel(x, y) ^ true;

            if (!on)
                OnSetCollision();

            m_Buffer[GetBufferPosition(x, y)] = on;

            if (!on && m_EnableAntiFlickerHack)
                return;

            OnPixelSet(x, y, on);

            // src 0 ^ 1 = 1 : Fill White
            // src 1 ^ 1 = 0 : Make Black, Set Collision
        }

        public virtual bool GetPixel(int x, int y)
        {
            return m_Buffer[GetBufferPosition(x, y)];
        }

        public void SetSuperMode(bool enabled)
        {
            m_EnableHighres = enabled;
            Shutdown();
            Initialize();
        }

        private void ScrollPixelsDown(int n)
        {
            bool[] arr = new bool[(m_ResX + 1) * (m_ResY + 1)];
            int offset = (m_ResX * n);

            Array.Copy(m_Buffer, arr, m_Buffer.Length);

            Array.Clear(m_Buffer, 0, m_Buffer.Length);

            Array.Copy(arr, 0,
                m_Buffer, offset,
                m_Buffer.Length - offset);
        }

        private void ScrollPixelsRight()
        {
            bool[] arr = new bool[(m_ResX + 1) * (m_ResY + 1)];

            // Duplicate the buffer
            Array.Copy(m_Buffer, arr, m_Buffer.Length);

            // Clear the buffer
            Array.Clear(m_Buffer, 0, m_Buffer.Length);

            for (int y = 0; y < m_ResY; y++)
            {
                for (int x = 4; x < m_ResX; x++)
                {
                    m_Buffer[GetBufferPosition(x, y)] = arr[GetBufferPosition(x - 4, y)];
                }
            }
        }

        private void ScrollPixelsLeft()
        {
            bool[] arr = new bool[(m_ResX + 1) * (m_ResY + 1)];

            // Duplicate the buffer
            Array.Copy(m_Buffer, arr, m_Buffer.Length);

            // Clear the buffer
            Array.Clear(m_Buffer, 0, m_Buffer.Length);

            for (int y = 0; y < m_ResY; y++)
            {
                for (int x = 0; x < m_ResX - 4; x++)
                {
                    m_Buffer[GetBufferPosition(x, y)] = arr[GetBufferPosition(x + 4, y)];
                }
            }
        }

        public virtual void ScrollPixels(int length, int dir)
        {
            switch (dir)
            {
                case 1: ScrollPixelsLeft(); break;
                case 2: ScrollPixelsDown(length); break;
                case 3: ScrollPixelsRight();  break;
                default: break;
            }
            Update();
        }

        protected abstract void OnInit();

        protected abstract void OnShutdown();

        public abstract void Update();

        protected abstract void OnPauseStateChange(bool paused);

        protected int GetBufferPosition(int x, int y)
        {
            int val = (y * m_ResX) + x;

            if (val < m_Buffer.Length)
                return (val);
            else
                return 0;
        }

        protected virtual void OnSetCollision()
        {
            if (OnPixelCollision != null)
                OnPixelCollision(this, new EventArgs());
        }

        public bool[] GetRawBuffer()
        {
            return this.m_Buffer;
        }

        protected abstract void OnPixelSet(int x, int y, bool on);

        protected abstract void OnScreenClear();

        public override void Initialize()
        {
            CreateFakeBuffer();
            OnInit();
        }

        public override void Shutdown()
        {
            OnShutdown();
        }

        public override void SetPause(bool paused)
        {
            OnPauseStateChange(paused);
        }

        public RgbColor BackgroundColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        public RgbColor ForegroundColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }

        public bool DisableWrappingX
        {
            get { return this.m_DisableWrappingX; }
            set { this.m_DisableWrappingX = value; }
        }

        public bool DisableWrappingY
        {
            get { return this.m_DisableWrappingY; }
            set { this.m_DisableWrappingY = value; }
        }

        public bool EnableHires
        {
            get { return this.m_EnableHighres; }
            set { this.m_EnableHighres = value;}
        }

        public bool EnableAntiFlickerHack
        {
            get { return this.m_EnableAntiFlickerHack; }
            set { this.m_EnableAntiFlickerHack = value; }
        }

        public int CurrentResolutionX
        {
            get { return this.m_ResX; }
            set { this.m_ResX = value; }
        }

        public int CurrentResolutionY
        {
            get { return this.m_ResY; }
            set { this.m_ResY = value; }
        }
    }
}
