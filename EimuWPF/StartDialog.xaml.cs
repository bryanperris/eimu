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
using Eimu.Core.Systems.Chip8;
using Eimu.Core.Plugin;
using Eimu.Core.Systems.Chip8.Engines;
using WPFColorPickerLib;
using Eimu.Devices;

namespace Eimu
{
	public partial class StartDialog : Window
	{
        private OpenFileDialog m_OpenFileDialog;
        private FileStream m_RomFileSource;
        private C8Machine m_VM;
        private ColorDialog colorDialog;

		public StartDialog()
		{
			this.InitializeComponent();
            m_OpenFileDialog = new OpenFileDialog();
            m_OpenFileDialog.Filter = "Chip8 Programs (*.ch8, *.c8)|*.ch8;*.c8|Super Chip8 Programs (*.sc)|*.sc;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
            GetPlugins();
            LoadConfig();
		}

        public void SaveConfig()
        {
            UpdateSelectedIndices();
            Config.FileROMPath = m_OpenFileDialog.FileName;
            //Config.SelectedAudioPlugin = m_ComboBox_C8SelectedAudioPlugin.SelectedIndex;
            //Config.SelectedGraphicsPlugin = m_ComboBox_C8SelectedGraphicsPlugin.SelectedIndex;
            //Config.SelectedInputPlugin = m_ComboBox_C8SelectedInputPlugin.SelectedIndex;
            //Config.UseInterpreter = !(bool)m_CheckBox_C8UseILRec.IsChecked;
            Config.UseInterpreter = true;
            Config.SaveConfigFile();
        }

        public void LoadConfig()
        {
            Config.UseInterpreter = true;

            Config.LoadConfigFile();

            if (File.Exists(Config.FileROMPath))
            {
                m_OpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(Config.FileROMPath);
                m_OpenFileDialog.FileName = Config.FileROMPath;
                m_TextBox_ProgramPath.Text = Config.FileROMPath;
                m_Button_RunEmulator.IsEnabled = true;
            }

            try
            {
                //m_ComboBox_C8SelectedAudioPlugin.SelectedIndex = Config.SelectedAudioPlugin;
                //m_ComboBox_C8SelectedGraphicsPlugin.SelectedIndex = Config.SelectedGraphicsPlugin;
                //m_ComboBox_C8SelectedInputPlugin.SelectedIndex = Config.SelectedInputPlugin;
            }
            catch (System.Exception)
            {

            }

            //if (Config.UseInterpreter)
            //    radioButton_CPUModeInterpreter.Checked = true;
            //else
            //    radioButton_CPUModeRecompiler.Checked = true;

            UpdateSelectedIndices();
        }

        public void SetVM(C8Machine vm)
        {
            this.m_VM = vm;
        }

