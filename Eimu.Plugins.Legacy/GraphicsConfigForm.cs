using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Eimu.Plugins.Legacy
{
    public partial class GraphicsConfigForm : Form
    {
        DrawingGraphicsDevice m_Device;

        public GraphicsConfigForm(DrawingGraphicsDevice device)
        {
            m_Device = device;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = m_Device.BackgroundColor;
            colorDialog1.ShowDialog();
            m_Device.BackgroundColor = colorDialog1.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = m_Device.ForgroundColor;
            colorDialog1.ShowDialog();
            m_Device.ForgroundColor = colorDialog1.Color;
        }
    }
}
