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
            this.m_Machine = machine;
            PluginManager.WindowHandle = this.Handle;
            PluginManager.RenderContext = this.panel_RenderContext.Handle;
            this.Shown += new EventHandler(RenderWindow_Shown);
        }

        void RenderWindow_Shown(object sender, EventArgs e)
        {
            if (this.m_Machine != null)
            {
                m_Machine.SetState(RunState.Running);
            }
        }
    }
}
