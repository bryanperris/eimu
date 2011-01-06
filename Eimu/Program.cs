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
using System.Windows.Forms;
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;
using System.IO;

namespace Eimu
{
    static class Program
    {
        static bool closeApp = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            StartDialog startscreen = new StartDialog();
            startscreen.FormClosed += new FormClosedEventHandler(startscreen_FormClosed);

            while (!closeApp)
            {
                C8Machine vm = new C8Machine();
                startscreen.SetVM(vm);
                startscreen.ShowDialog();

                if (!closeApp)
                {
                    FileStream font = new FileStream(Config.FONT_PATH, FileMode.Open, FileAccess.Read);
                    vm.SetFontResource(font);
                    RenderWindow renderWindow = new RenderWindow(vm);
                    renderWindow.ShowDialog();
                    vm.Stop();
                    font.Close();
                }
            }
        }

        static void startscreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeApp = true;
        }
    }
}
