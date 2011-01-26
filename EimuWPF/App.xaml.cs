using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Eimu.Core.Systems.Chip8;
using System.IO;

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
            base.OnStartup(e);
            //Eimu.App app = new Eimu.App();
            StartDialog startscreen = new StartDialog();
            startscreen.Closed += new EventHandler(startscreen_Closed);

            while (!closeApp)
            {
                C8Machine vm = new C8Machine();
                startscreen.SetVM(vm);
                startscreen.ShowDialog();

                if (!closeApp)
                {
                    FileStream font = new FileStream(Config.CHIP8_FONT_PATH, FileMode.Open, FileAccess.Read);
                    vm.SetFontResource(font);
                    RenderWindow renderWindow = new RenderWindow(vm);
                    renderWindow.ShowDialog();
                    vm.Stop();
                    font.Close();
                }
            }
        }

        static void startscreen_Closed(object sender, EventArgs e)
        {
            closeApp = true;
        }
    }
}
