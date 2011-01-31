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
using Eimu.Core.Systems.SChip8;
using Eimu.Core.Systems.SChip8.Engines;
using WPFColorPickerLib;
using Eimu.Devices;

namespace Eimu
{
	public partial class StartDialog : Window
	{
		private OpenFileDialog m_OpenFileDialog;
		private FileStream m_RomFileSource;
		private SChipMachine m_VM;
		private ColorDialog colorDialog;
		private Color m_C8BackColor = Color.FromRgb(0, 0, 0);
		private Color m_C8ForeColor = Color.FromRgb(1, 1, 1);

		public StartDialog()
		{
			this.InitializeComponent();
			m_OpenFileDialog = new OpenFileDialog();
			m_OpenFileDialog.Filter = "SChip8 Programs (*.sc, *.ch8, *.c8)|*.sc;*.ch8;*.c8;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
			LoadConfig();
		}

		public void SaveConfig()
		{
			Config.C8FileROMPath = m_OpenFileDialog.FileName;
			Config.UseC8Interpreter = true;
			Config.SaveConfigFile();
		}

		public void LoadConfig()
		{
			Config.UseC8Interpreter = true;

			Config.LoadConfigFile();

			Color c = Color.FromRgb(Config.C8BackColor.Red, Config.C8BackColor.Green, Config.C8BackColor.Blue);
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(c);
			m_C8BackColor = c;

			c = Color.FromRgb(Config.C8ForeColor.Red, Config.C8ForeColor.Green, Config.C8ForeColor.Blue);
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(c);
			m_C8ForeColor = c;

			if (File.Exists(Config.C8FileROMPath))
			{
				m_OpenFileDialog.InitialDirectory = System.IO.Path.GetFullPath(Config.C8FileROMPath);
				m_OpenFileDialog.FileName = Config.C8FileROMPath;
				m_TextBox_ProgramPath.Text = Config.C8FileROMPath;
				m_Button_RunEmulator.IsEnabled = true;

				if (File.Exists(m_TextBox_ProgramPath.Text))
				{
					m_OpenFileDialog.InitialDirectory = 
					m_OpenFileDialog.FileName = m_TextBox_ProgramPath.Text;
				}
			}
		}

		public void SetVM(SChipMachine vm)
		{
			this.m_VM = vm;
		}

		private void m_TextBox_ProgramPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			if (File.Exists(m_TextBox_ProgramPath.Text))
			{
				m_OpenFileDialog.InitialDirectory = System.IO.Path.GetFullPath(m_TextBox_ProgramPath.Text);
				m_OpenFileDialog.FileName = m_TextBox_ProgramPath.Text;
			}

			m_OpenFileDialog.ShowDialog();
			m_TextBox_ProgramPath.Text = m_OpenFileDialog.FileName;
			m_TextBox_ProgramPath.FontStyle = FontStyles.Normal;
			m_Button_RunEmulator.IsEnabled = true;
			SaveConfig();

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
			if (m_CheckBox_C8DisableGraphics.IsChecked == true) m_VM.CurrentGraphicsDevice = new NullGraphicsDevice(); 
			else m_VM.CurrentGraphicsDevice = new OGLDevice();

			//if (m_CheckBox_C8DisableSound.IsChecked == true) m_VM.CurrentAudioDevice = new NullAudioDevice();
			//else 
				m_VM.CurrentAudioDevice = new NullAudioDevice();

			m_VM.CurrentGraphicsDevice.BackgroundColor = new Core.RgbColor(m_C8BackColor.R, m_C8BackColor.G, m_C8BackColor.B);
			m_VM.CurrentGraphicsDevice.ForegroundColor = new Core.RgbColor(m_C8ForeColor.R, m_C8ForeColor.G, m_C8ForeColor.B);
			m_VM.CurrentGraphicsDevice.DisableWrapping = (this.m_CheckBox_C8DisableWrapping.IsChecked == true);
			m_VM.CurrentGraphicsDevice.EnableHires = (this.m_CheckBox_C8EnableHighres.IsChecked == true);
			m_VM.CurrentGraphicsDevice.EnableAntiFlickerHack = (this.m_CheckBox_C8AntiFlickerHack.IsChecked == true);
			m_VM.SetMediaSource(m_RomFileSource);
            m_VM.ExtraCycleSpeed = (this.m_CheckBox_C8EpicSpeed.IsChecked == true) ? 9001 : 0;

			Hide();
		}

		private void m_Rectangle_C8SelectedBackgroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(c);
			m_C8BackColor = c;
			Config.C8BackColor = new Core.RgbColor(c.R, c.G, c.B);
			SaveConfig();
		}

		private void m_Rectangle_C8SelectedForegroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			colorDialog = new ColorDialog();
			colorDialog.ShowDialog();
			Color c = colorDialog.SelectedColor;
			c.A = 255;
			m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(c);
			m_C8ForeColor = c;
			Config.C8ForeColor = new Core.RgbColor(c.R, c.G, c.B);
			SaveConfig();
		}
	}
}