        public void ShowAboutPlugin(PluginInfo info)
        {
            MessageBox.Show(info.Description, "About " + info.Name + " (" + info.Author + ")", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GetPlugins()
        {
            PluginManager.ClearPluginLists();
            PluginManager.LoadPluginsFromCallingAssembly();
            PluginManager.LoadPluginsFromFile("./Plugins");

            // Fill comboboxes
            //ListPlugins(PluginManager.AudioDeviceList, m_ComboBox_C8SelectedAudioPlugin);
            //ListPlugins(PluginManager.GraphicsDeviceList, m_ComboBox_C8SelectedGraphicsPlugin);
            //ListPlugins(PluginManager.InputDeviceDeviceList, m_ComboBox_C8SelectedInputPlugin);
        }

        private void ShowPluginConfig(Type type)
        {
            try
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                plugin.ShowConfigDialog();
            }
            catch (System.Exception)
            {
                return;
            }
        }

        private void UpdateSelectedIndices()
        {
            //PluginManager.SetSelectedPlugins(m_ComboBox_C8SelectedAudioPlugin.SelectedIndex,
            //                     m_ComboBox_C8SelectedGraphicsPlugin.SelectedIndex,
            //                     m_ComboBox_C8SelectedInputPlugin.SelectedIndex);
        }

        private void ListPlugins(List<Type> deviceTypeList, ComboBox box)
        {
            box.Items.Clear();

            if (deviceTypeList == null || box == null)
                return;

            foreach (Type deviceType in deviceTypeList)
            {
                PluginInfo info = PluginManager.GetPluginInfo(deviceType);

                if (info != null)
                {
                    box.Items.Add(info);
                }
                else
                {
                    box.Items.Add(deviceType);
                }
            }

            if (box.Items.Count > 0)
                box.SelectedIndex = 0;
            else
                box.SelectedIndex = -1;
        }

        private void m_TextBox_ProgramPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (File.Exists(m_TextBox_ProgramPath.Text))
                m_OpenFileDialog.InitialDirectory = System.IO.Path.GetFullPath(m_TextBox_ProgramPath.Text);

            m_OpenFileDialog.ShowDialog();
            m_TextBox_ProgramPath.Text = m_OpenFileDialog.FileName;
            m_TextBox_ProgramPath.FontStyle = FontStyles.Normal;
            m_Button_RunEmulator.IsEnabled = true;
            SaveConfig();

        }

        private void m_Button_C8AudioConfig_Click(object sender, RoutedEventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedAudioDevice);
        }

        private void m_Button_C8InputConfig_Click(object sender, RoutedEventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedInputDevice);
        }

        private void m_Button_C8GraphicsConfig_Click(object sender, RoutedEventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedGraphicsDevice);
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

            //if (m_ComboBox_C8SelectedAudioPlugin.SelectedIndex == -1)
            //{
            //    MessageBox.Show("No audio plugin selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //if (m_ComboBox_C8SelectedGraphicsPlugin.SelectedIndex == -1)
            //{
            //    MessageBox.Show("No graphics plugin selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //if (m_ComboBox_C8SelectedInputPlugin.SelectedIndex == -1)
            //{
            //    MessageBox.Show("No input plugin selected!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            SaveConfig();

            m_VM.SetCodeEngineType<Interpreter>();

            //this.m_VM.CurrentAudioDevice = (AudioDevice)Activator.CreateInstance(PluginManager.SelectedAudioDevice);
            //this.m_VM.CurrentGraphicsDevice = (GraphicsDevice)Activator.CreateInstance(PluginManager.SelectedGraphicsDevice);
            //this.m_VM.CurrentInputDevice = (Eimu.Core.Systems.Chip8.InputDevice)Activator.CreateInstance(PluginManager.SelectedInputDevice);

            // C8 Shit
            if (m_CheckBox_C8DisableGraphics.IsChecked == true) m_VM.CurrentGraphicsDevice = new NullGraphicsDevice(); else m_VM.CurrentGraphicsDevice = new OGLDevice();
            if (m_CheckBox_C8DisableInput.IsChecked == true) m_VM.CurrentInputDevice = new NullInputDevice(); else m_VM.CurrentInputDevice = new FormsInputDevice();
            if (m_CheckBox_C8DisableSound.IsChecked == true) m_VM.CurrentAudioDevice = new NullAudioDevice(); else m_VM.CurrentAudioDevice = new BeepAudioDevice();

            //this.m_VM.CurrentGraphicsDevice.BackgroundColor = colorDialog1.Color;
            //this.m_VM.CurrentGraphicsDevice.ForegroundColor = colorDialog2.Color;

            m_VM.SetMediaSource(m_RomFileSource);

            Hide();
        }

        private void m_Rectangle_C8SelectedBackgroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            m_Rectangle_C8SelectedBackgroundColor.Fill = new SolidColorBrush(colorDialog.SelectedColor);
        }

        private void m_Rectangle_C8SelectedForegroundColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            m_Rectangle_C8SelectedForegroundColor.Fill = new SolidColorBrush(colorDialog.SelectedColor);
        }
	}
}