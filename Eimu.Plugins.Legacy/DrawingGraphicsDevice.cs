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


namespace Eimu
{
    [PluginInfo("Legacy Video", "1.0", "Omegadox", "Draws using a bitmap")]
    public sealed class DrawingGraphicsDevice : GraphicsDevice, IPlugin
    {
        Bitmap m_Bitmap;
        Graphics m_Render;
        Control m_Context;
        int m_Scale = 6;

        public DrawingGraphicsDevice()
        {

        }

        public override void SetPixel(int x, int y)
        {
            m_Render.FillRectangle(Brushes.White, new Rectangle(x * m_Scale, y * m_Scale, m_Scale, m_Scale));
            m_Context.Invalidate();
        }

        public override void ClearScreen()
        {
            m_Render.Clear(Color.Black);
            m_Context.Invalidate();
        }

        public override void Initialize()
        {
            m_Context = Control.FromHandle(PluginManager.RenderContext);
            m_Bitmap = new Bitmap(GraphicsDevice.RESOLUTION_WIDTH * m_Scale, GraphicsDevice.RESOLUTION_HEIGHT * m_Scale);
            m_Render = Graphics.FromImage(m_Bitmap);
            m_Context.Paint += new PaintEventHandler(m_Context_Paint);
        }

        void m_Context_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.Bilinear;

            e.Graphics.ResetTransform();

            e.Graphics.ScaleTransform(
                m_Context.Size.Width / (GraphicsDevice.RESOLUTION_WIDTH * m_Scale),
                m_Context.Size.Height / (GraphicsDevice.RESOLUTION_HEIGHT * m_Scale), MatrixOrder.Prepend);

            e.Graphics.DrawImage(m_Bitmap, 0, 0);
        }

        public override void Shutdown()
        {
        }

        public override void SetPauseState(bool paused)
        {
        }

        #region IPlugin Members

        public void ShowConfigDialog()
        {
            throw new NotImplementedException();
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

        #endregion
    }
}
