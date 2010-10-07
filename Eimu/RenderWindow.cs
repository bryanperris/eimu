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

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!m_Paused)
            {
                m_Machine.SetPause(true);
                m_Paused = true;
                pauseToolStripMenuItem.Text = "Resume";
            }
            else
            {
                m_Machine.SetPause(false);
                m_Paused = false;
                pauseToolStripMenuItem.Text = "Pause";
            }
        }

        private void audioConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void graphicsConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void inputConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void projectSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/eimu"); 
        }
    }
}
