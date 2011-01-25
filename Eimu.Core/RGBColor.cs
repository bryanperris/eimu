using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    [Serializable]
    public struct RGBColor
    {
        byte r;
        byte g;
        byte b;

        public RGBColor(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public byte R
        {
            get { return this.r; }
        }

        public byte G
        {
            get { return this.g; }
        }

        public byte B
        {
            get { return this.b; }
        }
    }
}
