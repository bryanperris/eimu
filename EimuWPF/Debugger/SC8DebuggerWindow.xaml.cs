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
using Eimu.Core;


namespace Eimu.Debugger
{
    /// <summary>
    /// Interaction logic for SC8DebuggerWindow.xaml
    /// </summary>
    public partial class SC8DebuggerWindow : Window, IDebugger
    {
        SChipMachine m_Machine;

        public SC8DebuggerWindow()
        {
            InitializeComponent();
        }


        #region IDebugger Members

        public void StartDebugging(object currentMachine)
        {
            m_Machine = (SChipMachine)currentMachine;
        }

        public void Report(object state)
        {
            throw new NotImplementedException();
        }

        public void StopDebugging()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
