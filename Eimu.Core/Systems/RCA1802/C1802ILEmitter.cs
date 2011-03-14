using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Eimu.Core.Systems.Chip8X;
using System.Reflection;

namespace Eimu.Core.Systems.RCA1802
{
    public enum SelectedRegister
    {
        None,
        D,
        P,
        X,
        T,
        DF
    }

    public enum C1802OpCodes : byte
    {
        Sub0 = 0,
        INC = 1,
        DEC = 2,
        Sub3 = 3,
        LDA = 4,
        STR = 5,
        IRX = 6,
        Sub7 = 7,
        GLO = 8,
        GHI = 9,
        PLO = 10,
        PHI = 11,
        Sub12 = 12,
        SEP = 13,
        SEX = 14,
        Sub15 = 15
    }

    public enum C1802OpCodesSub0 : byte
    {
        IDL = 0,
        IDN = 1,
    }

    public enum C1802OpCodesSub3 : byte
    {
        BR = 0,
        BQ = 1,
        BZ = 2,
        BDF = 3,
        B1 = 4,
        B2 = 5,
        B3 = 6,
        B4 = 7,
        NBR = 8,
        BNQ = 9,
        BNZ = 10,
        BNF = 11,
        BN1 = 12,
        BN2 = 13,
        BN3 = 14,
        BN4 = 15
    }

    public enum C1802OpCodesSub7 : byte
    {
        RET = 0,
        DIS = 1,
        LDXA = 2,
        STXD = 3,
        ADC = 4,
        SDB = 5,
        SHRC = 6,
        SMB = 7,
        SAV = 8,
        MARK = 9,
        REQ = 10,
        SEQ = 11,
        ADCI = 12,
        SDBI = 13,
        SHLC = 14,
        SMBI = 15
    }

    public enum C1802OpCodesSub12 : byte
    {
        LBR = 0,
        LBQ = 1,
        LBZ = 2,
        LBDF = 3,
        NOP = 4,
        LSNQ = 5,
        LSNZ = 6,
        LSNF = 7,
        NLBR = 8,
        LBNQ = 9,
        LBNZ = 10,
        LBNF = 11,
        LSIE = 12,
        LSQ = 13,
        LSZ = 14,
        LSDF = 15
    }

    public enum C1802OpCodesSub15 : byte
    {
        LDX = 0,
        OR = 1,
        AND = 2,
        XOR = 3,
        ADD = 4,
        SD = 5,
        SHR = 6,
        SM = 7,
        LDI = 8,
        ORI = 9,
        ANI = 10,
        XRI = 11,
        ADI = 12,
        SDI = 13,
        SHL = 14,
        SMI = 15
    }

    public static class C1802ILEmitter
    {
        private static int s_LocalCount;
        private static int s_LocalOffset;
        private static ushort s_CurrentAddress;
        private static ushort s_LastAddress;
        private static ILGenerator s_ILGen;
        private static CodeEngine s_CodeEngine;
        private static Dictionary<ushort, Label> s_Labels;
        private static ushort[] s_Regs;


        public static DynamicMethod CreateSyscallMethod(ushort address, CodeEngine engine)
        {
            s_Regs = new ushort[16];

            s_CodeEngine = engine;

            DynamicMethod function = new DynamicMethod(
                "SysFunc_" + address.ToString("x"),
                typeof(void),
                new Type[] { typeof(CodeEngine) },
                typeof(C1802ILEmitter).Module);

            RunEmitter(function.GetILGenerator(), address);

            //AssemblyName assn = new AssemblyName("TestAssembly_" + address.ToString("x"));
            //AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(assn, AssemblyBuilderAccess.Save);
            //ModuleBuilder mb = ab.DefineDynamicModule(assn.Name, assn.Name + ".dll");
            //TypeBuilder tb = mb.DefineType("MyTestType");
            //ConstructorBuilder cb = tb.DefineDefaultConstructor(MethodAttributes.Public);
            //MethodBuilder mtb = tb.DefineMethod("SysFunc_" + address.ToString("x"),
            //    MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new Type[] { typeof(CodeEngine) });
            //RunEmitter(mtb.GetILGenerator(), address);
            //tb.CreateType();
            //ab.Save(assn.Name + ".dll");

            return function;
        }

