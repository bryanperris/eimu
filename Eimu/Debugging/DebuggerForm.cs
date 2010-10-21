using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eimu.Core.CPU;
using Eimu.Core;

namespace Eimu.Debugging
{
    public partial class DebuggerForm : Form
    {
        VirtualMachine m_Machine;

        public DebuggerForm(VirtualMachine machine)
        {
            InitializeComponent();
            this.Shown += new EventHandler(DebuggerForm_Shown);
            this.VisibleChanged += new EventHandler(DebuggerForm_VisibleChanged);

            m_Machine = machine;
        }

        void DebuggerForm_VisibleChanged(object sender, EventArgs e)
        {
            DebuggerForm form = sender as DebuggerForm;

            if (!form.Visible)
            {
                RemoveMachineHooks();
            }
        }

        void DebuggerForm_Shown(object sender, EventArgs e)
        {
            AddMachineHooks();
            LoadCode();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void AddMachineHooks()
        {
        }

        private void RemoveMachineHooks()
        {
        }

        void LoadCode()
        {
            listBox_CodeView.Items.Clear();
            Memory mem = m_Machine.MachineMemory;

            for (int i = 0; i < mem.Size; i += 2)
            {
                ChipInstruction inst = new ChipInstruction(mem[i], mem[i + 1]);
                ChipOpcodes opcode = Disassembler.DecodeInstruction(inst);
                listBox_CodeView.Items.Add(new DebugInstruction(i, opcode));
            }
        }
    }
}
