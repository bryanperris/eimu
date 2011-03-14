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
			this.InitializeComponent();
			m_OpenFileDialog = new OpenFileDialog();
			m_OpenFileDialog.Filter = "SChip8 Programs (*.sc, *.ch8, *.c8)|*.sc;*.ch8;*.c8;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
			LoadConfig();
		}

		public void SaveConfig()
		{
			#region SCHIP

			SchipConfig.antiFlicker = m_CheckBox_C8AntiFlickerHack.IsChecked == true;
			SchipConfig.disableTimers = m_CheckBox_C8DisableCoreTimers.IsChecked == true;
			SchipConfig.disableGraphics = m_CheckBox_C8DisableGraphics.IsChecked == true;
			SchipConfig.disableAudio = m_CheckBox_C8DisableSound.IsChecked == true;
			SchipConfig.disableWrappingX = m_CheckBox_C8DisableWrappingX.IsChecked == true;
			SchipConfig.disableWrappingY = m_CheckBox_C8DisableYWrap.IsChecked == true;
			SchipConfig.enableCodeCache = m_CheckBox_C8EnableCodeCache.IsChecked == true;
			SchipConfig.forceHires = m_CheckBox_C8EnableHighres.IsChecked == true;
			SchipConfig.epicSpeed = m_CheckBox_C8EpicSpeed.IsChecked == true;
			SchipConfig.enableNetplay = m_CheckBox_C8PlayOnline.IsChecked == true;
			SchipConfig.useRecompiler = m_CheckBox_C8UseILRec.IsChecked == true;
			SchipConfig.use1802Recompiler = m_CheckBox_Use1802Dynarec.IsChecked == true;
			SchipConfig.hleMode = m_ComboBox_HLESelector.SelectedIndex;

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
			
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(Color.FromRgb(SchipConfig.backgroundColorR, SchipConfig.backgroundColorG, SchipConfig.backgroundColorB));
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(Color.FromRgb(SchipConfig.foregroundColorR, SchipConfig.foregroundColorG, SchipConfig.foregroundColorB));
			m_CheckBox_C8AntiFlickerHack.IsChecked = SchipConfig.antiFlicker;
			m_CheckBox_C8DisableCoreTimers.IsChecked = SchipConfig.disableTimers;
			m_CheckBox_C8DisableGraphics.IsChecked = SchipConfig.disableGraphics;
			m_CheckBox_C8DisableSound.IsChecked = SchipConfig.disableAudio;
			m_CheckBox_C8DisableWrappingX.IsChecked = SchipConfig.disableWrappingX;
			m_CheckBox_C8DisableYWrap.IsChecked = SchipConfig.disableWrappingY;
			m_CheckBox_C8EnableCodeCache.IsChecked = SchipConfig.enableCodeCache;
			m_CheckBox_C8EnableHighres.IsChecked = SchipConfig.forceHires;
			m_CheckBox_C8EpicSpeed.IsChecked = SchipConfig.epicSpeed;
			m_CheckBox_C8PlayOnline.IsChecked = SchipConfig.enableNetplay;
			m_CheckBox_C8UseILRec.IsChecked = SchipConfig.useRecompiler;
			m_ComboBox_HLESelector.SelectedIndex = (SchipConfig.hleMode > m_ComboBox_HLESelector.Items.Count) ? 0 : SchipConfig.hleMode;
			m_CheckBox_Use1802Dynarec.IsChecked = SchipConfig.use1802Recompiler;

			#endregion
		}

		public void SetVM(Chip8XMachine vm)
		{
			this.m_VM = vm;
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
			//m_TextBox_ProgramPath.FontStyle = FontStyles.Normal;
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

			//m_VM.CodeEngineCore.DisableTimers = (this.m_CheckBox_C8DisableCoreTimers.IsChecked == true);

			// C8 Shit
			if (m_CheckBox_C8DisableGraphics.IsChecked == true) 
				m_VM.CurrentGraphicsDevice = new NullGraphicsDevice();
			else 
				m_VM.CurrentGraphicsDevice = new OGLDevice();

			//if (m_CheckBox_C8DisableSound.IsChecked == true) m_VM.CurrentAudioDevice = new NullAudioDevice();
			//else 
			m_VM.CurrentAudioDevice = new NullAudioDevice();

			m_VM.CurrentGraphicsDevice.BackgroundColor = SchipConfig.BackColor;
			m_VM.CurrentGraphicsDevice.ForegroundColor = SchipConfig.ForeColor;
			m_VM.CurrentGraphicsDevice.DisableWrappingX = (this.m_CheckBox_C8DisableWrappingX.IsChecked == true);
			m_VM.CurrentGraphicsDevice.DisableWrappingY = (this.m_CheckBox_C8DisableYWrap.IsChecked == true);
			m_VM.CurrentGraphicsDevice.EnableHires = (this.m_CheckBox_C8EnableHighres.IsChecked == true);
			m_VM.CurrentGraphicsDevice.EnableAntiFlickerHack = (this.m_CheckBox_C8AntiFlickerHack.IsChecked == true);
			m_VM.SetMediaSource(m_RomFileSource);
			m_VM.ExtraCycleSpeed = (this.m_CheckBox_C8EpicSpeed.IsChecked == true) ? 9001 : 0;
			m_VM.Enable1802Dyanrec = SchipConfig.use1802Recompiler;

			m_VM.HleMode = (C1802Mode)m_ComboBox_HLESelector.SelectedIndex;

			Hide();
		}

		private void m_Rectangle_C8SelectedBackgroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(c);
			SchipConfig.backgroundColorR = c.R;
			SchipConfig.backgroundColorG = c.G;
			SchipConfig.backgroundColorB = c.B;
			Config.SaveConfig();
		}

		private void m_Rectangle_C8SelectedForegroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(c);
			SchipConfig.foregroundColorR = c.R;
			SchipConfig.foregroundColorG = c.G;
			SchipConfig.foregroundColorB = c.B;
			Config.SaveConfig();
		}
	}
}