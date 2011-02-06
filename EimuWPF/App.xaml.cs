using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Eimu.Core.Systems.SChip8;
using Eimu.Configuration;
using System.IO;
using System.Threading;

namespace Eimu
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static bool closeApp = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            Thread.CurrentThread.Name = "Main App";
            //ThreadPool
            base.OnStartup(e);
            StartDialog startscreen = new StartDialog();
            startscreen.Closed += new EventHandler(startscreen_Closed);

            while (!closeApp)
            {
                SChipMachine vm = new SChipMachine();
                startscreen.SetVM(vm);
                startscreen.ShowDialog();

                if (!closeApp)
                {
                    FileStream font = new FileStream(SchipConfig.Chip8FontPath, FileMode.Open, FileAccess.Read);
                    FileStream sfont = new FileStream(SchipConfig.SChipFontPath, FileMode.Open, FileAccess.Read);
                    vm.SetFontResource(font, sfont);
                    RenderWindow renderWindow = new RenderWindow(vm);
                    renderWindow.ShowDialog();
                    vm.Stop();
                    font.Close();
                    sfont.Close();
                }
            }
        }

        static void startscreen_Closed(object sender, EventArgs e)
        {
            closeApp = true;
        }
    }
}
