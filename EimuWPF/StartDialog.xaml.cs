using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Eimu.Core.Systems.Chip8X;
using Eimu.Core.Systems.Chip8X.Engines;
using WPFColorPickerLib;
using Eimu.Devices;
using Eimu.Configuration;
using Eimu.Core.Systems.CDP1802;

namespace Eimu
{
	public partial class StartDialog : Window
	{
		private OpenFileDialog m_OpenFileDialog;
		private FileStream m_RomFileSource;
		private Chip8XMachine m_VM;
		private ColorDialog colorDialog;
        private bool m_Quitting;

		public StartDialog()
		{
			InitializeComponent();
            this.Closing += new System.ComponentModel.CancelEventHandler(StartDialog_Closing);
            Application.Current.Exit += new ExitEventHandler(Current_Exit);
            m_Textbox_C8XLoadPointAddress.TextChanged += new TextChangedEventHandler(m_Textbox_C8XLoadPointAddress_TextChanged);
            m_RadioButton_C8XNormalLoadPoint.Checked += new RoutedEventHandler(m_RadioButton_C8XNormalLoadPoint_Checked);
			m_OpenFileDialog = new OpenFileDialog();
			m_OpenFileDialog.Filter = "Chip8X Programs (*.sc, *.ch8, *.c8, *.c8x)|*.sc;*.ch8;*.c8;*.c8x;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
			LoadConfig();
		}

        void Current_Exit(object sender, ExitEventArgs e)
        {
            m_Quitting = true;
        }

        void StartDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // A hack or force the program closed
            Application.Current.Shutdown();
        }

        void m_RadioButton_C8XNormalLoadPoint_Checked(object sender, RoutedEventArgs e)
        {
            m_Button_RunEmulator.IsEnabled = true;
            m_Textbox_C8XLoadPointAddress.Text = "0200";
        }

        void m_Textbox_C8XLoadPointAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            string val = m_Textbox_C8XLoadPointAddress.Text;
            Color normal = Color.FromRgb(255, 255, 255);
            Color bad = Color.FromRgb(255, 100, 100);