        private static CdpInstruction GetInstruction(ushort address)
        {
            return new CdpInstruction(s_CodeEngine.CurrentMemory[address]);
        }

        private static void RunEmitter(ILGenerator gen, ushort address)
        {
            Console.WriteLine("Emitting function: " + address.ToString("x"));
            s_CurrentAddress = address;
            s_LocalCount = 0;
            s_LocalOffset = 0;
            s_ILGen = gen;

            // First emit fake registers that all functions can use
            EmitLocal(typeof(byte), false); // D local.0
            EmitLocal(typeof(byte), false); // P local.1
            EmitLocal(typeof(byte), false); // X local.2
            EmitLocal(typeof(byte), false); // T local.3
            EmitLocal(typeof(byte), false); // DF local.4

            // Set P to 3
            EmitByteConstant(3);
            EmitRegisterWrite(SelectedRegister.P);

            EmitOpcodes();

            EmitNop();
            s_ILGen.Emit(OpCodes.Ret);
        }

        private static void EmitOpcodes()
        {
            bool end = false;
            s_Labels = new Dictionary<ushort, Label>();
            ushort funcAddr = s_CurrentAddress;

            do
            {
                s_LocalOffset = s_LocalCount;
                s_LastAddress = s_CurrentAddress;
                CdpInstruction inst = GetInstruction(s_CurrentAddress++);

                try
                {
                    C1802OpCodes opcode = (C1802OpCodes)inst.Hi;
                    Console.Write("1802 Opcode: " + opcode.ToString());

                    Label lb = s_ILGen.DefineLabel();
                    s_ILGen.MarkLabel(lb);
                    s_Labels.Add(s_LastAddress, lb);

                    switch (opcode)
                    {
                        case C1802OpCodes.GLO: Emit_GLO(inst); break;
                        case C1802OpCodes.SEX: Emit_SEX(inst); break;
                        case C1802OpCodes.STR: Emit_STR(inst); break;
                        case C1802OpCodes.SEP: end = true; break;
                        case C1802OpCodes.LDA: Emit_LDA(inst); break;
                        case C1802OpCodes.Sub15: EmitSubOpcodes15(inst); break;
                        case C1802OpCodes.Sub0: EmitSubOpcodes0(inst); break;
                        case C1802OpCodes.Sub7: EmitSubOpcodes7(inst); break;
                        case C1802OpCodes.Sub3: EmitSubOpcodes3(inst); break;
                        case C1802OpCodes.PLO: Emit_PLO(inst); break;
                        case C1802OpCodes.INC: Emit_INC(inst); break;
                        case C1802OpCodes.GHI: Emit_GHI(inst); break;
                        case C1802OpCodes.PHI: Emit_PHI(inst); break;
                        case C1802OpCodes.DEC: Emit_DEC(inst); break;
                        default: EmitNop(); Console.Write("  ...No Emit!"); break;
                    }

                    Console.WriteLine("");

                    //if (funcAddr == 0x3f3)
                    //    EmitDumpRegsCall();
                }
                catch (ArgumentException)
                {
                    end = true;
                    continue;
                }
            }
            while (!end);

            s_Labels.Clear();
        }

