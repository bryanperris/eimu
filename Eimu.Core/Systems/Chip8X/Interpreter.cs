using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Threading;

namespace Eimu.Core.Systems.Chip8X.Engines
{
    [Serializable]
    public sealed class Interpreter : CodeEngine
    {
        Profiler m_Profiler;


        public Interpreter(Chip8XMachine machine) : base(machine) { }

        private Dictionary<ChipOpCode, OpcodeHandler> m_MethodCallTable;

        public override void OnInit()
        {
            m_MethodCallTable = new Dictionary<ChipOpCode, OpcodeHandler>();
            //m_Profiler = new Profiler();
            LoadMethods();
        }

        public override void OnShutdown()
        {
            //m_Profiler.DumpStats();
            m_MethodCallTable.Clear();
            m_MethodCallTable = null;
        }

        public override void Call(ChipInstruction inst)
        {
            //m_Profiler.CountOpcode(inst.OpCode);
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
            return "PC: " + PC.ToString(CultureInfo.CurrentCulture);
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

        [OpcodeTag(ChipOpCode.scrollN)]
        private void ScrollDown(ChipInstruction inst)
        {
            OnPixelScroll(2, inst.N);
        }

        [OpcodeTag(ChipOpCode.scrollL)]
        private void ScrollLeft(ChipInstruction inst)
        {
            OnPixelScroll(1, 4);
        }

        [OpcodeTag(ChipOpCode.scrollR)]
        private void ScrollRight(ChipInstruction inst)
        {
            OnPixelScroll(3, 4);
        }


        // -----------------------------------------
        // Math Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Add_7)]
        void Add_7(ChipInstruction inst)
        {
            VRegisters[inst.X] += inst.KK;
        }

        [OpcodeTag(ChipOpCode.Add_8)]
        void Add_8(ChipInstruction inst)
        {
            ushort val = (ushort)(VRegisters[inst.X] + VRegisters[inst.Y]);
            VRegisters[0xF] = (byte)((val > 255) ? 1 : 0);
            VRegisters[inst.X] = (byte)(val & 0x00FF);
        }

        [OpcodeTag(ChipOpCode.Add_F)]
        void Add_F(ChipInstruction inst)
        {
            if (((int)AddressRegister + (int)VRegisters[inst.X]) >= 0x1000)
            {
                AddressRegister = (ushort)Memory.Size;
                VRegisters[0xF] = 1;
            }
            else
                AddressRegister += VRegisters[inst.X];
        }

        [OpcodeTag(ChipOpCode.Or)]
        void Or(ChipInstruction inst)
        {
            VRegisters[inst.X] |= VRegisters[inst.Y];
        }

        [OpcodeTag(ChipOpCode.And)]
        void And(ChipInstruction inst)
        {
            VRegisters[inst.X] &= VRegisters[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Xor)]
        void Xor(ChipInstruction inst)
        {
            VRegisters[inst.X] ^= VRegisters[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Sub)]
        void Sub(ChipInstruction inst)
        {
            VRegisters[0xF] = (byte)((VRegisters[inst.X] >= VRegisters[inst.Y]) ? 1 : 0);
            VRegisters[inst.X] -= VRegisters[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Shr)]
        void Shr(ChipInstruction inst)
        {
            VRegisters[0xF] = (byte)(((VRegisters[inst.X] & 1) == 1) ? 1 : 0);
            VRegisters[inst.X] /= 2;
        }

        [OpcodeTag(ChipOpCode.Subn)]
        void Subn(ChipInstruction inst)
        {
            VRegisters[0xF] = (byte)((VRegisters[inst.Y] >= VRegisters[inst.X]) ? 1 : 0);
            VRegisters[inst.X] = (byte)(VRegisters[inst.Y] - VRegisters[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Shl)]
        void Shl(ChipInstruction inst)
        {
            VRegisters[0xF] = (byte)((VRegisters[inst.X] & 0x80) >> 7);
            VRegisters[inst.X] *= 2;
        }

        [OpcodeTag(ChipOpCode.Rnd)]
        void Rnd(ChipInstruction inst)
        {
            VRegisters[inst.X] = (byte)(this.Random.Next(255) & inst.KK);
        }

        // -----------------------------------------
        // Jump Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Sys)]
        void Sys(ChipInstruction inst)
        {
            Console.WriteLine(this.AddressRegister.ToString("x"));
        }

        [OpcodeTag(ChipOpCode.Jp_1)]
        void Jump_1(ChipInstruction inst)
        {
            PC = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Call)]
        void CallF(ChipInstruction inst)
        {
            Stack.Push((ushort)PC);
            PC = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Se_3)]
        void Se_3(ChipInstruction inst)
        {
            if (VRegisters[inst.X] == inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Sne_4)]
        void Sne_4(ChipInstruction inst)
        {
            if (VRegisters[inst.X] != inst.KK)
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Se_5)]
        void Se_5(ChipInstruction inst)
        {
            if (VRegisters[inst.X] == VRegisters[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Sne_9)]
        void Sne_9(ChipInstruction inst)
        {
            if (VRegisters[inst.X] != VRegisters[inst.Y])
            {
                IncrementPC();
            }
        }

        [OpcodeTag(ChipOpCode.Jp_B)]
        void Jp_B(ChipInstruction inst)
        {
            PC = (ushort)(inst.NNN + VRegisters[0]);
        }

        [OpcodeTag(ChipOpCode.Skp)]
        void Skp(ChipInstruction inst)
        {
            if (VRegisters[inst.X] == PressedKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key checked for: " + VRegisters[inst.X].ToString());
            }
        }

        [OpcodeTag(ChipOpCode.Sknp)]
        void Sknp(ChipInstruction inst)
        {
            if (VRegisters[inst.X] != PressedKey)
            {
                IncrementPC();
            }
            else
            {
                //Console.WriteLine("Key not checked for: " + VRegisters[inst.X].ToString());
            }
        }

        [OpcodeTag(ChipOpCode.Ret)]
        void Ret(ChipInstruction inst)
        {
            if (Stack.Count > 0)
                PC = Stack.Pop();
            else
                PC = 512;
        }


        // -----------------------------------------
        // Load Opcodes
        // -----------------------------------------
        [OpcodeTag(ChipOpCode.Ld_6)]
        void Load_6(ChipInstruction inst)
        {
            this.VRegisters[inst.X] = inst.KK;
        }

        [OpcodeTag(ChipOpCode.Ld_8)]
        void Load_8(ChipInstruction inst)
        {
            this.VRegisters[inst.X] = this.VRegisters[inst.Y];
        }

        [OpcodeTag(ChipOpCode.Ld_A)]
        void Load_A(ChipInstruction inst)
        {
            this.AddressRegister = inst.NNN;
        }

        [OpcodeTag(ChipOpCode.Ld_F_07)]
        void Load_F07(ChipInstruction inst)
        {
            if (DelayTimer < 0)
                DelayTimer = 0;

            this.VRegisters[inst.X] = (byte)this.DelayTimer;
        }

        [OpcodeTag(ChipOpCode.Ld_F_0A)]
        void Load_F0A(ChipInstruction inst)
        {
            OnWaitForKey();
            VRegisters[inst.X] = PressedKey;
        }

        [OpcodeTag(ChipOpCode.Ld_DT)]
        void Load_DT(ChipInstruction inst)
        {
            OnSetDelayTimer(VRegisters[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Ld_ST)]
        void Load_ST(ChipInstruction inst)
        {
            OnSetSoundTimer(VRegisters[inst.X]);
        }

        [OpcodeTag(ChipOpCode.Ld_F_29)]
        void Load_F29(ChipInstruction inst)
        {
            AddressRegister = (ushort)(VRegisters[inst.X] * 5);
        }

        [OpcodeTag(ChipOpCode.Ld_F_33)]
        void Load_F33(ChipInstruction inst)
        {
            byte val = VRegisters[inst.X];
            Memory[AddressRegister] = (byte)(val / 100);
            Memory[AddressRegister + 1] = (byte)((val % 100) / 10);
            Memory[AddressRegister + 2] = (byte)((val % 100) % 10);
        }

        [OpcodeTag(ChipOpCode.Ld_F_55)]
        void Load_F55(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                Memory[AddressRegister + i] = VRegisters[i];
            }
        }

        [OpcodeTag(ChipOpCode.Ld_F_65)]
        void Load_F65(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
            {
                VRegisters[i] = Memory[AddressRegister + i];
            }
        }

        // -----------------------------------------
        // Super Chips
        // -----------------------------------------

        [OpcodeTag(ChipOpCode.Ld_F_75)]
        void Load_F75(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                RPLRegisters[i] = VRegisters[i];
        }

        [OpcodeTag(ChipOpCode.Ld_F_85)]
        void Load_F85(ChipInstruction inst)
        {
            for (int i = 0; i <= inst.X; i++)
                VRegisters[i] = RPLRegisters[i];
        }

        [OpcodeTag(ChipOpCode.extOn)]
        void ExtOn(ChipInstruction inst)
        {
            VideoInterface.Initialize(ChipMode.SuperChip);
        }

        [OpcodeTag(ChipOpCode.extOff)]
        void ExtOff(ChipInstruction inst)
        {
            VideoInterface.Initialize(ChipMode.Chip8);
        }

        [OpcodeTag(ChipOpCode.exit)]
        void Exit(ChipInstruction inst)
        {
            return;
        }

        [OpcodeTag(ChipOpCode.Ld_F_30)]
        void Load_F30(ChipInstruction inst)
        {
            AddressRegister = (ushort)(80 + (VRegisters[inst.X] * 10));
        }

        #endregion
    }
}
