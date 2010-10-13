/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Eimu.Core;
using Eimu.Core.CPU;
using Eimu.Core.Devices;
using Eimu.Plugins;


namespace Eimu
{
    public partial class StartDialog : Form
    {
        private OpenFileDialog m_OpenFileDialog;
        private FileStream m_RomFileSource;
        private VirtualMachine m_VM;

        public StartDialog()
        {
            InitializeComponent();
            m_OpenFileDialog = new OpenFileDialog();
            m_OpenFileDialog.Filter = "Chip8 Programs (*.ch8, *.c8)|*.ch8;*.c8|Super Chip8 Programs (*.sc)|*.sc;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
            GetPlugins();
            this.Text = Eimu.Properties.Resources.WindowCaption;
            LoadConfig();
        }

        public void SaveConfig()
        {
            UpdateSelectedIndices();
            Config.FileROMPath = m_OpenFileDialog.FileName;
            Config.SelectedAudioPlugin = comboBox_SelectedAudio.SelectedIndex;
            Config.SelectedGraphicsPlugin = comboBox_SelectedGraphics.SelectedIndex;
            Config.SelectedInputPlugin = comboBox_SelectedInput.SelectedIndex;
            Config.UseInterpreter = radioButton_CPUModeInterpreter.Checked;
            Config.SaveConfigFile();
        }

        public void LoadConfig()
        {
            Config.LoadConfigFile();

            if (File.Exists(Config.FileROMPath))
            {
                m_OpenFileDialog.InitialDirectory = Path.GetDirectoryName(Config.FileROMPath);
                m_OpenFileDialog.FileName = Config.FileROMPath;
                textBox_RomPath.Text = Config.FileROMPath;
                button_RunProgram.Enabled = true;
            }

            comboBox_SelectedAudio.SelectedIndex = Config.SelectedAudioPlugin;
            comboBox_SelectedGraphics.SelectedIndex = Config.SelectedGraphicsPlugin;
            comboBox_SelectedInput.SelectedIndex = Config.SelectedInputPlugin;

            if (Config.UseInterpreter)
                radioButton_CPUModeInterpreter.Checked = true;
            else
                radioButton_CPUModeRecompiler.Checked = true;

            UpdateSelectedIndices();
        }

        public void SetVM(VirtualMachine vm)
        {
            this.m_VM = vm;
        }

        public void ShowAboutPlugin(PluginInfo info)
        {
            MessageBox.Show(info.Description, "About " + info.Name + " (" + info.Author + ")", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GetPlugins()
        {
            PluginManager.ClearPluginLists();
            PluginManager.LoadPluginsFromCallingAssembly();
            PluginManager.LoadPluginsFromFile("./Plugins");

            // Fill comboboxes
            ListPlugins(PluginManager.AudioDeviceList, this.comboBox_SelectedAudio);
            ListPlugins(PluginManager.GraphicsDeviceList, this.comboBox_SelectedGraphics);
            ListPlugins(PluginManager.InputDeviceDeviceList, this.comboBox_SelectedInput);
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
            PluginManager.SetSelectedPlugins(comboBox_SelectedAudio.SelectedIndex,
                                 comboBox_SelectedGraphics.SelectedIndex,
                                 comboBox_SelectedInput.SelectedIndex);
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

        private void button_RunProgram_Click(object sender, EventArgs e)
        {
            if (m_OpenFileDialog.FileName != "")
            {
                m_RomFileSource = new FileStream(m_OpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            else
            {
                MessageBox.Show("No rom file selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox_SelectedAudio.SelectedIndex == -1)
            {
                MessageBox.Show("No audio plugin selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBox_SelectedGraphics.SelectedIndex == -1)
            {
                MessageBox.Show("No graphics plugin selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; 
            }

            if (comboBox_SelectedInput.SelectedIndex == -1)
            {
                MessageBox.Show("No input plugin selected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveConfig();

            if (radioButton_CPUModeInterpreter.Checked)
                this.m_VM.CurrentProcessor = new Interpreter();
            else
                this.m_VM.CurrentProcessor = new Recompiler();



            this.m_VM.CurrentAudioDevice = (AudioDevice)Activator.CreateInstance(PluginManager.SelectedAudioDevice);
            this.m_VM.CurrentGraphicsDevice = (GraphicsDevice)Activator.CreateInstance(PluginManager.SelectedGraphicsDevice);
            this.m_VM.CurrentInputDevice = (InputDevice)Activator.CreateInstance(PluginManager.SelectedInputDevice);

            this.m_VM.LoadROM(m_RomFileSource);

            m_RomFileSource.Close();

            Hide();
        }

        private void button_FileBrowse_Click(object sender, EventArgs e)
        {
            m_OpenFileDialog.ShowDialog();
            textBox_RomPath.Text = m_OpenFileDialog.FileName;
            button_RunProgram.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowAboutPlugin((PluginInfo)comboBox_SelectedInput.SelectedItem);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowAboutPlugin((PluginInfo)comboBox_SelectedAudio.SelectedItem);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowAboutPlugin((PluginInfo)comboBox_SelectedGraphics.SelectedItem);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedInputDevice);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedAudioDevice);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ShowPluginConfig(PluginManager.SelectedGraphicsDevice);
        }

        private void textBox_RomPath_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                m_OpenFileDialog.FileName = files[0];
                textBox_RomPath.Text = files[0];
            }

        }

        private void textBox_RomPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
        }
    }
}
