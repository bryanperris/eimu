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
            MessageBox.Show("Program has finished running!", "Eimu", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        void RenderWindow_Shown(object sender, EventArgs e)
        {
            if (this.m_Machine != null)
            {
                m_Machine.Start();
            }
        }
    }
}
