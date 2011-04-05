using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Eimu.Core.Systems.Chip8X
{
    public delegate void RenderCallback(VideoInterface currentInterface);

    [Serializable]
    public sealed class VideoInterface
    {
        public const int StandardResolutionX = 64;
        public const int StandardResolutionY = 32;
        public const int StandardSpriteSize = 8;
        public const int SuperResolutionX = 128;
        public const int SuperResolutionY = 64;
        public const int SuperSpriteSize = 16;
        private bool[] m_Buffer;
        private int m_ResX;
        private int m_ResY;
        private bool m_DisableWrappingX;
        private bool m_DisableWrappingY;
        private bool m_EnableAntiFlickerHack;
        public event RenderCallback Render;
        private Timer m_RenderInterrupt;
        EventWaitHandle m_RenderWait;

        public void Initialize(ChipMode mode)
        {
            m_RenderWait = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_RenderInterrupt = new Timer(new TimerCallback(OnRefresh), this, 0, 1);

            switch (mode)
            {
                case ChipMode.SuperChip: m_ResX = SuperResolutionX; m_ResY = SuperResolutionY; break;
                case ChipMode.MegaChip:
                case ChipMode.Chip8:
                default: m_ResX = StandardResolutionX; m_ResY = StandardResolutionY; break;
            }

            m_Buffer = new bool[(m_ResX + 1) * (m_ResY + 1)];
        }

        public void Shutdown()
        {

        }

        public bool IsPaused { get; set; }

        public void ClearPixels()
        {
            Array.Clear(m_Buffer, 0, m_Buffer.Length);
            //OnRefresh();
        }

        public bool SetPixel(int x, int y)
        {
            if (!m_DisableWrappingX)
                x &= (m_ResX - 1);

            if (!m_DisableWrappingY)
                y &= (m_ResY - 1);

            bool on = GetPixel(x, y) ^ true;

            m_Buffer[GetBufferPosition(x, y)] = on;

            //if (!(!on && m_EnableAntiFlickerHack))
            //    OnRefresh();

            return !on;

            // src 0 ^ 1 = 1 : Fill White
            // src 1 ^ 1 = 0 : Make Black, Set Collision
        }

        public void RenderWait()
        {
             //m_RenderWait.WaitOne();
            Thread.Sleep(8);
        }

        private void OnRefresh(object state)
        {
            if (Render != null)
            {
                Render(this);
            }

            m_RenderWait.Set();
        }

        public bool GetPixel(int x, int y)
        {
            return m_Buffer[GetBufferPosition(x, y)];
        }

        public void ScrollPixels(int length, int dir)
        {
            switch (dir)
            {
                case 1: ScrollPixelsLeft(); break;
                case 2: ScrollPixelsDown(length); break;
                case 3: ScrollPixelsRight(); break;
                default: break;
            }
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

        private int GetBufferPosition(int x, int y)
        {
            int val = (y * m_ResX) + x;

            if (val < m_Buffer.Length)
                return (val);
            else
                return 0;
        }

        public byte PeekPixels(int offset)
        {
            byte pixelRow = 0;

            for (int i = 0; i < 8; i++)
            {
                if (m_Buffer[offset++])
                    pixelRow |= 1;

                pixelRow <<= 1;
            }

            return pixelRow;
        }

        public void PokePixels(int offset, byte value)
        {
            for (int i = 7; i >= 0; i--)
            {
                m_Buffer[offset++] = (((value >> i) & 0x1) == 1 ? true : false);
            }
        }

        public bool[] Pixels
        {
            get { return this.m_Buffer; }
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
