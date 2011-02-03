using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core;

namespace Eimu.Configuration
{
    public static class SchipConfig
    {
        public const string Chip8FontPath = Config.systemDirectory + "c8fnt.bin";
        public static bool useRecompiler = false;
        public static byte backgroundColorR;
        public static byte backgroundColorG = 0;
        public static byte backgroundColorB = 0;
        public static byte foregroundColorR = 255;
        public static byte foregroundColorG = 255;
        public static byte foregroundColorB = 255;
        public static bool disableTimers = false;
        public static bool enableCodeCache = false;
        public static bool disableAudio = false;
        public static bool disableGraphics = false;
        public static bool enableNetplay = false;
        public static bool disableWrapping = false;
        public static bool forceHires = false;
        public static bool enableSuperMode = false;
        public static bool epicSpeed = false;
        public static bool antiFlicker = false;

        public static RgbColor BackColor
        {
            get
            {
                return new RgbColor(backgroundColorR, backgroundColorG, backgroundColorB);
            }
        }

        public static RgbColor ForeColor
        {
            get
            {
                return new RgbColor(foregroundColorR, foregroundColorG, foregroundColorB);
            }
        }
    }
}
