using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    [Serializable]
    public struct RgbColor
    {
        byte r;
        byte g;
        byte b;

        public RgbColor(byte red, byte green, byte blue)
        {
            this.r = red;
            this.g = green;
            this.b = blue;
        }

        public override bool Equals(object obj)
        {
            RgbColor color = (RgbColor)obj;

            if (this.r.Equals(color.r) && this.g.Equals(color.g) && this.b.Equals(color.b))
                return true;
            else
                return false;
        }

        public static bool operator ==(RgbColor colorA, RgbColor colorB)
        {
            return colorA.Equals(colorB);
        }

        public static bool operator !=(RgbColor colorA, RgbColor colorB)
        {
            return !colorA.Equals(colorB);
        }

        public override int GetHashCode()
        {
            return r ^ g ^ b;
        }

        public byte Red
        {
            get { return this.r; }
        }

        public byte Green
        {
            get { return this.g; }
        }

        public byte Blue
        {
            get { return this.b; }
        }
    }
}
