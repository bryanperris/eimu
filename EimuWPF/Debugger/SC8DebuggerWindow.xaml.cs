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

        private void UpdateDebugInfo()
        {
            CodeEngine en = m_Machine.CodeEngineCore;

            ListRegisters();
        }

        private void ListRegisters()
        {
            CodeEngine en = m_Machine.CodeEngineCore;
            m_ListBox_Regs.Items.Clear();
            m_ListBox_Regs.Items.Add("PC: " + en.m_PC.ToString("x"));
            m_ListBox_Regs.Items.Add("I: " + en.m_IReg.ToString("x"));
            m_ListBox_Regs.Items.Add("V0: " + en.m_VRegs[0].ToString("x"));
            m_ListBox_Regs.Items.Add("V1: " + en.m_VRegs[1].ToString("x"));
            m_ListBox_Regs.Items.Add("V2: " + en.m_VRegs[2].ToString("x"));
            m_ListBox_Regs.Items.Add("V3: " + en.m_VRegs[3].ToString("x"));
            m_ListBox_Regs.Items.Add("V4: " + en.m_VRegs[4].ToString("x"));
            m_ListBox_Regs.Items.Add("V5: " + en.m_VRegs[5].ToString("x"));
            m_ListBox_Regs.Items.Add("V6: " + en.m_VRegs[6].ToString("x"));
            m_ListBox_Regs.Items.Add("V7: " + en.m_VRegs[7].ToString("x"));
            m_ListBox_Regs.Items.Add("V8: " + en.m_VRegs[8].ToString("x"));
            m_ListBox_Regs.Items.Add("V9: " + en.m_VRegs[9].ToString("x"));
            m_ListBox_Regs.Items.Add("VA: " + en.m_VRegs[10].ToString("x"));
            m_ListBox_Regs.Items.Add("VB: " + en.m_VRegs[11].ToString("x"));
            m_ListBox_Regs.Items.Add("VC: " + en.m_VRegs[12].ToString("x"));
            m_ListBox_Regs.Items.Add("VD: " + en.m_VRegs[13].ToString("x"));
            m_ListBox_Regs.Items.Add("VE: " + en.m_VRegs[14].ToString("x"));
            m_ListBox_Regs.Items.Add("VF: " + en.m_VRegs[15].ToString("x"));
        }


        #region IDebugger Members

        public void StartDebugging(VirtualMachine currentMachine)
        {
            m_Machine = (SChipMachine)currentMachine;
            UpdateDebugInfo();
        }

        public void Report()
        {
            UpdateDebugInfo();
        }

        public void StopDebugging()
        {
            
        }

        #endregion

        private void m_TbButton_Pause_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.Pause();
        }

        private void m_TbButton_Step_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.Pause();
            m_Machine.Step(1);
            UpdateDebugInfo();
        }

        private void m_TbButton_Run_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.Run();
        }

        private void m_TbButton_Skip_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.Pause();
            m_Machine.CodeEngineCore.IncrementPC();
            m_Machine.Step(1);
            UpdateDebugInfo();
        }
    }
}
