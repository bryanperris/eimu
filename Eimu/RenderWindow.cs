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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Eimu.Core;
using Eimu.Core.Plugin;
using Eimu.Core.Systems.Chip8;

namespace Eimu
{
    public partial class RenderWindow : Form
    {
        C8Machine m_Machine;
        bool m_Paused = false;

        public RenderWindow(C8Machine machine)
        {
            InitializeComponent();
            this.Text = Eimu.Properties.Resources.WindowCaption;
            this.m_Machine = machine;
            PluginManager.WindowHandle = this.Handle;
            PluginManager.RenderContext = this.panel_RenderContext.Handle;
            this.Shown += new EventHandler(RenderWindow_Shown);
        }

        void RenderWindow_Shown(object sender, EventArgs e)
        {
            m_Machine.Run();
        }

        private void aboutToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void projectSiteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/eimu"); 
        }

        private void pauseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!m_Paused)
            {
                m_Machine.Pause();
                m_Paused = true;
                pauseToolStripMenuItem1.Text = "Resume";
            }
            else
            {
                m_Machine.Run();
                m_Paused = false;
                pauseToolStripMenuItem1.Text = "Pause";
            }
        }

        private void stopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_Machine.Restart();
        }

        private void graphicsConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ((IPlugin)m_Machine.CurrentGraphicsDevice).ShowConfigDialog();
            }
            catch (System.Exception)
            {
            }
        }

        private void audioConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ((IPlugin)m_Machine.CurrentAudioDevice).ShowConfigDialog();
            }
            catch (System.Exception)
            {
            }
        }

        private void inputConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                ((IPlugin)m_Machine.CurrentInputDevice).ShowConfigDialog();
            }
            catch (System.Exception)
            {
            }
        }

        private void RenderWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C: m_Machine.CurrentGraphicsDevice .ClearScreen(); break;
                    case Keys.S: m_Machine.CurrentProcessor.SetCollision(); break;
                    case Keys.R: m_Machine.Restart(); break;
                    case Keys.P: pauseToolStripMenuItem1_Click(this, new EventArgs()); break;
                    case Keys.Escape: this.Close(); break;
                    default: break;
                }
            }
        }
    }
}
