using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;
using System.Windows.Interop;

namespace Eimu
{
    /// <summary>
    /// Interaction logic for RenderWindow.xaml
    /// </summary>
    public partial class RenderWindow : Window
    {
        C8Machine m_Machine;
        bool m_Paused = false;
        WindowInteropHelper m_WinHelper;

        public RenderWindow(C8Machine machine)
        {
            m_Machine = machine;
            InitializeComponent();
        }

        void machine_MachineEnded(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_MenuItem_Reset_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.Restart();
        }

        private void m_MenuItem_Stop_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void m_MenuItem_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void m_MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog d = new AboutDialog();
            d.ShowDialog();
        }

        private void m_MenuItem_ProjectSite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://code.google.com/p/eimu"); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            m_WinHelper = new WindowInteropHelper(this);
            m_Machine.MachineEnded += new EventHandler(machine_MachineEnded);
            PluginManager.WindowHandle = m_WinHelper.Handle;
            PluginManager.RenderContext = renderPanel.Handle;
            m_Machine.Run();
        }

        private void m_MenuItem_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (m_MenuItem_Pause.IsChecked == true)
            {
                m_Machine.Pause();
            }
            else
            {
                m_Machine.Run();
            }
        }
    }
}
