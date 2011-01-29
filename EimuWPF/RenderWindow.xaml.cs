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
using Eimu.Core.Systems.SChip8;
using System.Windows.Interop;

namespace Eimu
{
    /// <summary>
    /// Interaction logic for RenderWindow.xaml
    /// </summary>
    public partial class RenderWindow : Window
    {
        SC8Machine m_Machine;
        WindowInteropHelper m_WinHelper;

        public RenderWindow(SC8Machine machine)
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
            m_FormHost.Focus();
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

        private void WindowsFormsHost_KeyDown(object sender, KeyEventArgs e)
        {
            ChipKeys key = ChipKeys.None;

            switch (e.Key)
            {
                case Key.Q: key = ChipKeys.One; break;
                case Key.W: key = ChipKeys.Two; break;
                case Key.E: key = ChipKeys.Three; break;
                case Key.R: key = ChipKeys.A; break;
                case Key.T: key = ChipKeys.D; break;
                case Key.A: key = ChipKeys.Four; break;
                case Key.S: key = ChipKeys.Five; break;
                case Key.D: key = ChipKeys.Six; break;
                case Key.G: key = ChipKeys.E; break;
                case Key.F: key = ChipKeys.B; break;
                case Key.Z: key = ChipKeys.Seven; break;
                case Key.X: key = ChipKeys.Eight; break;
                case Key.C: key = ChipKeys.Nine; break;
                case Key.V: key = ChipKeys.C; break;
                case Key.B: key = ChipKeys.F; break;
                case Key.Space: key = ChipKeys.Zero; break;
                default: break;
            }

            m_Machine.SetKeyPress(key);
        }

        private void WindowsFormsHost_KeyUp(object sender, KeyEventArgs e)
        {
           m_Machine.SetKeyPress(ChipKeys.None);
        }
    }
}
