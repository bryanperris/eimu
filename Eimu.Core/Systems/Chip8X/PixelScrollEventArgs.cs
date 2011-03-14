using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class PixelScrollEventArgs : EventArgs
    {
        private int dir;
        private int length;

        public PixelScrollEventArgs(int length, int dir)
        {
            this.dir = dir;
            this.length = length;
        }

        public int Direction
        {
            get { return this.dir; }
        }

        public int Length
        {
            get { return this.length; }
        }
    }
}
