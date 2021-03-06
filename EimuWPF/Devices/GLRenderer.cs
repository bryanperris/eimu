﻿/*  
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
using Eimu.Core.Systems.Chip8X;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using Eimu.Core.Systems.Chip8X.Interfaces;

namespace Eimu.Devices
{
    public sealed class GLRenderer : Renderer, IWinFormAttachment
    {

        private Control m_ControlContext;
        private GraphicsContext m_GContext;
        private IWindowInfo m_WindowInfo;

        public GLRenderer()
        {
        }

        void m_ControlContext_Paint(object sender, PaintEventArgs e)
        {
            if (AttachedVideoInterface == null && m_GContext.IsDisposed)
                return;

            if (!m_GContext.IsCurrent)
                Console.WriteLine("Warning context isn't on current thread!");


            GL.Viewport(m_ControlContext.ClientRectangle);

            float m_ScaleX = (float)m_ControlContext.Width / (float)AttachedVideoInterface.CurrentResolutionX;
            float m_ScaleY = (float)m_ControlContext.Height / (float)AttachedVideoInterface.CurrentResolutionY;

            GL.ClearColor(Color.FromArgb(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            Matrix4 matrix = OpenTK.Matrix4.CreateOrthographicOffCenter(0, (float)m_ControlContext.Width, (float)m_ControlContext.Height, 0, -1, 100);
            GL.LoadMatrix(ref matrix);

            GL.Begin(BeginMode.Quads);
            GL.Color4(Color.FromArgb(ForegroundColor.R, ForegroundColor.G, ForegroundColor.B));

            for (int y = 0; y < AttachedVideoInterface.CurrentResolutionY; y++)
            {
                for (int x = 0; x < AttachedVideoInterface.CurrentResolutionX; x++)
                {
                    if (AttachedVideoInterface.GetPixel(x, y))
                    {
                        GL.Vertex2(x * m_ScaleX, y * m_ScaleY);
                        GL.Vertex2((x * m_ScaleX) + m_ScaleX, y * m_ScaleY);
                        GL.Vertex2((x * m_ScaleX) + m_ScaleX, (y * m_ScaleY) + m_ScaleY);
                        GL.Vertex2(x * m_ScaleX, (y * m_ScaleY) + m_ScaleY);
                    }
                }
            }

            GL.End();

            GL.LoadIdentity();

            m_GContext.SwapBuffers();
        }

        #region IWinFormAttachment Members

        public void SetPanelHandle(IntPtr handle)
        {
            m_ControlContext = Control.FromHandle(handle);
            m_ControlContext.Paint += new PaintEventHandler(m_ControlContext_Paint);
            m_WindowInfo = Utilities.CreateWindowsWindowInfo(handle);
            m_GContext = new GraphicsContext(GraphicsMode.Default, m_WindowInfo);
            m_GContext.MakeCurrent(m_WindowInfo);

            if (!m_GContext.IsCurrent)
                throw new InvalidOperationException();

            m_GContext.LoadAll();
        }

        public void SetWindowHandle(IntPtr handle)
        {
        }

        public bool UseDoubleBugger
        {
            get
            {
                return false;
            }
            set
            {
                
            }
        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.DepthRange(-1, 100);
            GL.ClearColor(Color.FromArgb(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
            GL.Clear(ClearBufferMask.ColorBufferBit);
            m_GContext.SwapBuffers();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_GContext.Dispose();
        }

        public override void DisplayRefresh()
        {
            m_ControlContext.Invalidate();
        }
    }
}
