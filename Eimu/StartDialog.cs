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

        public StartDialog(VirtualMachine vm)
        {
            this.m_VM = vm;
            InitializeComponent();
            m_OpenFileDialog = new OpenFileDialog();
            m_OpenFileDialog.Filter = "Chip8 Programs (*.ch8)|*.ch8;|Super Chip8 Programs (*.sc)|*.sc;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
            GetPlugins();
            this.Text = Eimu.Properties.Resources.WindowCaption;
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
            if (m_RomFileSource == null)
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

            if (radioButton_CPUModeInterpreter.Checked)
                this.m_VM.CurrentProcessor = new Interpreter();
            else
                this.m_VM.CurrentProcessor = new Recompiler();

            PluginManager.SetSelectedPlugins(comboBox_SelectedAudio.SelectedIndex,
                                             comboBox_SelectedGraphics.SelectedIndex,
                                             comboBox_SelectedInput.SelectedIndex);

            this.m_VM.CurrentAudioDevice = (AudioDevice)Activator.CreateInstance(PluginManager.SelectedAudioDevice);
            this.m_VM.CurrentGraphicsDevice = (GraphicsDevice)Activator.CreateInstance(PluginManager.SelectedGraphicsDevice);
            this.m_VM.CurrentInputDevice = (InputDevice)Activator.CreateInstance(PluginManager.SelectedInputDevice);

            this.m_VM.LoadROM(m_RomFileSource);

            Hide();
        }

        private void button_FileBrowse_Click(object sender, EventArgs e)
        {
            m_OpenFileDialog.ShowDialog();
            if (m_OpenFileDialog.FileName != "")
            {
               m_RomFileSource = new FileStream(m_OpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                textBox_RomPath.Text = m_OpenFileDialog.FileName;
            }
        }
    }
}
