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

namespace Eimu.Core.Devices
{
    public enum GraphicsCommand : int
    {
        Nop = 0,
        Cls,
        SetPixel
    }

    public delegate void GraphicsCallback(GraphicsCommand command, ushort data);

    public abstract class GraphicsDevice
    {
        private readonly byte[] FONT = {0xF0, 0x90, 0x90, 0x90, 0xF0,   //0
	                                    0x20, 0x60, 0x20, 0x20, 0x70,   //1
	                                    0xF0, 0x10, 0xF0, 0x80, 0xF0,   //2
	                                    0xF0, 0x10, 0xF0, 0x10, 0xF0,   //3
	                                    0x90, 0x90, 0xF0, 0x10, 0x10,   //4
	                                    0xF0, 0x80, 0xF0, 0x10, 0xF0,   //5
	                                    0xF0, 0x80, 0xF0, 0x90, 0xF0,   //6
	                                    0xF0, 0x10, 0x20, 0x40, 0x40,   //7
	                                    0xF0, 0x90, 0xF0, 0x90, 0xF0,   //8
	                                    0xF0, 0x90, 0xF0, 0x10, 0xF0,   //9
	                                    0xF0, 0x90, 0xF0, 0x90, 0x90,   //A
	                                    0xE0, 0x90, 0xE0, 0x90, 0xE0,   //B
	                                    0xF0, 0x80, 0x80, 0x80, 0xF0,   //C
	                                    0xE0, 0x90, 0x90, 0x90, 0xE0,   //D
	                                    0xF0, 0x80, 0xF0, 0x80, 0xF0,   //E
	                                    0xF0, 0x80, 0xF0, 0x80, 0x80,}; //F
        public const int RESOLUTION_WIDTH = 64;
        public const int RESOLUTION_HEIGHT = 32;
        public const int SPRITE_WIDTH = 8;
        public const int SPRITE_HEIGHT = 1;
        public const int FONT_WIDHT = 4;
        public const int FONT_HEIGH = 5;

        public abstract void SendGraphicsCommand(GraphicsCommand command, ushort data);
    }
}