        private static void EmitSubOpcodes15(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub15 opcode = (C1802OpCodesSub15)inst.Low;
                Console.Write("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub15.ADD: Emit_ADD(inst); break;
                    case C1802OpCodesSub15.LDI: Emit_LDI(inst); break;
                    default: EmitNop(); Console.Write("  ...No Emit!"); break;
                }

            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private static void EmitSubOpcodes3(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub3 opcode = (C1802OpCodesSub3)inst.Low;
                Console.Write("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub3.BNZ: Emit_BNZ(inst); break;
                    case C1802OpCodesSub3.BR: Emit_BR(inst); break;
                    default: EmitNop(); Console.Write("  ...No Emit!"); break;
                }
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private static void EmitSubOpcodes7(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub7 opcode = (C1802OpCodesSub7)inst.Low;
                Console.Write("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub7.ADCI: Emit_ADCI(inst); break;
                    default: EmitNop(); Console.Write("  ...No Emit!"); break;
                }
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private static void EmitSubOpcodes0(CdpInstruction inst)
        {
            if (inst.Low == 0)
            {
                Console.Write(" IDL");
                EmitNop(); // Set Idle mode, we don't bother with it   
            }
            else
            {
                Console.Write("  LDN (" + inst.Low.ToString("x") + ")");
                Emit_LDN(inst);
            }

        }

        public static ushort ReadFake1802Reg(CodeEngine engine, byte num)
        {
            switch (num)
            {
                //case 2: return (ushort)engine.m_Stack.Count;
                case 3: return s_CurrentAddress;
                case 5: return (ushort)engine.m_PC;
                //case 4: return 0; // VIP PC
                //case 6: return 0; // TOOD: figure out VX selection
                //case 7: return 0; // TODO: figure out VY selection
                //case 8: return (ushort)engine.m_ST; //??
                case 9: return engine.m_LastRand;
                case 10: return engine.m_IReg;
                case 11: return 0x7F00; // Return the defualt display pointer (Video RAM DMA pointer
                default: return s_Regs[num];
            }
        }

        public static void WriteFake1802Reg(CodeEngine engine, byte num, ushort value)
        {
            switch (num)
            {
                // should use workable regs anywyas, let game fill in empty vars
                case 3: s_CurrentAddress = value; break;
                case 4: s_Regs[4] = value; break;
                case 5: engine.m_PC = (int)value; break;
               // case 6: s_Regs[0] = value; break; // TOOD: figure out VX selection
                //case 7: s_Regs[0] = value; break; // TODO: figure out VY selection
                case 9: engine.m_LastRand = value; break;
                case 10: engine.m_IReg = value; break;

                default:
                    {
                        s_Regs[num] = value; break;
                        //Console.WriteLine("Emitter: Unknown reg write! (" + num.ToString("x") + ")"); break;
                    }
            }
        }

        private static Label GetLabel(ushort address)
        {
            Label lb;
            s_Labels.TryGetValue(address, out lb);
            return lb;
        }

        private static byte GetNextOperand()
        {
            return s_CodeEngine.CurrentMemory[s_CurrentAddress++];
        }

        private static byte GetMemoryByte(ushort address)
        {
            return s_CodeEngine.CurrentMemory[address];
        }

        public static void MemoryWrite(ushort address, byte value)
        {
            if (address >= 0x7F00)
            {
                // DMA into video memory
            }
            else if (address >= 0 && address < s_CodeEngine.CurrentMemory.Size)
            {
                s_CodeEngine.CurrentMemory[address] = value;
            }
            else
            {
                Console.WriteLine("1802 Dynarec: Invalid Memory Write Access " + address.ToString("x"));
            }
        }

        public static byte MemoryRead(ushort address)
        {
            if (address >= 0x7F00)
            {
                // DMA into video memory

                return 0;
            }
            else if (address >= 0 && address < s_CodeEngine.CurrentMemory.Size)
            {
                return s_CodeEngine.CurrentMemory[address];
            }
            else
            {
                Console.WriteLine("1802 Dynarec: Invalid Memory Read Access " + address.ToString("x"));
                return 0;
            }
        }

        #region Common Emit Tools

        private static void EmitLocal(Type type, bool pinned)
        {
            s_LocalCount++;
            s_ILGen.DeclareLocal(type, pinned);
        }

        private static int GetResolvedLocal(int offset)
        {
            return (s_LocalOffset + offset);
        }

        private static void EmitReadMemory()
        {
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("MemoryRead"));
        }

        private static void EmitWriteMemory()
        {
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("MemoryWrite"));
        }

        private static void EmitUshortConstant(ushort value)
        {
            s_ILGen.Emit(OpCodes.Ldc_I4, value);
            s_ILGen.Emit(OpCodes.Conv_U2);
        }

        private static void EmitByteConstant(byte value)
        {
            s_ILGen.Emit(OpCodes.Ldc_I4_S, value);
            s_ILGen.Emit(OpCodes.Conv_U1);
        }

        private static void EmitIntConstant(int value)
        {
            switch (value)
            {
                case 0: s_ILGen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: s_ILGen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: s_ILGen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: s_ILGen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: s_ILGen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: s_ILGen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: s_ILGen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: s_ILGen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: s_ILGen.Emit(OpCodes.Ldc_I4_8); break;
                default: s_ILGen.Emit(OpCodes.Ldc_I4_S, value); break;
            }
        }

        private static void EmitGeneralRegisterRead(byte selectedR)
        {
            EmitCodeEngine();
            EmitByteConstant(selectedR);
            EmitGeneralRegisterReadFunc();
        }

        private static void EmitGeneralRegisterReadFunc()
        {
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("ReadFake1802Reg"));
        }

