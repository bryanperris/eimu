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
using System.Timers;
using System.Reflection;
using System.Reflection.Emit;
using Eimu.Core.Devices;

namespace Eimu.Core.CPU.Recompiling
{
    partial class Recompiler
    {
        [OpcodeTag(ChipOpcodes.Clr)]
        private void Cls(byte local1, byte local2)
        {
			EmitMethodCall(this.GetType(), "GraphicsDeviceCallback", local1, local2);
        }
    }
}