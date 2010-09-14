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
		private void EmitLoadLocal(byte num)
		{
			if (num < 4)
			{
				switch (num)
				{
				case 0: m_Emitter.Emit(OpCodes.Ldloc_0); break;
				case 1: m_Emitter.Emit(OpCodes.Ldloc_1); break;
				case 2: m_Emitter.Emit(OpCodes.Ldloc_2); break;
				case 3: m_Emitter.Emit(OpCodes.Ldloc_3); break;
				default: break;
				}
			}
			else
			{
				m_Emitter.Emit(OpCodes.Ldloc_S, num);
			}
		}
		
		private void EmitMethodCall(Type type, string methodName, byte loc1, byte loc2)
		{
			EmitLoadLocal(loc1);
			EmitLoadLocal(loc2);
			
			MethodInfo info = type.GetMethod(methodName);
			
			ParameterInfo[] pars = info.GetParameters();
			Type[] arguments = new Type[pars.Length];
			
			for (int i = 0; i < pars.Length; i++)
			{
				arguments[i] = pars[i].GetType();
			}
			
			m_Emitter.EmitCall(OpCodes.Call, info, arguments);
			
			m_Emitter.Emit(OpCodes.Nop);
		}
	}
}