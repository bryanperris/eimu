using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core;
using System.Windows.Media;
using System.Runtime.Serialization;

namespace Eimu.Configuration
{
    [Serializable]
    public static class Chip8XConfig
    {
        public static bool useRecompiler = false;
        public static bool use1802Recompiler = false;
        public static byte backgroundColorR;
        public static byte backgroundColorG = 0;
        public static byte backgroundColorB = 0;
        public static byte foregroundColorR = 255;
        public static byte foregroundColorG = 255;
        public static byte foregroundColorB = 255;
        public static bool enableCodeCache = false;
        public static bool disableAudio = false;
        public static bool disableGraphics = false;
        public static bool enableNetplay = false;
        public static bool disableWrappingX = false;
        public static bool disableWrappingY = false;
        public static bool forceHires = false;
        public static bool enableSuperMode = false;
        public static bool epicSpeed = false;
        public static bool antiFlicker = false;
        public static int hleMode = 0;
        public static bool loadFonts = true;
        public static bool normalBoot = true;
        public static int customLoadPoint = 0x200;

        public static Color BackColor
        {
            get
            {
                return Color.FromRgb(backgroundColorR, backgroundColorG, backgroundColorB);
            }
        }

        public static Color ForeColor
        {
            get
            {
                return Color.FromRgb(foregroundColorR, foregroundColorG, foregroundColorB);
            }
        }
    }
}
