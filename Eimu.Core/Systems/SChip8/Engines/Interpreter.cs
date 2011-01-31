using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace Eimu.Core.Systems.SChip8.Engines
{
    [Serializable]
    public sealed class Interpreter : CodeEngine
    {
        private Dictionary<ChipOpCode, OpcodeHandler> m_MethodCallTable;

        public override void Init(Memory memory)
        {
            base.Init(memory);
            m_MethodCallTable = new Dictionary<ChipOpCode, OpcodeHandler>();
            LoadMethods();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_MethodCallTable.Clear();
            m_MethodCallTable = null;
            m_Rand = null;
        }

        public override void Call(ChipInstruction inst)
        {
            OpcodeHandler handler;
            if (m_MethodCallTable != null)
            {
                if (this.m_MethodCallTable.TryGetValue(inst.OpCode, out handler))
                {
                    handler.Invoke(inst);
                }
            }
        }

        private void LoadMethods()
        {
            MethodInfo[] infos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (MethodInfo info in infos)
            {
                object[] attrs = info.GetCustomAttributes(typeof(OpcodeTag), false);

                if (attrs.Length > 0)
                {
                    OpcodeTag tag = ((OpcodeTag)attrs[0]);
                    this.m_MethodCallTable.Add(tag.Opcode, (OpcodeHandler)Delegate.CreateDelegate(typeof(OpcodeHandler), this, info, false));
                }
            }
        }

        public override string ToString()
        {
            return "PC: " + m_PC.ToString(CultureInfo.CurrentCulture);
        }

        internal delegate void OpcodeHandler(ChipInstruction instruction);

        internal class OpcodeTag : Attribute
        {
            private ChipOpCode m_Opcode;

            public OpcodeTag(ChipOpCode opcode)
            {
                this.m_Opcode = opcode;
            }

            public ChipOpCode Opcode
            {
                get { return this.m_Opcode; }
            }
        }

        #region Opcodes

        // ----------------------------------------
        // Graphics Opcodes
        // ----------------------------------------
        [OpcodeTag(ChipOpCode.Clr)]
        private void Clr(ChipInstruction inst)
        {
            OnScreenClear();
        }

        [OpcodeTag(ChipOpCode.Drw)]
        private void Drw(ChipInstruction inst)
        {
            OnPixelSet(inst);
        }


        // -----------------------------------------
        // Math Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Add_7)]
        void Add_7(ChipInstruction inst)
        {
            m_VRegs[inst.X] += inst.KK;
        }

        [OpcodeTag(ChipOpCode.Add_8)]
        void Add_8(ChipInstruction inst)
        {
            ushort val = (ushort)(m_VRegs[inst.X] + m_VRegs[inst.Y]);
            m_VRegs[0xF] = (byte)((val > 255) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(val & 0x00FF);
        }

        [OpcodeTag(ChipOpCode.Add_F)]
        void Add_F(ChipInstruction inst)
        {
            if (((int)m_IReg + (int)m_VRegs[inst.X]) >= SChipMachine.MEMORY_SIZE)
            {
                m_IReg = SChipMachine.MEMORY_SIZE;
                m_VRegs[0xF] = 1;
            }
            else
                m_IReg += m_VRegs[inst.X];
        }

        [OpcodeTag(ChipOpCode.Or)]
        void Or(ChipInstruction inst)
        {
            m_VRegs[inst.X] |= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpCode.And)]
        void And(ChipInstruction inst)
        {
            m_VRegs[inst.X] &= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Xor)]
        void Xor(ChipInstruction inst)
        {
            m_VRegs[inst.X] ^= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Sub)]
        void Sub(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] >= m_VRegs[inst.Y]) ? 1 : 0);
            m_VRegs[inst.X] -= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Shr)]
        void Shr(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)(((m_VRegs[inst.X] & 1) == 1) ? 1 : 0);
            m_VRegs[inst.X] /= 2;
        }

        [OpcodeTag(ChipOpCode.Subn)]
        void Subn(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.Y] >= m_VRegs[inst.X]) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(m_VRegs[inst.Y] - m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Shl)]
        void Shl(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] & 0x80) >> 7);
            m_VRegs[inst.X] *= 2;
        }

        [OpcodeTag(ChipOpCode.Rnd)]
        void Rnd(ChipInstruction inst)
        {
            m_VRegs[inst.X] = (byte)(m_Rand.Next(255) & inst.KK);
        }

        // -----------------------------------------
        // Jump Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Sys)]
        void Sys(ChipInstruction inst)
        {
            // Ignored Opcode
            //Console.WriteLine("Sys call: " + inst.NNN.ToString("x"));
        }

        [OpcodeTag(ChipOpCode.Jp_1)]
        void Jump_1(ChipInstruction inst)
        {
            m_PC = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Call)]
        void CallF(ChipInstruction inst)
        {
            m_Stack.Push((ushort)m_PC);
            m_PC = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Se_3)]
        void Se_3(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Sne_4)]
        void Sne_4(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Se_5)]
        void Se_5(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Sne_9)]
        void Sne_9(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Jp_B)]
        void Jp_B(ChipInstruction inst)
        {
            m_PC = (ushort)(inst.NNN + m_VRegs[0]);
        }

        [OpcodeTag(ChipOpCode.Skp)]
        void Skp(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == m_LastKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key checked for: " + m_VRegs[inst.X].ToString());
            }
        }

        [OpcodeTag(ChipOpCode.Sknp)]
        void Sknp(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != m_LastKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key not checked for: " + m_VRegs[inst.X].ToString());
            }
        }

        [OpcodeTag(ChipOpCode.Ret)]
        void Ret(ChipInstruction inst)
        {
            m_PC = m_Stack.Pop();
        }


        // -----------------------------------------
        // Load Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Ld_6)]
        void Load_6(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = inst.KK;
        }

        [OpcodeTag(ChipOpCode.Ld_8)]
        void Load_8(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = this.m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Ld_A)]
        void Load_A(ChipInstruction inst)
        {
            this.m_IReg = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Ld_F_07)]
        void Load_F07(ChipInstruction inst)
        {
            if (m_DT < 0)
                m_DT = 0;

            this.m_VRegs[inst.X] = (byte)this.m_DT;
        }

        [OpcodeTag(ChipOpCode.Ld_F_0A)]
        void Load_F0A(ChipInstruction inst)
        {
            OnWaitForKey();
            m_VRegs[inst.X] = m_LastKey;
        }

        [OpcodeTag(ChipOpCode.Ld_DT)]
        void Load_DT(ChipInstruction inst)
        {
            OnSetDelayTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Ld_ST)]
        void Load_ST(ChipInstruction inst)
        {
            OnSetSoundTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Ld_F_29)]
        void Load_F29(ChipInstruction inst)
        {
            m_IReg = (ushort)(m_VRegs[inst.X] * SChipMachine.FONT_SIZE);
        }

        [OpcodeTag(ChipOpCode.Ld_F_33)]
        void Load_F33(ChipInstruction inst)
        {
            byte val = m_VRegs[inst.X];
            m_Memory[m_IReg] = (byte)(val / 100);
            m_Memory[m_IReg + 1] = (byte)((val % 100) / 10);
            m_Memory[m_IReg + 2] = (byte)((val % 100) % 10);
        }

        [OpcodeTag(ChipOpCode.Ld_F_55)]
        void Load_F55(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                m_Memory[m_IReg + i] = m_VRegs[i];
            }
        }

        [OpcodeTag(ChipOpCode.Ld_F_65)]
        void Load_F65(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                m_VRegs[i] = m_Memory[m_IReg + i];
            }
        }

        // -----------------------------------------
        // Super Chips
        // -----------------------------------------

        [OpcodeTag(ChipOpCode.Ld_F_75)]
        void Load_F75(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_RPLFlags[i] = m_VRegs[i];
        }

        [OpcodeTag(ChipOpCode.Ld_F_85)]
        void Load_F85(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_VRegs[i] = m_RPLFlags[i];
        }

        [OpcodeTag(ChipOpCode.extOn)]
        void ExtOn(ChipInstruction inst)
        {
            OnSuperModeChange(true);
        }

        [OpcodeTag(ChipOpCode.extOff)]
        void ExtOff(ChipInstruction inst)
        {
            OnSuperModeChange(false);
        }

        [OpcodeTag(ChipOpCode.exit)]
        void Exit(ChipInstruction inst)
        {
            return;
        }

        [OpcodeTag(ChipOpCode.Ld_F_30)]
        void Load_F30(ChipInstruction inst)
        {
            m_IReg = m_VRegs[inst.X];
        }

        #endregion
    }
}
