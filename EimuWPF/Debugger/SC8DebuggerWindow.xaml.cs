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
using System.IO;
using Eimu.Core.Systems.Chip8X;
using Eimu.Core;
using System.Windows.Threading;
using System.Globalization;
using System.Threading;


namespace Eimu.Debugger
{
    /// <summary>
    /// Interaction logic for SC8DebuggerWindow.xaml
    /// </summary>
    public partial class SC8DebuggerWindow : Window, IDebugger
    {
        Chip8XMachine m_Machine;
        Timer m_Timer_FontRefresh;
        EventWaitHandle m_EventWaitHandle_TimeFontRefresh;

        public SC8DebuggerWindow()
        {
            InitializeComponent();
            m_EventWaitHandle_TimeFontRefresh = new EventWaitHandle(false, EventResetMode.AutoReset);
            m_TextBox_MemSelectedAddress.Text = "0000";
            m_TextBox_MemSelectedAddress.TextChanged += new TextChangedEventHandler(m_TextBox_MemSelectedAddress_TextChanged);
            m_Button_MemGotoI.Click += new RoutedEventHandler(m_Button_MemGotoI_Click);
            m_Button_MemGotoPC.Click += new RoutedEventHandler(m_Button_MemGotoPC_Click);
            m_TextBox_MemSelectedAddress.MouseWheel += new MouseWheelEventHandler(m_TextBox_MemSelectedAddress_MouseWheel);
            m_TextBox_SprCurrentAddress.TextChanged += new TextChangedEventHandler(m_TextBox_SprCurrentAddress_TextChanged);
            m_Buttion_SprGotoI.Click += new RoutedEventHandler(m_Buttion_SprGotoI_Click);
            m_Button_SprRender.Click += new RoutedEventHandler(m_Button_SprRender_Click);
            m_RadioButton_SprSize5.Checked += new RoutedEventHandler(m_RadioButton_SprSize5_Checked);
            m_RadioButton_SprSize16.Checked += new RoutedEventHandler(m_RadioButton_SprSize16_Checked);
            m_RadioButton_SprSizeCustom.Checked += new RoutedEventHandler(m_RadioButton_SprSizeCustom_Checked);
            m_RadioButton_SprSizeScreen.Checked += new RoutedEventHandler(m_RadioButton_SprSizeScreen_Checked);
            m_Slider_SprExtSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(m_Slider_SprExtSize_ValueChanged);
            m_Button_MemDumpSection.Click += new RoutedEventHandler(m_Button_MemDumpSection_Click);
            m_Button_FontDraw.Click += new RoutedEventHandler(m_Button_FontDraw_Click);
            m_CheckBox_FontAutoRefresh.Checked += new RoutedEventHandler(m_CheckBox_FontAutoRefresh_Checked);
            m_CheckBox_FontAutoRefresh.Unchecked += new RoutedEventHandler(m_CheckBox_FontAutoRefresh_Unchecked);
            m_Button_MiscForce1802Rec.Click += new RoutedEventHandler(m_Button_MiscForce1802Rec_Click);
            m_Button_MiscScreenXor.Click += new RoutedEventHandler(m_Button_MiscScreenXor_Click);
            m_Button_MiscForceCPUStart.Click += new RoutedEventHandler(m_Button_MiscForceCPUStart_Click);
        }

        void m_Button_MiscForceCPUStart_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.ProcessorCore.PC = 0x200;
            m_Machine.StartCPUThread();
        }

        void m_Button_MiscScreenXor_Click(object sender, RoutedEventArgs e)
        {
            lock (m_Machine.VideoInterface)
            {
                for (int y = 0; y < m_Machine.VideoInterface.CurrentResolutionY; y++)
                {
                    for (int x = 0; x < m_Machine.VideoInterface.CurrentResolutionX; x++)
                    {
                        m_Machine.VideoInterface.SetPixel(x, y);
                    }
                }
            }
        }

