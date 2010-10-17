/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  Omegadox, http://code.google.com/p/eimu

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

using Eimu.Core;
using Eimu.Core.Devices;
using Eimu.Plugins;

using OpenTK;
using System.Windows.Forms;
using OpenTK.Graphics;
using OpenTK.Platform;
using System.Threading;


namespace Eimu.Plugins.OpenTK
{
    [PluginInfo("OpenGL Plugin", "1.0", "Omegadox", "Renders using OpenGL")]
    public sealed class PluginOGL : GraphicsDevice, IPlugin
    {
        private Control m_ControlContext;
        private GraphicsContext m_GContext;
        private IWindowInfo m_WindowInfo;
        private float m_ScaleX = 10;
        private float m_ScaleY = 5;

        public PluginOGL()
        {
            PluginManager.EnableDoubleBuffer = false;
        }

        public override void Initialize()
        {
            m_ControlContext = (Control)Control.FromHandle(PluginManager.RenderContext);
            m_ControlContext.Resize += new EventHandler(m_ControlContext_Resize);
            m_ControlContext.Paint += new PaintEventHandler(m_ControlContext_Paint);

            m_WindowInfo = Utilities.CreateWindowsWindowInfo(PluginManager.RenderContext);
            m_GContext = new GraphicsContext(GraphicsMode.Default, m_WindowInfo);
            m_GContext.MakeCurrent(m_WindowInfo);
            
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.DepthRange(-1, 100);

            SetMatrix();
        }

        private void SetMatrix()
        {
            m_ScaleX = (float)m_ControlContext.Width / (float)GraphicsDevice.RESOLUTION_WIDTH;
            m_ScaleY = (float)m_ControlContext.Height / (float)GraphicsDevice.RESOLUTION_HEIGHT;
 
            GL.Viewport(m_ControlContext.ClientRectangle);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Glu.Ortho2D(0.0, (double)m_ControlContext.Width, (double)m_ControlContext.Height, 0.0);
        }

        void m_ControlContext_Paint(object sender, PaintEventArgs e)
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            GL.Begin(BeginMode.Quads);

            for (int y = 0; y < GraphicsDevice.RESOLUTION_HEIGHT; y++)
            {
                for (int x = 0; x < GraphicsDevice.RESOLUTION_WIDTH; x++)
                {
                    bool on = GetPixel(x, y);

                    if (on)
                    {
                        GL.Color4(Color.White);
                    }
                    else
                    {
                        GL.Color4(Color.Black);
                    }

                    GL.Vertex2(x * m_ScaleX, y * m_ScaleY);
                    GL.Vertex2((x * m_ScaleX) + m_ScaleX, y * m_ScaleY);
                    GL.Vertex2((x * m_ScaleX) + m_ScaleX, (y * m_ScaleY) + m_ScaleY);
                    GL.Vertex2(x * m_ScaleX, (y * m_ScaleY) + m_ScaleY);
                }
            }

            GL.End();

            GL.PopMatrix();
            m_GContext.SwapBuffers();
        }

        void m_ControlContext_Resize(object sender, EventArgs e)
        {
            SetMatrix();
        }

        protected override void OnPixelSet(int x, int y, bool on)
        {
            m_ControlContext.Invalidate();
        }

        protected override void OnScreenClear()
        {
            m_ControlContext.Invalidate();
        }

        public override void SetPauseState(bool paused)
        {
            
        }

        public override void Shutdown()
        {
            m_GContext.Dispose();
        }

        public string GetOption(string name)
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

        public void ShowConfigDialog()
        {
            throw new NotImplementedException();
        }
    }
}
