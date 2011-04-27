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
using Eimu.Core.Systems.Chip8X;
using System.Windows.Interop;
using Eimu.Devices;
using Eimu.Debugger;
using Eimu.Configuration;
using Eimu.Core.Systems.Chip8X.Interfaces;
using Eimu.Core;

namespace Eimu
{
    /// <summary>
    /// Interaction logic for RenderWindow.xaml
    /// </summary>
    public partial class RenderWindow : Window
    {
        Chip8XMachine m_Machine;
        WindowInteropHelper m_WinHelper;
        SC8DebuggerWindow m_Debugger;
        GLRenderer m_GLRenderer;

        public RenderWindow(Chip8XMachine machine)
        {
            m_Machine = machine;
            InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(RenderWindow_Closing);
        }

        void RenderWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            m_Machine.Dispose();
            m_GLRenderer.Shutdown();
        }

        private void UpdatePressKeyLabel()
        {
            m_Label_MenuPressedKey.Content = "Key Pressed: " + ((int)(m_Machine.PressedKey)).ToString("X2");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            m_Debugger.Close();
            m_Machine.AttachDebugger(null);
            m_Machine.Stop();
            m_GLRenderer.Shutdown();
            base.OnClosing(e);
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

            m_GLRenderer = new GLRenderer();
            m_GLRenderer.SetPanelHandle(renderPanel.Handle);
            m_GLRenderer.SetWindowHandle(m_WinHelper.Handle);
            m_GLRenderer.Initialize();
            m_GLRenderer.BackgroundColor = Color.FromRgb(Chip8XConfig.BackColor.R, Chip8XConfig.BackColor.G, Chip8XConfig.BackColor.B);
            m_GLRenderer.ForegroundColor = Color.FromRgb(Chip8XConfig.ForeColor.R, Chip8XConfig.ForeColor.G, Chip8XConfig.ForeColor.B);
            
            if (!Chip8XConfig.disableGraphics)
            {
                m_GLRenderer.AttachToVideoInterface(m_Machine.VideoInterface);
            }
            
            m_Debugger = new SC8DebuggerWindow();
            m_Machine.AttachDebugger(m_Debugger);

            try
            {
                m_Machine.Run();
            }
            catch (VMException err)
            {
                MessageBox.Show(err.Message, "VM Runtime Error", MessageBoxButton.OK, MessageBoxImage.Error);
                m_GLRenderer.Shutdown();
                this.Close();
            }
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
            HexKey key = HexKey.None;

            switch (e.Key)
            {
                case Key.Q: key = HexKey.One; break;
                case Key.W: key = HexKey.Two; break;
                case Key.E: key = HexKey.Three; break;
                case Key.R: key = HexKey.A; break;
                case Key.T: key = HexKey.D; break;
                case Key.A: key = HexKey.Four; break;
                case Key.S: key = HexKey.Five; break;
                case Key.D: key = HexKey.Six; break;
                case Key.G: key = HexKey.E; break;
                case Key.F: key = HexKey.B; break;
                case Key.Z: key = HexKey.Seven; break;
                case Key.X: key = HexKey.Eight; break;
                case Key.C: key = HexKey.Nine; break;
                case Key.V: key = HexKey.C; break;
                case Key.B: key = HexKey.F; break;
                case Key.Space: key = HexKey.Zero; break;
                default: break;
            }

            m_Machine.PressedKey = key;
            UpdatePressKeyLabel();
        }

        private void WindowsFormsHost_KeyUp(object sender, KeyEventArgs e)
        {
            m_Machine.PressedKey = HexKey.None;
            UpdatePressKeyLabel();
        }

        private void m_MenuItem_Debugger_Click(object sender, RoutedEventArgs e)
        {
            if (m_Debugger != null)
                m_Debugger.Show();
        }
    }
}