            try
            {
                ushort address = ushort.Parse(val, System.Globalization.NumberStyles.HexNumber);

                if (address < 0 || address > ushort.MaxValue)
                {
                    m_Textbox_C8XLoadPointAddress.Background = new SolidColorBrush(bad);
                    m_Button_RunEmulator.IsEnabled = false;
                }
                else
                {
                    m_Textbox_C8XLoadPointAddress.Background = new SolidColorBrush(normal);
                    m_Button_RunEmulator.IsEnabled = true;
                }
            }
            catch (FormatException)
            {
                m_Textbox_C8XLoadPointAddress.Background = new SolidColorBrush(bad);
                m_Button_RunEmulator.IsEnabled = false;
            }
            catch (OverflowException)
            {
                m_Textbox_C8XLoadPointAddress.Background = new SolidColorBrush(bad);
                m_Button_RunEmulator.IsEnabled = false;
            }
        }

		public void SaveConfig()
		{
			#region Chip8X

			Chip8XConfig.antiFlicker = m_CheckBox_C8AntiFlickerHack.IsChecked == true;
			Chip8XConfig.disableGraphics = m_CheckBox_C8DisableGraphics.IsChecked == true;
			Chip8XConfig.disableAudio = m_CheckBox_C8DisableSound.IsChecked == true;
			Chip8XConfig.disableWrappingX = m_CheckBox_C8DisableWrappingX.IsChecked == true;
			Chip8XConfig.disableWrappingY = m_CheckBox_C8DisableYWrap.IsChecked == true;
			Chip8XConfig.enableCodeCache = m_CheckBox_C8EnableCodeCache.IsChecked == true;
			Chip8XConfig.forceHires = m_CheckBox_C8EnableHighres.IsChecked == true;
			Chip8XConfig.epicSpeed = m_CheckBox_C8EpicSpeed.IsChecked == true;
			Chip8XConfig.enableNetplay = m_CheckBox_C8PlayOnline.IsChecked == true;
			Chip8XConfig.useRecompiler = m_CheckBox_C8UseILRec.IsChecked == true;
			Chip8XConfig.use1802Recompiler = m_CheckBox_Use1802Dynarec.IsChecked == true;
			Chip8XConfig.hleMode = m_ComboBox_HLESelector.SelectedIndex;
            Chip8XConfig.loadFonts = m_Checkbox_C8XLoadFonts.IsChecked == true;
            Chip8XConfig.customLoadPoint = int.Parse(m_Textbox_C8XLoadPointAddress.Text, System.Globalization.NumberStyles.HexNumber);
            Chip8XConfig.normalBoot = m_RadioButton_C8XNormalLoadPoint.IsChecked == true;

			#endregion

			Config.SaveConfig();
		}

		public void LoadConfig()
		{
			Config.LoadConfig();

			if (File.Exists(Config.romFilePath))
			{
				m_OpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Config.romFilePath);
				m_OpenFileDialog.FileName = Config.romFilePath;
				m_TextBox_ProgramPath.Text = Config.romFilePath;
				m_Button_RunEmulator.IsEnabled = true;

				if (File.Exists(m_TextBox_ProgramPath.Text))
				{
					m_OpenFileDialog.InitialDirectory = Config.romFilePath;
					m_OpenFileDialog.FileName = m_TextBox_ProgramPath.Text;
				}
			}

			#region Chip8X
			
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(Color.FromRgb(Chip8XConfig.backgroundColorR, Chip8XConfig.backgroundColorG, Chip8XConfig.backgroundColorB));
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(Color.FromRgb(Chip8XConfig.foregroundColorR, Chip8XConfig.foregroundColorG, Chip8XConfig.foregroundColorB));
			m_CheckBox_C8AntiFlickerHack.IsChecked = Chip8XConfig.antiFlicker;
			m_CheckBox_C8DisableGraphics.IsChecked = Chip8XConfig.disableGraphics;
			m_CheckBox_C8DisableSound.IsChecked = Chip8XConfig.disableAudio;
			m_CheckBox_C8DisableWrappingX.IsChecked = Chip8XConfig.disableWrappingX;
			m_CheckBox_C8DisableYWrap.IsChecked = Chip8XConfig.disableWrappingY;
			m_CheckBox_C8EnableCodeCache.IsChecked = Chip8XConfig.enableCodeCache;
			m_CheckBox_C8EnableHighres.IsChecked = Chip8XConfig.forceHires;
			m_CheckBox_C8EpicSpeed.IsChecked = Chip8XConfig.epicSpeed;
			m_CheckBox_C8PlayOnline.IsChecked = Chip8XConfig.enableNetplay;
			m_CheckBox_C8UseILRec.IsChecked = Chip8XConfig.useRecompiler;
			m_ComboBox_HLESelector.SelectedIndex = (Chip8XConfig.hleMode > m_ComboBox_HLESelector.Items.Count) ? 0 : Chip8XConfig.hleMode;
			m_CheckBox_Use1802Dynarec.IsChecked = Chip8XConfig.use1802Recompiler;
            m_Checkbox_C8XLoadFonts.IsChecked = Chip8XConfig.loadFonts;
            m_Textbox_C8XLoadPointAddress.Text = Chip8XConfig.customLoadPoint.ToString("X4");
            m_RadioButton_C8XNormalLoadPoint.IsChecked = Chip8XConfig.normalBoot;
            m_RadioButton_C8XCustomLoadPoint.IsChecked = !Chip8XConfig.normalBoot;

			#endregion
		}

        private void InitChip8XMachine()
        {
           this.m_VM = new Chip8XMachine();

            m_VM.VideoInterface.DisableWrappingX = (this.m_CheckBox_C8DisableWrappingX.IsChecked == true);
            m_VM.VideoInterface.DisableWrappingY = (this.m_CheckBox_C8DisableYWrap.IsChecked == true);
            if (m_CheckBox_C8EnableHighres.IsChecked == true) m_VM.StartingChipMode = ChipMode.SuperChip;
            m_VM.ProcessorCore.AntiFlickerHack = (this.m_CheckBox_C8AntiFlickerHack.IsChecked == true);
            ((ChipResources)m_VM.Resources).ProgramSource = m_RomFileSource;
            ((ChipResources)m_VM.Resources).FontSource = new FileStream(Paths.Chip8FontPath, FileMode.Open, FileAccess.Read);
            ((ChipResources)m_VM.Resources).SuperFontSource = new FileStream(Paths.SChipFontPath, FileMode.Open, FileAccess.Read);
            ((ChipResources)m_VM.Resources).LoadFonts = (this.m_Checkbox_C8XLoadFonts.IsChecked == true);
            if (m_RadioButton_C8XNormalLoadPoint.IsChecked == true)
                ((ChipResources)m_VM.Resources).LoadPointAddress = Chip8XMachine.PROGRAM_ENTRY_POINT;
            else
                ((ChipResources)m_VM.Resources).LoadPointAddress = Chip8XConfig.customLoadPoint;
            m_VM.ExtraCycleSpeed = (this.m_CheckBox_C8EpicSpeed.IsChecked == true) ? 9001 : 0;
            m_VM.IsHybridDynarecEnabled = Chip8XConfig.use1802Recompiler;
            m_VM.MachineMode = (CDP1802Mode)m_ComboBox_HLESelector.SelectedIndex;
        }

		private void m_TextBox_ProgramPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (File.Exists(m_TextBox_ProgramPath.Text))
			{
				m_OpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(m_TextBox_ProgramPath.Text);
				m_OpenFileDialog.FileName = m_TextBox_ProgramPath.Text;
			}

			m_OpenFileDialog.ShowDialog();
			m_TextBox_ProgramPath.Text = m_OpenFileDialog.FileName;
			m_Button_RunEmulator.IsEnabled = true;
			Config.romFilePath = m_OpenFileDialog.FileName;
            Config.SaveConfig();
		}

		private void m_Button_RunEmulator_Click(object sender, RoutedEventArgs e)
		{
			if (m_OpenFileDialog.FileName != "")
			{
				m_RomFileSource = new FileStream(m_OpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			else
			{
				MessageBox.Show("No rom file selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			SaveConfig();

            InitChip8XMachine();

			Hide();

            RenderWindow window = new RenderWindow(m_VM);
            window.ShowDialog();

            if (m_Quitting)
                return;

            Show();
		}

		private void m_Rectangle_C8SelectedBackgroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(c);
			Chip8XConfig.backgroundColorR = c.R;
			Chip8XConfig.backgroundColorG = c.G;
			Chip8XConfig.backgroundColorB = c.B;
			Config.SaveConfig();
		}

		private void m_Rectangle_C8SelectedForegroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(c);
			Chip8XConfig.foregroundColorR = c.R;
			Chip8XConfig.foregroundColorG = c.G;
			Chip8XConfig.foregroundColorB = c.B;
			Config.SaveConfig();
		}
	}
}