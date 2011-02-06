﻿/*  
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
using System.Runtime.InteropServices;

namespace Eimu.Core.Systems.SChip8
{
    [ComVisible(true)]
    public abstract class AudioDevice : Device
    {
        public abstract void Beep();

        protected abstract void OnInit();

        protected abstract void OnShutdown();

        public abstract void LoopBegin();

        public abstract void LoopEnd();

        protected abstract void OnPauseStateChange(bool paused);

        public override void Initialize()
        {
            OnInit();
        }

        public override void Shutdown()
        {
            OnShutdown();
        }

        public override void SetPause(bool paused)
        {
            OnPauseStateChange(paused);
        }
    }
}
