﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Eimu.Core.Systems.Chip8X;
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
            //base.OnStartup(e);
            Thread.CurrentThread.Name = "Main App";
            StartDialog startscreen = new StartDialog();
            startscreen.Closed += new EventHandler(startscreen_Closed);

            while (!closeApp)
            {
                Chip8XMachine vm = new Chip8XMachine();
                startscreen.SetVM(vm);
                startscreen.ShowDialog();

                if (!closeApp)
                {
                    RenderWindow renderWindow = new RenderWindow(vm);
                    renderWindow.ShowDialog();
                    vm.Stop();
                }
            }
        }

        static void startscreen_Closed(object sender, EventArgs e)
        {
            closeApp = true;
        }
    }
}
