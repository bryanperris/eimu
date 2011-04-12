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

		public StartDialog()
		{
			InitializeComponent();
			m_OpenFileDialog = new OpenFileDialog();
			m_OpenFileDialog.Filter = "Chip8X Programs (*.sc, *.ch8, *.c8)|*.sc;*.ch8;*.c8;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
			LoadConfig();
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

			#region SCHIP
			
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
            ((ChipResources)m_VM.Resources).FontSource = new FileStream(Chip8XConfig.Chip8FontPath, FileMode.Open, FileAccess.Read);
            ((ChipResources)m_VM.Resources).SuperFontSource = new FileStream(Chip8XConfig.SChipFontPath, FileMode.Open, FileAccess.Read);
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