using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Eimu.Plugins;

namespace Eimu
{
    public class RenderPanel : Panel
    {
    
        public RenderPanel()
        {
            base.SetStyle(ControlStyles.Opaque, true);
            base.SetStyle(ControlStyles.UserPaint, true);
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.DoubleBuffered = PluginManager.EnableDoubleBuffer;
            this.InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (base.DesignMode)
            {
                e.Graphics.Clear(Color.Green);
            }

            base.OnPaint(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
