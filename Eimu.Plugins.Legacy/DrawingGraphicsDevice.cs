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
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

using Eimu.Core.Devices;
using Eimu.Plugins;


namespace Eimu.Plugins.Legacy
{
    [PluginInfo("GDI+ Graphics Plugin (Legacy)", "1.0", "Omegadox", "Renders using GDI+, but performence can be slow (use for accurate testing)")]
    public sealed class DrawingGraphicsDevice : GraphicsDevice, IPlugin
    {
        private Bitmap m_Bitmap;
        private Graphics m_Render;
        private Control m_Context;
        private int m_ResX;
        private int m_ResY;

        // Config
        private int m_Scale = 5;
        private SolidBrush m_BackBrush;
        private SolidBrush m_Brush;

        public DrawingGraphicsDevice()
        {
            PluginManager.EnableDoubleBuffer = true;
            m_Brush = new SolidBrush(Color.White);
            m_BackBrush = new SolidBrush(Color.Black);
        }

        private void SetResolution()
        {
            m_ResX = GraphicsDevice.RESOLUTION_WIDTH * m_Scale;
            m_ResY = GraphicsDevice.RESOLUTION_HEIGHT * m_Scale;
        }

        public override void OnPixelSet(int x, int y, bool on)
        {
            m_Render.FillRectangle(on ? m_Brush : m_BackBrush, new Rectangle(x * m_Scale, y * m_Scale, m_Scale, m_Scale));
            m_Context.Invalidate();
        }

        private void DrawToBuffer(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.Clear(m_BackBrush.Color);
            g.DrawImage(m_Bitmap, 0, 0, m_Context.Size.Width, m_Context.Size.Height);
        }

        public override void OnScreenClear()
        {
            m_Render.Clear(m_BackBrush.Color);
            m_Context.Invalidate();
        }

        public override void Initialize()
        {
            SetResolution();

            if (m_Context == null)
            {
                m_Context = Control.FromHandle(PluginManager.RenderContext);
                m_Context.Paint += new PaintEventHandler(m_Context_Paint);
                m_Context.Resize += new EventHandler(m_Context_Resize);
            }

            if (m_Bitmap == null)
            {
                m_Bitmap = new Bitmap(m_ResX, m_ResY);
                m_Render = Graphics.FromImage(m_Bitmap);
            }
        }

        void m_Context_Resize(object sender, EventArgs e)
        {
            m_Context.Invalidate();
        }

        void m_Context_Paint(object sender, PaintEventArgs e)
        {
            DrawToBuffer(e.Graphics);
        }

        public override void Shutdown()
        {
            this.ClearScreen();
        }

        public override void SetPauseState(bool paused)
        {
        }

        public void ShowConfigDialog()
        {
            GraphicsConfigForm f = new GraphicsConfigForm(this);
            f.ShowDialog();
        }

        public string[] GetOptionsList()
        {
            throw new NotImplementedException();
        }

        public void SetOption(string name, string value)
        {
            throw new NotImplementedException();
        }

        public string GetOption(string name)
        {
            throw new NotImplementedException();
        }

        public Color BackgroundColor
        {
            get { return this.m_BackBrush.Color; }
            set { this.m_BackBrush.Color = value; }
        }

        public Color ForgroundColor
        {
            get { return this.m_Brush.Color; }
            set { m_Brush.Color = value; }
        }
    }
}