        void m_Button_MiscForce1802Rec_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.IsHybridDynarecEnabled = true;
        }

        void m_RadioButton_SprSizeScreen_Checked(object sender, RoutedEventArgs e)
        {
            m_Slider_SprExtSize.IsEnabled = false;
        }

        void m_CheckBox_FontAutoRefresh_Unchecked(object sender, RoutedEventArgs e)
        {
            m_Timer_FontRefresh.Dispose(m_EventWaitHandle_TimeFontRefresh);
            m_EventWaitHandle_TimeFontRefresh.WaitOne();
        }

        void m_CheckBox_FontAutoRefresh_Checked(object sender, RoutedEventArgs e)
        {
            m_Timer_FontRefresh = new Timer(new TimerCallback(FontRefresh), null, 0, 2);
        }

        void m_Button_FontDraw_Click(object sender, RoutedEventArgs e)
        {
            DrawFonts();
        }

        void m_Button_MemDumpSection_Click(object sender, RoutedEventArgs e)
        {
            FileStream file = new FileStream("memdump.txt", FileMode.Create, FileAccess.Write);
            StringBuilder sb = new StringBuilder();

            ushort address = memoryViewer1.CurrentAddress;

            for (int i = 0; i < memoryViewer1.FieldCount; i++)
            {
                sb.AppendLine(address.ToString("X4") + ": " + Tools.PrintBits(m_Machine.SystemMemory.ReadByte(address++)));
                sb.AppendLine(address.ToString("X4") + ": " + Tools.PrintBits(m_Machine.SystemMemory.ReadByte(address++)));
            }

            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
            file.Write(buffer, 0, buffer.Length);
            file.Flush();
            file.Close();

            MessageBox.Show("Done!", "message", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        void m_Slider_SprExtSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_Label_SprCusSize.Content = (string)m_Slider_SprExtSize.Value.ToString();
        }

        void m_RadioButton_SprSizeCustom_Checked(object sender, RoutedEventArgs e)
        {
            m_Slider_SprExtSize.IsEnabled = true;
        }

        void m_RadioButton_SprSize16_Checked(object sender, RoutedEventArgs e)
        {
            m_Slider_SprExtSize.IsEnabled = false;
        }

        void m_RadioButton_SprSize5_Checked(object sender, RoutedEventArgs e)
        {
            m_Slider_SprExtSize.IsEnabled = false;
        }

        void m_Button_SprRender_Click(object sender, RoutedEventArgs e)
        {
            ushort address = ushort.Parse(m_TextBox_SprCurrentAddress.Text, NumberStyles.HexNumber);

            if (m_RadioButton_SprSizeScreen.IsChecked == true)
            {
                DrawSpriteToCanvas(32, 64, m_Canvas_SprSurface, address);
                return;
            }

            if (m_RadioButton_SprSize16.IsChecked == false)
            {
                if (m_RadioButton_SprSize5.IsChecked == true)
                {
                    DrawSpriteToCanvas(5, 8, m_Canvas_SprSurface, address);
                }
                else
                {
                    DrawSpriteToCanvas((int)m_Slider_SprExtSize.Value, 8, m_Canvas_SprSurface, address);
                }
            }
            else
            {
                DrawSpriteToCanvas(0, 8, m_Canvas_SprSurface, address);
            }
        }

        void m_Buttion_SprGotoI_Click(object sender, RoutedEventArgs e)
        {
            m_TextBox_SprCurrentAddress.Text = m_Machine.ProcessorCore.AddressRegister.ToString("X4");
        }

        void m_TextBox_SprCurrentAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = m_TextBox_SprCurrentAddress.Text;
            Color normal = Color.FromRgb(255, 255, 255);
            Color bad = Color.FromRgb(255, 100, 100);

            try
            {
                ushort address = ushort.Parse(val, System.Globalization.NumberStyles.HexNumber);

                if (address < 0 || address > (m_Machine.SystemMemory.Size-1))
                {
                    m_TextBox_SprCurrentAddress.Background = new SolidColorBrush(bad);
                    m_Button_SprRender.IsEnabled = false;
                }
                else
                {
                    m_TextBox_SprCurrentAddress.Background = new SolidColorBrush(normal);
                    m_Button_SprRender.IsEnabled = true;
                }
            }
            catch (FormatException)
            {
                m_TextBox_SprCurrentAddress.Background = new SolidColorBrush(bad);
                m_Button_SprRender.IsEnabled = false;
            }
            catch (OverflowException)
            {
                m_TextBox_SprCurrentAddress.Background = new SolidColorBrush(bad);
                m_Button_SprRender.IsEnabled = false;
            }
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
            m_TextBox_MemSelectedAddress.Text = m_Machine.ProcessorCore.PC.ToString("X4");
        }

        void m_Button_MemGotoI_Click(object sender, RoutedEventArgs e)
        {
            m_TextBox_MemSelectedAddress.Text = m_Machine.ProcessorCore.AddressRegister.ToString("X4");
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

        private void DrawSpriteToCanvas(int ysize, int xsize, Canvas canvas, ushort address)
        {
            int sizeW = 10;
            int sizeH = 10;
            canvas.Children.Clear();

            if (ysize > 0)
            {
                sizeW = (int)(canvas.ActualWidth / 8.0d);
                sizeH = (int)(canvas.ActualHeight / (double)ysize);
                byte read;


                for (int y = 0; y < ysize; y++)
                {
                    read = m_Machine.SystemMemory.ReadByte(address + y);

                    for (byte x = 0; x < 8; x++)
                    {
                        if ((read & (0x80 >> x)) != 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.Width = sizeW;
                            rect.Height = sizeH;
                            rect.Fill = new SolidColorBrush(Colors.Lime);
                            Canvas.SetLeft(rect, x * sizeW);
                            Canvas.SetTop(rect, y * sizeH);
                            canvas.Children.Add(rect);
                        }
                    }
                }
            }
            else
            {
                sizeW = (int)(canvas.ActualWidth / 16.0d);
                sizeH = (int)(canvas.ActualHeight / 16.0d);
                ushort data;

                for (int y = 0; y < 0x10; y++)
                {
                    data = Tools.Create16(m_Machine.SystemMemory.ReadByte(address + (y << 1)), m_Machine.SystemMemory.ReadByte(address + (y << 1) + 1));

                    for (int x = 0; x < 0x10; x++)
                    {
                        if ((data & (((int)0x8000) >> x)) != 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.Width = sizeW;
                            rect.Height = sizeH;
                            rect.Fill = new SolidColorBrush(Colors.Lime);
                            Canvas.SetLeft(rect, x * sizeW);
                            Canvas.SetTop(rect, y * sizeH);
                            canvas.Children.Add(rect);
                        }
                    }
                }
            }
        }

        private void FontRefresh(object state)
        {
            m_CheckBox_FontAutoRefresh.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                new Action(
                delegate()
                {
                  DrawFonts();
                }
              )
            );
        }

        private void DrawFonts()
        {
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox1, 5 * 0);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox2, 5 * 1);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox3, 5 * 2);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox4, 5 * 3);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox5, 5 * 4);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox6, 5 * 5);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox7, 5 * 6);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox8, 5 * 7);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox9, 5 * 8);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox10, 5 * 9);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox11, 5 * 10);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox12, 5 * 11);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox13, 5 * 12);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox14, 5 * 13);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox15, 5 * 14);
            DrawSpriteToCanvas(5, 8, m_Canvas_FontBox16, 5 * 15);
        }

        private void UpdateDebugInfo()
        {
            m_ListBox_Regs.Dispatcher.Invoke(DispatcherPriority.Render, new Action(ListRegisters));
            UpdateMemory();
        }

        private void ListRegisters()
        {
            CodeEngine en = m_Machine.ProcessorCore;
            m_ListBox_Regs.Items.Clear();
            m_ListBox_Regs.Items.Add("PC: " + en.PC.ToString("x"));
            m_ListBox_Regs.Items.Add("I: " + en.AddressRegister.ToString("x"));
            m_ListBox_Regs.Items.Add("V0: " + en.VRegisters[0].ToString("x"));
            m_ListBox_Regs.Items.Add("V1: " + en.VRegisters[1].ToString("x"));
            m_ListBox_Regs.Items.Add("V2: " + en.VRegisters[2].ToString("x"));
            m_ListBox_Regs.Items.Add("V3: " + en.VRegisters[3].ToString("x"));
            m_ListBox_Regs.Items.Add("V4: " + en.VRegisters[4].ToString("x"));
            m_ListBox_Regs.Items.Add("V5: " + en.VRegisters[5].ToString("x"));
            m_ListBox_Regs.Items.Add("V6: " + en.VRegisters[6].ToString("x"));
            m_ListBox_Regs.Items.Add("V7: " + en.VRegisters[7].ToString("x"));
            m_ListBox_Regs.Items.Add("V8: " + en.VRegisters[8].ToString("x"));
            m_ListBox_Regs.Items.Add("V9: " + en.VRegisters[9].ToString("x"));
            m_ListBox_Regs.Items.Add("VA: " + en.VRegisters[10].ToString("x"));
            m_ListBox_Regs.Items.Add("VB: " + en.VRegisters[11].ToString("x"));
            m_ListBox_Regs.Items.Add("VC: " + en.VRegisters[12].ToString("x"));
            m_ListBox_Regs.Items.Add("VD: " + en.VRegisters[13].ToString("x"));
            m_ListBox_Regs.Items.Add("VE: " + en.VRegisters[14].ToString("x"));
            m_ListBox_Regs.Items.Add("VF: " + en.VRegisters[15].ToString("x"));

            m_ListBox_Regs.Items.Add("RPL0: " + en.RPLRegisters[0].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL1: " + en.RPLRegisters[1].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL2: " + en.RPLRegisters[2].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL3: " + en.RPLRegisters[3].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL4: " + en.RPLRegisters[4].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL5: " + en.RPLRegisters[5].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL6: " + en.RPLRegisters[6].ToString("x"));
            m_ListBox_Regs.Items.Add("RPL7: " + en.RPLRegisters[7].ToString("x"));
        }

        private void ListCode()
        {

        }

        private void UpdateMemory()
        {
            memoryViewer1.UpdateFields();
            m_Label_MemVideoPointer.Content = "Video Offset: " + ((ChipMemory)m_Machine.SystemMemory).VideoPointer.ToString("X2");
        }

        #region IDebugger Members

        public void StartDebugging(VirtualMachine currentMachine)
        {
            m_Machine = (Chip8XMachine)currentMachine;
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
            m_Machine.ProcessorCore.IncrementPC();
            UpdateDebugInfo();
        }

        private void m_Button_MemRandFill_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random(System.Environment.TickCount);

            for (int i = 0; i < m_Machine.SystemMemory.Size; i++)
            {
                m_Machine.SystemMemory[i] = (byte)rnd.Next(0, 255);
            }
        }

        private void m_Button_RegsSetPC_Click(object sender, RoutedEventArgs e)
        {
            m_Machine.ProcessorCore.PC = ushort.Parse(m_TextBox_RegsAddressBox.Text, System.Globalization.NumberStyles.HexNumber);
        }

        private void m_TextBox_RegsAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = m_TextBox_RegsAddressBox.Text;
            Color normal = Color.FromRgb(255, 255, 255);
            Color bad = Color.FromRgb(255, 100, 100);

            try
            {
                ushort address = ushort.Parse(val, System.Globalization.NumberStyles.HexNumber);

                if (address < 0 || address > (m_Machine.SystemMemory.Size - 1))
                {
                    m_TextBox_RegsAddressBox.Background = new SolidColorBrush(bad);
                    m_Button_RegsSetPC.IsEnabled = false;
                }
                else
                {
                    m_TextBox_RegsAddressBox.Background = new SolidColorBrush(normal);
                    m_Button_RegsSetPC.IsEnabled = true;
                }
            }
            catch (FormatException)
            {
                m_TextBox_RegsAddressBox.Background = new SolidColorBrush(bad);
                m_Button_RegsSetPC.IsEnabled = false;
            }
            catch (OverflowException)
            {
                m_TextBox_SprCurrentAddress.Background = new SolidColorBrush(bad);
                m_Button_RegsSetPC.IsEnabled = false;
            }
        }

        private void m_Button_MemRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateMemory();
        }

    }
}
