using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Eimu.Core.Systems.Chip8.Engines
{
    [Serializable]
    public sealed class Interpreter : CodeEngine
    {
        private Dictionary<ChipOpcodes, OpcodeHandler> m_MethodCallTable;

        public Interpreter(Memory memory)
            : base(memory)
        {
        }

        public override void Init()
        {
            base.Init();
            m_MethodCallTable = new Dictionary<ChipOpcodes, OpcodeHandler>();
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
                if (this.m_MethodCallTable.TryGetValue(inst.Opcode, out handler))
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
            return "PC: " + m_PC.ToString();
        }

        internal delegate void OpcodeHandler(ChipInstruction instruction);

        internal class OpcodeTag : Attribute
        {
            private ChipOpcodes m_Opcode;

            public OpcodeTag(ChipOpcodes opcode)
            {
                this.m_Opcode = opcode;
            }

            public ChipOpcodes Opcode
            {
                get { return this.m_Opcode; }
            }
        }

        #region Opcodes

        // ----------------------------------------
        // Graphics Opcodes
        // ----------------------------------------
        [OpcodeTag(ChipOpcodes.Clr)]
        private void Clr(ChipInstruction inst)
        {
            OnScreenClear();
        }

        [OpcodeTag(ChipOpcodes.Drw)]
        private void Drw(ChipInstruction inst)
        {
            OnPixelSet(inst);
        }


        // -----------------------------------------
        // Math Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpcodes.Add_7)]
        void Add_7(ChipInstruction inst)
        {
            m_VRegs[inst.X] += inst.KK;
        }

        [OpcodeTag(ChipOpcodes.Add_8)]
        void Add_8(ChipInstruction inst)
        {
            ushort val = (ushort)(m_VRegs[inst.X] + m_VRegs[inst.Y]);
            m_VRegs[0xF] = (byte)((val > 255) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(val & 0x00FF);
        }

        [OpcodeTag(ChipOpcodes.Add_F)]
        void Add_F(ChipInstruction inst)
        {
            if (((int)m_IReg + (int)m_VRegs[inst.X]) >= C8Machine.MEMORY_SIZE)
            {
                m_IReg = C8Machine.MEMORY_SIZE;
                m_VRegs[0xF] = 1;
            }
            else
                m_IReg += m_VRegs[inst.X];
        }

        [OpcodeTag(ChipOpcodes.Or)]
        void Or(ChipInstruction inst)
        {
            m_VRegs[inst.X] |= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.And)]
        void And(ChipInstruction inst)
        {
            m_VRegs[inst.X] &= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Xor)]
        void Xor(ChipInstruction inst)
        {
            m_VRegs[inst.X] ^= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Sub)]
        void Sub(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] >= m_VRegs[inst.Y]) ? 1 : 0);
            m_VRegs[inst.X] -= m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Shr)]
        void Shr(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)(((m_VRegs[inst.X] & 1) == 1) ? 1 : 0);
            m_VRegs[inst.X] /= 2;
        }

        [OpcodeTag(ChipOpcodes.Subn)]
        void Subn(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.Y] >= m_VRegs[inst.X]) ? 1 : 0);
            m_VRegs[inst.X] = (byte)(m_VRegs[inst.Y] - m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Shl)]
        void Shl(ChipInstruction inst)
        {
            m_VRegs[0xF] = (byte)((m_VRegs[inst.X] & 0x80) >> 7);
            m_VRegs[inst.X] *= 2;
        }

        [OpcodeTag(ChipOpcodes.Rnd)]
        void Rnd(ChipInstruction inst)
        {
            m_VRegs[inst.X] = (byte)(m_Rand.Next(255) & inst.KK);
        }

        // -----------------------------------------
        // Jump Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpcodes.Sys)]
        void Sys(ChipInstruction inst)
        {
            // Ignored Opcode
            //Console.WriteLine("Sys call: " + inst.NNN.ToString("x"));
        }

        [OpcodeTag(ChipOpcodes.Jp_1)]
        void Jump_1(ChipInstruction inst)
        {
            m_PC = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Call)]
        void CallF(ChipInstruction inst)
        {
            m_Stack.Push((ushort)m_PC);
            m_PC = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Se_3)]
        void Se_3(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Sne_4)]
        void Sne_4(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Se_5)]
        void Se_5(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] == m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Sne_9)]
        void Sne_9(ChipInstruction inst)
        {
            if (m_VRegs[inst.X] != m_VRegs[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpcodes.Jp_B)]
        void Jp_B(ChipInstruction inst)
        {
            m_PC = (ushort)(inst.NNN + m_VRegs[0]);
        }

        [OpcodeTag(ChipOpcodes.Skp)]
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

        [OpcodeTag(ChipOpcodes.Sknp)]
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

        [OpcodeTag(ChipOpcodes.Ret)]
        void Ret(ChipInstruction inst)
        {
            m_PC = m_Stack.Pop();
        }


        // -----------------------------------------
        // Load Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpcodes.Ld_6)]
        void Load_6(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = inst.KK;
        }

        [OpcodeTag(ChipOpcodes.Ld_8)]
        void Load_8(ChipInstruction inst)
        {
            this.m_VRegs[inst.X] = this.m_VRegs[inst.Y];
        }

        [OpcodeTag(ChipOpcodes.Ld_A)]
        void Load_A(ChipInstruction inst)
        {
            this.m_IReg = inst.NNN;
        }

        [OpcodeTag(ChipOpcodes.Ld_F_07)]
        void Load_F07(ChipInstruction inst)
        {
            if (m_DT < 0)
                m_DT = 0;

            this.m_VRegs[inst.X] = (byte)this.m_DT;
        }

        [OpcodeTag(ChipOpcodes.Ld_F_0A)]
        void Load_F0A(ChipInstruction inst)
        {
            OnWaitForKey();
            m_VRegs[inst.X] = m_LastKey;
        }

        [OpcodeTag(ChipOpcodes.Ld_DT)]
        void Load_DT(ChipInstruction inst)
        {
            OnSetDelayTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Ld_ST)]
        void Load_ST(ChipInstruction inst)
        {
            OnSetSoundTimer(m_VRegs[inst.X]);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_29)]
        void Load_F29(ChipInstruction inst)
        {
            m_IReg = (ushort)(m_VRegs[inst.X] * C8Machine.FONT_SIZE);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_33)]
        void Load_F33(ChipInstruction inst)
        {
            byte val = m_VRegs[inst.X];
            m_Memory[m_IReg] = (byte)(val / 100);
            m_Memory[m_IReg + 1] = (byte)((val % 100) / 10);
            m_Memory[m_IReg + 2] = (byte)((val % 100) % 10);
        }

        [OpcodeTag(ChipOpcodes.Ld_F_55)]
        void Load_F55(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                m_Memory[m_IReg + i] = m_VRegs[i];
            }
        }

        [OpcodeTag(ChipOpcodes.Ld_F_65)]
        void Load_F65(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                m_VRegs[i] = m_Memory[m_IReg + i];
            }
        }

        [OpcodeTag(ChipOpcodes.Ld_F_75)]
        void Load_F75(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_ERegs[i] = m_VRegs[i];
        }

        [OpcodeTag(ChipOpcodes.Ld_F_85)]
        void Load_F85(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                m_VRegs[i] = m_ERegs[i];
        }

        #endregion
    }
}