        private static void EmitGeneralRegisterWriteFunc()
        {
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("WriteFake1802Reg"));
        }

        private static void EmitGeneralRegisterWrite(byte selectedR, ushort value)
        {
            EmitCodeEngine();
            EmitByteConstant(selectedR);
            EmitUshortConstant(value);
            EmitGeneralRegisterWriteFunc();
        }

        private static void EmitRegisterWrite(SelectedRegister reg)
        {
            switch (reg)
            {
                case SelectedRegister.D: s_ILGen.Emit(OpCodes.Stloc_0); break;
                case SelectedRegister.P: s_ILGen.Emit(OpCodes.Stloc_1); break;
                case SelectedRegister.X: s_ILGen.Emit(OpCodes.Stloc_2); break;
                case SelectedRegister.T: s_ILGen.Emit(OpCodes.Stloc_3); break;
                case SelectedRegister.DF: s_ILGen.Emit(OpCodes.Stloc_S, 4); break;
                default: break;
            }
        }

        private static void EmitRegisterRead(SelectedRegister reg)
        {
            switch (reg)
            {
                case SelectedRegister.D: s_ILGen.Emit(OpCodes.Ldloc_0); break;
                case SelectedRegister.P: s_ILGen.Emit(OpCodes.Ldloc_1); break;
                case SelectedRegister.X: s_ILGen.Emit(OpCodes.Ldloc_2); break;
                case SelectedRegister.T: s_ILGen.Emit(OpCodes.Ldloc_3); break;
                case SelectedRegister.DF: s_ILGen.Emit(OpCodes.Ldloc_S, 4); break;
                default: break;
            }
        }

        private static void EmitLowByte()
        {
            s_ILGen.Emit(OpCodes.Ldc_I4_S, (ushort)0xFF);
            s_ILGen.Emit(OpCodes.And);
            s_ILGen.Emit(OpCodes.Conv_U1);
        }

        private static void EmitHighByte()
        {
            s_ILGen.Emit(OpCodes.Ldc_I4_S, (ushort)0xFF);
            s_ILGen.Emit(OpCodes.Conv_I2);
            s_ILGen.Emit(OpCodes.And);
            EmitByteConstant(8);
            s_ILGen.Emit(OpCodes.Shr_Un);
            s_ILGen.Emit(OpCodes.Conv_U1);
        }

        private static void EmitNop()
        {
            s_ILGen.Emit(OpCodes.Nop);
        }

        private static void EmitCodeEngine()
        {
            s_ILGen.Emit(OpCodes.Ldarg_0);
        }

        private static void EmitIncrementPC()
        {
            EmitCodeEngine();
            s_ILGen.EmitCall(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("IncrementPC"), null); // Increment the PC
            s_ILGen.Emit(OpCodes.Nop);
        }

