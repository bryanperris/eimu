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

namespace EimuWPF
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
            InitializeComponent();
            m_WinHelper = new WindowInteropHelper(this);
            this.m_Machine = machine;
            machine.MachineEnded += new EventHandler(machine_MachineEnded);
            PluginManager.WindowHandle = m_WinHelper.Handle;
            PluginManager.RenderContext = renderPanel.Handle;
            this.Loaded += new RoutedEventHandler(RenderWindow_Loaded);
            m_Machine.Run();
        }

        void RenderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void machine_MachineEnded(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
