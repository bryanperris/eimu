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
using Eimu.Core;
using Eimu.Core.CPU;
using Eimu.Core.Devices;
using System.IO;

namespace Eimu
{
    public partial class StartDialog : Form
    {
        private MachineParamaters m_MParams;

        public StartDialog()
        {
            InitializeComponent();
        }

        void GetSelectedProcessor()
        {
            this.m_MParams.CPU = new Interpreter();

            if (this.radioButton_CPUModeInterpreter.Checked)
            {
                return;
            }
            else if (this.radioButton_CPUModeRecompiler.Checked)
            {
                this.m_MParams.CPU = new Recompiler();
            }
            else
            {
                MessageBox.Show("Invalid CPU Mode selected, using interpreter as defualt");
            }
        }

        void GetSelectedGraphicsDevice()
        {
            if (radioButton_GraphicsDrawing.Checked)
            {
                this.m_MParams.Graphics = new DrawingGraphicsDevice();
            }
        }

        void GetSelectedInputDevice()
        {
            if (radioButton_InputWinForms.Checked)
            {
                m_MParams.Input = new FormsInputDevice();
            }
        }

        void GetSelectedAudioDevice()
        {
            if (radioButton_AudioBeep.Checked)
            {
                m_MParams.Audio= new BeepAudioDevice();
            }
        }

        private void button_RunProgram_Click(object sender, EventArgs e)
        {
            GetSelectedProcessor();
            GetSelectedGraphicsDevice();
            GetSelectedInputDevice();
            GetSelectedAudioDevice();
            Hide();
        }

        public MachineParamaters MParams
        {
            get { return this.m_MParams; }
        }

        private void button_FileBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Chip8 Programs (*.ch8)|*.ch8;|Super Chip8 Programs (*.sc)|*.sc;|Binary Files (*.bin)|*.bin;|All Files (*.*)|*.*;";
            dialog.ShowDialog();
            this.m_MParams.RomSource = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            textBox_RomPath.Text = dialog.FileName;
        }
    }
}