        private static void EmitTestObject()
        {
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("TestObject"));
        }

        public static void TestObject(object engine)
        {
            Console.WriteLine(engine.ToString());
        }

        public static void TestShort(ushort v)
        {
            Console.WriteLine(v.ToString());
        }

        public static void EmitDumpRegsCall()
        {
            EmitUshortConstant(s_LastAddress);
            EmitCodeEngine();
            EmitRegisterRead(SelectedRegister.D);
            EmitRegisterRead(SelectedRegister.P);
            EmitRegisterRead(SelectedRegister.X);
            EmitRegisterRead(SelectedRegister.T);
            EmitRegisterRead(SelectedRegister.DF);
            s_ILGen.Emit(OpCodes.Call, typeof(C1802ILEmitter).GetMethod("DumpRegs"));
        }

        public static void DumpRegs(ushort address, CodeEngine engine, ushort d, ushort p, ushort x, ushort t, ushort df)
        {
            Console.WriteLine("======== Register State (" + address.ToString("x") + ") =========");
            Console.WriteLine("Register D: " + d.ToString("x"));
            Console.WriteLine("Register P: " + p.ToString("x"));
            Console.WriteLine("Register X: " + x.ToString("x"));
            Console.WriteLine("Register T: " + t.ToString("x"));
            Console.WriteLine("Register DF: " + df.ToString("x"));
            Console.WriteLine("Register R0: " + ReadFake1802Reg(engine, 0).ToString("x"));
            Console.WriteLine("Register R1: " + ReadFake1802Reg(engine, 1).ToString("x"));
            Console.WriteLine("Register R2: " + ReadFake1802Reg(engine, 2).ToString("x"));
            Console.WriteLine("Register R3: " + ReadFake1802Reg(engine, 3).ToString("x"));
            Console.WriteLine("Register R4: " + ReadFake1802Reg(engine, 4).ToString("x"));
            Console.WriteLine("Register R5: " + ReadFake1802Reg(engine, 5).ToString("x"));
            Console.WriteLine("Register R6: " + ReadFake1802Reg(engine, 6).ToString("x"));
            Console.WriteLine("Register R7: " + ReadFake1802Reg(engine, 7).ToString("x"));
            Console.WriteLine("Register R8: " + ReadFake1802Reg(engine, 8).ToString("x"));
            Console.WriteLine("Register R9: " + ReadFake1802Reg(engine, 9).ToString("x"));
            Console.WriteLine("Register RA: " + ReadFake1802Reg(engine, 10).ToString("x"));
            Console.WriteLine("Register RB: " + ReadFake1802Reg(engine, 11).ToString("x"));
            Console.WriteLine("Register RC: " + ReadFake1802Reg(engine, 12).ToString("x"));
            Console.WriteLine("Register RD: " + ReadFake1802Reg(engine, 13).ToString("x"));
            Console.WriteLine("Register RE: " + ReadFake1802Reg(engine, 14).ToString("x"));
            Console.WriteLine("Register RF: " + ReadFake1802Reg(engine, 15).ToString("x"));
            Console.WriteLine("=================================");
        }

        #endregion

        #region 1802 Instructions

        private static void Emit_LDA(CdpInstruction inst)
        {
            EmitGeneralRegisterRead(inst.Low);
            EmitReadMemory();
            EmitRegisterWrite(SelectedRegister.D);
            EmitCodeEngine();
            EmitByteConstant(inst.Low);
            EmitUshortConstant(1);
            EmitGeneralRegisterRead(inst.Low);
            s_ILGen.Emit(OpCodes.Add);
            EmitGeneralRegisterWriteFunc();
            EmitNop();
        }

        private static void Emit_STR(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // address [0]
            EmitLocal(typeof(ushort), false); // value   [1]
            EmitGeneralRegisterRead(inst.Low);
            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(1));
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(1));
            EmitWriteMemory();
            EmitNop();
        }

        private static void Emit_SEX(CdpInstruction inst)
        {
            // MMM sex!
            EmitByteConstant(inst.Low);
            EmitRegisterWrite(SelectedRegister.X); // Sex X to point to a register
            EmitNop();
        }

        private static void Emit_GLO(CdpInstruction inst)
        {
            EmitGeneralRegisterRead(inst.Low);
            EmitLowByte();
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private static void Emit_ADD(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);  // result  [0]
            Label brOverflow = s_ILGen.DefineLabel();
            Label brEnd = s_ILGen.DefineLabel();

            // Load reference of the codeEngine
            EmitCodeEngine();

            // Load X onto stack
            EmitRegisterRead(SelectedRegister.X);

            // Add (Memory Byte + D)
            EmitGeneralRegisterReadFunc();
            EmitReadMemory();
            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Add);

            // Store result
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));

            // load result of math result > 255
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitUshortConstant(0xFF);
            s_ILGen.Emit(OpCodes.Cgt);

            // If (math result > 255), goto brOverflow
            s_ILGen.Emit(OpCodes.Brtrue_S, brOverflow);
            EmitNop(); // else

            // Store math result in D
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            s_ILGen.Emit(OpCodes.Conv_U1); // cast to byte
            EmitRegisterWrite(SelectedRegister.D);

            // Set DF carry flag to 0
            s_ILGen.Emit(OpCodes.Ldc_I4_0);
            EmitRegisterWrite(SelectedRegister.DF);

            // Go to end of opcode code
            EmitNop();
            s_ILGen.Emit(OpCodes.Br_S, brEnd);

            // Overflow path
            s_ILGen.MarkLabel(brOverflow);
            EmitNop();

            // Set D to result & 0xFF
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitUshortConstant(0xFF);
            s_ILGen.Emit(OpCodes.And);
            s_ILGen.Emit(OpCodes.Conv_U1); // cast to byte
            EmitRegisterWrite(SelectedRegister.D);

            // Set DF carry flag to 1
            s_ILGen.Emit(OpCodes.Ldc_I4_1);
            EmitRegisterWrite(SelectedRegister.DF);

            // End path
            s_ILGen.MarkLabel(brEnd);
            EmitNop();
        }

        private static void Emit_PLO(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // New reg value [0]

            EmitGeneralRegisterRead(inst.Low);
            EmitUshortConstant(0xFF00);
            s_ILGen.Emit(OpCodes.And);

            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Or);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));

            EmitCodeEngine();
            EmitByteConstant(inst.Low);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitGeneralRegisterWriteFunc();
        }

        private static void Emit_INC(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitByteConstant(1);
            s_ILGen.Emit(OpCodes.Add);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));
            EmitCodeEngine();
            EmitByteConstant(inst.Low);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitGeneralRegisterWriteFunc();
        }

        private static void Emit_GHI(CdpInstruction inst)
        {
            EmitGeneralRegisterRead(inst.Low);
            EmitHighByte();
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private static void Emit_ADCI(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // result       [0]
            Label brOverflow = s_ILGen.DefineLabel();
            Label brEnd = s_ILGen.DefineLabel();

            // Store result of operand + D + DF
            EmitByteConstant(GetNextOperand());
            EmitRegisterRead(SelectedRegister.DF);
            s_ILGen.Emit(OpCodes.Add);
            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Add);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));

            // Compare result > 255
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitUshortConstant(0xFF);
            s_ILGen.Emit(OpCodes.Cgt);

            // Branch path
            s_ILGen.Emit(OpCodes.Brtrue, brOverflow);

            // Not Greater Than
            EmitNop();
            EmitByteConstant(0);
            EmitRegisterWrite(SelectedRegister.DF);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitRegisterWrite(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Br_S, brEnd);

            //Else Greater Than
            s_ILGen.MarkLabel(brOverflow);
            EmitNop();
            EmitByteConstant(1);
            EmitRegisterWrite(SelectedRegister.DF);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitUshortConstant(0xFF);
            s_ILGen.Emit(OpCodes.And);
            s_ILGen.Emit(OpCodes.Conv_U1);
            EmitRegisterWrite(SelectedRegister.D);

            // End
            s_ILGen.MarkLabel(brEnd);
            EmitNop();
        }

        private static void Emit_PHI(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // New reg value [0]

            EmitUshortConstant(0xFF);
            EmitGeneralRegisterRead(inst.Low);
            s_ILGen.Emit(OpCodes.And);

            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Conv_U2);
            s_ILGen.Emit(OpCodes.Ldc_I4_S, 8);
            s_ILGen.Emit(OpCodes.Shl);
            s_ILGen.Emit(OpCodes.Or);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));

            EmitCodeEngine();
            EmitByteConstant(inst.Low);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitGeneralRegisterWriteFunc();
        }

        private static void Emit_LDI(CdpInstruction inst)
        {
            EmitByteConstant(GetNextOperand());
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private static void Emit_BNZ(CdpInstruction inst)
        {
            ushort address = Tools.Create16(inst.Hi, GetMemoryByte(s_CurrentAddress++));
            Label lb = GetLabel(address);

            // Compare D to 0
            EmitRegisterRead(SelectedRegister.D);
            EmitByteConstant(0);
            s_ILGen.Emit(OpCodes.Ceq);
            EmitNop();

            // Branch
            s_ILGen.Emit(OpCodes.Brfalse, lb);

            EmitNop();
        }

        private static void Emit_DEC(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitByteConstant(1);
            s_ILGen.Emit(OpCodes.Sub);
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));
            EmitCodeEngine();
            EmitByteConstant(inst.Low);
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitGeneralRegisterWriteFunc();
        }

        private static void Emit_LDN(CdpInstruction inst)
        {
            // Yeah baby!!!
            EmitGeneralRegisterRead(inst.Low);
            EmitReadMemory();
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private static void Emit_BR(CdpInstruction inst)
        {
            // Lets just change the address to the another section of code and keep emitting
            s_CurrentAddress = (ushort)((s_CurrentAddress & 0xFF00) | GetMemoryByte(s_CurrentAddress++));
        }

        #endregion
    }
}
