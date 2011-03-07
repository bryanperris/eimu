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
using System.Windows.Threading;
using System.Globalization;


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
            m_TextBox_MemSelectedAddress.Text = "0000";
            m_TextBox_MemSelectedAddress.TextChanged += new TextChangedEventHandler(m_TextBox_MemSelectedAddress_TextChanged);
            m_Button_MemGotoI.Click += new RoutedEventHandler(m_Button_MemGotoI_Click);
            m_Button_MemGotoPC.Click += new RoutedEventHandler(m_Button_MemGotoPC_Click);
            m_TextBox_MemSelectedAddress.MouseWheel += new MouseWheelEventHandler(m_TextBox_MemSelectedAddress_MouseWheel);
        }

        void m_TextBox_MemSelectedAddress_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                ushort addr = ushort.Parse(((TextBox)sender).Text, NumberStyles.HexNumber);

                if (e.Delta > 0)
                {
                    addr++;
                }
                else
                {
                    addr--;
                }

                m_TextBox_MemSelectedAddress.Text = addr.ToString("X4");
            }
            catch (Exception)
            {
                return;
            }
        }

        void m_Button_MemGotoPC_Click(object sender, RoutedEventArgs e)
        {
            m_TextBox_MemSelectedAddress.Text = m_Machine.CodeEngineCore.PC.ToString("X4");
        }

        void m_Button_MemGotoI_Click(object sender, RoutedEventArgs e)
        {
            m_TextBox_MemSelectedAddress.Text = m_Machine.CodeEngineCore.m_IReg.ToString("X4");
        }

        void m_TextBox_MemSelectedAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = m_TextBox_MemSelectedAddress.Text;
            Color normal = Color.FromRgb(255, 255, 255);
            Color bad = Color.FromRgb(255, 100, 100);

            try
            {
                ushort address = ushort.Parse(val, System.Globalization.NumberStyles.HexNumber);

                if (!memoryViewer1.GotoAddress(address))
                {
                    m_TextBox_MemSelectedAddress.Background = new SolidColorBrush(bad);
                }
                else
                {
                    m_TextBox_MemSelectedAddress.Background = new SolidColorBrush(normal);
                }
            }
            catch (FormatException)
            {
                m_TextBox_MemSelectedAddress.Background = new SolidColorBrush(bad);
            }
            catch (OverflowException)
            {
                m_TextBox_MemSelectedAddress.Background = new SolidColorBrush(bad);
            }
        }

        private void UpdateDebugInfo()
        {
            m_ListBox_Regs.Dispatcher.Invoke(DispatcherPriority.Render, new Action(ListRegisters));
            UpdateMemory();
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

        private void ListCode()
        {

        }

        private void UpdateMemory()
        {
            memoryViewer1.UpdateFields();
        }


        #region IDebugger Members

        public void StartDebugging(VirtualMachine currentMachine)
        {
            m_Machine = (SChipMachine)currentMachine;
            memoryViewer1.SetMemory(m_Machine.SystemMemory);
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
            UpdateDebugInfo();
        }
    }
}
