/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  Omegadox, http://code.google.com/p/eimu

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
using System.Media;
using Eimu.Core.Systems.SChip8;

namespace Eimu.Devices
{
    public class BeepAudioDevice : AudioDevice
    {
        SoundPlayer player;

        public override void Beep()
        {
            player.Play();
        }

        protected override void OnInit()
        {
            player = new SoundPlayer(".\\sys\\c8beep.wav");
            player.LoadAsync();
        }

        protected override void OnShutdown()
        {
            player.Dispose();
        }

        protected override void OnPauseStateChange(bool paused)
        {
            if (paused)
            {
                player.Stop();
            }
            else
            {
                player.PlayLooping();
            }
        }

        public override void LoopBegin()
        {
            player.PlaySync();
            player.PlayLooping();
        }

        public override void LoopEnd()
        {
            player.Stop();
        }
    }
}
