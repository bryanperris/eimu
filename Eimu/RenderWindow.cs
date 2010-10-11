using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eimu.Core;
using Eimu.Plugins;

namespace Eimu
{
    public partial class RenderWindow : Form
    {
        VirtualMachine m_Machine;
        bool m_Paused = false;

        public RenderWindow(VirtualMachine machine)
        {
            InitializeComponent();
            this.Text = Eimu.Properties.Resources.WindowCaption;
            this.m_Machine = machine;
            PluginManager.WindowHandle = this.Handle;
            PluginManager.RenderContext = this.panel_RenderContext.Handle;
            this.Shown += new EventHandler(RenderWindow_Shown);
            m_Machine.CurrentProcessor.ProgramEnd += new EventHandler(CurrentProcessor_ProgramEnd);
        }

        void CurrentProcessor_ProgramEnd(object sender, EventArgs e)
        {
            //MessageBox.Show("Program has finished running!", "Eimu", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void RenderWindow_Shown(object sender, EventArgs e)
        {
            m_Machine.Start();
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
                m_Machine.SetPause(true);
                m_Paused = true;
                pauseToolStripMenuItem1.Text = "Resume";
            }
            else
            {
                m_Machine.SetPause(false);
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
            ((IPlugin)m_Machine.CurrentGraphicsDevice).ShowConfigDialog();
        }

        private void audioConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ((IPlugin)m_Machine.CurrentAudioDevice).ShowConfigDialog();
        }

        private void inputConfigToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ((IPlugin)m_Machine.CurrentInputDevice).ShowConfigDialog();
        }

        private void RenderWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C: m_Machine.CurrentGraphicsDevice.ClearScreen(); break;
                    case Keys.S: m_Machine.CurrentProcessor.SetCollision(); break;
                    case Keys.R: m_Machine.Restart(); break;
                    case Keys.P: pauseToolStripMenuItem1_Click(this, new EventArgs()); break;
                    default: break;
                }
            }
        }
    }
}
