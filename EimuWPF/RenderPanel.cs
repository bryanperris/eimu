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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Eimu.Core.Plugin;

namespace EimuWPF
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
