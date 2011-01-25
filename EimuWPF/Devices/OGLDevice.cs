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
using System.Text;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Eimu.Core;
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Eimu.Devices
{
    [PluginInfo("OpenGL Plugin", "1.0", "Omegadox", "Renders using OpenGL")]
    public sealed class OGLDevice : GraphicsDevice, IPlugin
    {
        private Control m_ControlContext;
        private GraphicsContext m_GContext;
        private IWindowInfo m_WindowInfo;
        private float m_ScaleX = 10;
        private float m_ScaleY = 5;

        public OGLDevice()
        {
            PluginManager.EnableDoubleBuffer = false;
        }

        void m_ControlContext_Paint(object sender, PaintEventArgs e)
        {
            GL.Viewport(m_ControlContext.ClientRectangle);

            m_ScaleX = (float)m_ControlContext.Width / (float)GraphicsDevice.RESOLUTION_WIDTH;
            m_ScaleY = (float)m_ControlContext.Height / (float)GraphicsDevice.RESOLUTION_HEIGHT;

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4 matrix = OpenTK.Matrix4.CreateOrthographicOffCenter(0, (float)m_ControlContext.Width, (float)m_ControlContext.Height, 0, -1, 100);
            GL.LoadMatrix(ref matrix);

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

            GL.LoadIdentity();

            m_GContext.SwapBuffers();
        }

        protected override void OnPixelSet(int x, int y, bool on)
        {
            m_ControlContext.Invalidate();
        }

        protected override void OnScreenClear()
        {
            m_ControlContext.Invalidate();
        }

        protected override void OnPauseStateChange(bool paused)
        {

        }

        protected override void OnShutdown()
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

        protected override void OnInit()
        {
            m_ControlContext = Control.FromHandle(PluginManager.RenderContext);
            m_ControlContext.Paint += new PaintEventHandler(m_ControlContext_Paint);
            m_WindowInfo = Utilities.CreateWindowsWindowInfo(PluginManager.RenderContext);
            m_GContext = new GraphicsContext(GraphicsMode.Default, m_WindowInfo);

            m_GContext.MakeCurrent(m_WindowInfo);

            if (!m_GContext.IsCurrent)
                throw new InvalidOperationException();

            m_GContext.LoadAll();

            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.DepthRange(-1, 100);
        }
    }
}
