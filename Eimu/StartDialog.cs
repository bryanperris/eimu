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
using System.Reflection;
using Eimu.Core;
using Eimu.Core.CPU;
using Eimu.Core.Devices;
using Eimu.Plugins;


namespace Eimu
{
    public partial class StartDialog : Form
    {
        private MachineParamaters m_MParams;
        private OpenFileDialog m_OpenFileDialog;
        private FileStream m_RomFileSource;
        private PluginManager m_PluginManager;

        public StartDialog()
        {
            InitializeComponent();
            this.m_MParams = new MachineParamaters();
            m_OpenFileDialog = new OpenFileDialog();
            m_OpenFileDialog.Filter = "Chip8 Programs (*.ch8)|*.ch8;|Super Chip8 Programs (*.sc)|*.sc;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
            m_PluginManager = new PluginManager();
            GetPlugins();
        }

        private void GetPlugins()
        {
            this.m_PluginManager.LoadPluginsFromAssembly(Assembly.GetExecutingAssembly());

            // DLL plugins


            // Fill comboboxes
            ListPlugins(this.m_PluginManager.AudioDeviceList, this.comboBox_SelectedAudio);
            //ListPlugins<GraphicsDevice>(this.m_PluginManager.GraphicsDeviceList, this.comboBox_SelectedGraphics);
            //ListPlugins<InputDevice>(this.m_PluginManager.InputDeviceDeviceList, this.comboBox_SelectedInput);
        }

        private void ListPlugins(List<Type> deviceList, ComboBox box)
        {
            box.Items.Clear();

            if (deviceList == null || box == null)
                return;

            foreach (Type device in deviceList)
            {
                if (device.IsDefined(typeof(PluginInfo), false))
                {
                    PluginInfo info = (PluginInfo)device.GetCustomAttributes(typeof(PluginInfo), false)[0];
                    box.Items.Add(new DeviceListItem(device, info.Name + " " + info.Version));
                }
                else
                {
                    box.Items.Add(new DeviceListItem(device, device.ToString()));
                }
            }

            box.SelectedIndex = 0;
        }

        private void button_RunProgram_Click(object sender, EventArgs e)
        {
        }

        public MachineParamaters MParams
        {
            get { return this.m_MParams; }
        }

        private void button_FileBrowse_Click(object sender, EventArgs e)
        {
            m_OpenFileDialog.ShowDialog();
            m_RomFileSource = new FileStream(m_OpenFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            textBox_RomPath.Text = m_OpenFileDialog.FileName;
        }

        internal class DeviceListItem
        {
            Type m_DeviceType;
            string m_Name;

            public DeviceListItem(Type type, string name)
            {
                m_DeviceType = type;
                this.m_Name = name;
            }

            public override string ToString()
            {
                return this.m_Name;
            }

            public Type DeviceType
            {
                get { return this.m_DeviceType; }
            }
        }
    }
}
