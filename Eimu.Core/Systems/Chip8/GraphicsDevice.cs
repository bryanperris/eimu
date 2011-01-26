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

namespace Eimu.Core.Systems.Chip8
{
    public abstract class GraphicsDevice : Device
    {
        public const int RESOLUTION_WIDTH = 64;
        public const int RESOLUTION_HEIGHT = 32;
        public const int RESOLUTION_SUPER_WIDTH = 128;
        public const int RESOLUTION_SUPER_HEIGHT = 64;
        public const int RESOLUTION_ENHANCED_WIDTH =70;
        public const int RESOLUTION_ENHANCED_HEIGHT = 40;
        public const int SPRITE_WIDTH = 8;
        private int m_ResX;
        private int m_ResY;
        private int m_XOffset;
        private int m_YOffset;
        private RGBColor m_BackColor;
        private RGBColor m_ForeColor;
        private bool[] m_Buffer;
        public event EventHandler OnPixelCollision;
        private bool m_DisableWrapping;
        private bool m_EnableHighres;
        private bool m_EnableAntiFlickerHack;
        private bool m_EnableEnhancedMode;

        private void CreateFakeBuffer()
        {

           m_XOffset = 0;
           m_YOffset = 0;
           m_ResX = RESOLUTION_WIDTH;
           m_ResY = RESOLUTION_HEIGHT;

            if (m_EnableEnhancedMode)
            {
                m_XOffset = 15;
                m_YOffset = 5;
                m_ResX = RESOLUTION_ENHANCED_WIDTH;
                m_ResY = RESOLUTION_ENHANCED_HEIGHT;
            }

            if (m_EnableHighres)
            {
                m_ResX = RESOLUTION_SUPER_WIDTH;
                m_ResY = RESOLUTION_SUPER_HEIGHT;
            }

            m_ResX += m_XOffset;
            m_ResY += m_YOffset;
            m_Buffer = new bool[(m_ResX + 1) * (m_ResY + 1)];
        }

        public virtual void ClearScreen()
        {
            Array.Clear(this.m_Buffer, 0, this.m_Buffer.Length);
            OnScreenClear();
        }

        public virtual void SetPixel(int x, int y)
        {
            if (!EnableEnhancedMode)
            {
                x &= 0x3F;
                y &= 0x1F;
            }

            if (!m_DisableWrapping)
            {

                // Wrapping
                if (x > m_ResX)
                    x -= m_ResX;

                if (x < 0)
                    x += m_ResX;

                if (y > m_ResY)
                    y -= m_ResY;

                if (y < 0)
                    y += m_ResY;
            }

            bool on = GetPixel(x, y) ^ true;

            if (!on)
                OnSetCollision();

            m_Buffer[GetBufferPosition(x, y)] = on;

            if (on && m_EnableAntiFlickerHack)
                return;

            OnPixelSet(x, y, on);

            // src 0 ^ 1 = 1 : Fill White
            // src 1 ^ 1 = 0 : Make Black, Set Collision
        }

        public virtual bool GetPixel(int x, int y)
        {
            x += m_XOffset;
            y += m_YOffset;
            return m_Buffer[GetBufferPosition(x, y)];
        }

        protected abstract void OnInit();

        protected abstract void OnShutdown();

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

        protected bool[] InternalBuffer
        {
            get
            {
                return this.m_Buffer;
            }
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

        public override void SetPauseState(bool paused)
        {
            OnPauseStateChange(paused);
        }

        public RGBColor BackgroundColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        public RGBColor ForegroundColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }

        public bool DisableWrapping
        {
            get { return this.m_DisableWrapping; }
            set { this.m_DisableWrapping = value; }
        }

        private void Reset()
        {
            Shutdown();
            Initialize();
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

        public bool EnableEnhancedMode
        {
            get { return this.m_EnableEnhancedMode; }
            set { this.m_EnableEnhancedMode = value; }
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
