using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Eimu.Core.Systems.SChip8;
using System.Reflection;

namespace Eimu.Core.Systems.CDP1802
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
        private static ILGenerator s_ILGen;
        private static CodeEngine s_CodeEngine;


        public static DynamicMethod CreateSyscallMethod(ushort address, CodeEngine engine)
        {
            s_CodeEngine = engine;

            DynamicMethod function = new DynamicMethod(
                "SysFunc_" + address.ToString(),
                typeof(void),
                new Type[] { typeof(CodeEngine) },
                typeof(C1802ILEmitter).Module);

            RunEmitter(function.GetILGenerator(), address);

            //AssemblyName assn = new AssemblyName("TestAssembly");
            //AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(assn, AssemblyBuilderAccess.Save);
            //ModuleBuilder mb = ab.DefineDynamicModule(assn.Name, assn.Name + ".dll");
            //TypeBuilder tb = mb.DefineType("MyTestType");
            //ConstructorBuilder cb = tb.DefineDefaultConstructor(MethodAttributes.Public);
            //MethodBuilder mtb = tb.DefineMethod("SysFunc_" + address.ToString(),
            //    MethodAttributes.Public, CallingConventions.Standard, typeof(void), new Type[] { typeof(CodeEngine) });
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

            EmitOpcodes();

            EmitNop();
            s_ILGen.Emit(OpCodes.Ret);
        }

        private static void EmitOpcodes()
        {
            bool end = false;

            do
            {
                s_LocalOffset = s_LocalCount;
                CdpInstruction inst = GetInstruction(s_CurrentAddress++);

                try
                {
                    C1802OpCodes opcode = (C1802OpCodes)inst.Hi;
                    Console.WriteLine("1802 Opcode: " + opcode.ToString());

                    switch (opcode)
                    {
                        case C1802OpCodes.GLO: Emit_GLO(inst); break;
                        case C1802OpCodes.SEX: Emit_SEX(inst); break;
                        case C1802OpCodes.SEP: end = true; break;
                        case C1802OpCodes.LDA: Emit_LDA(inst); break;
                        case C1802OpCodes.Sub15: EmitSubOpcodes15(inst); break;
                        case C1802OpCodes.Sub0: EmitSubOpcodes0(inst); break;
                        case C1802OpCodes.Sub7: EmitSubOpcodes7(inst); break;
                        default: EmitNop(); break;
                    }
                }
                catch (ArgumentException)
                {
                    end = true;
                    continue;
                }
            }
            while (!end);
        }

        private static void EmitSubOpcodes15(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub15 opcode = (C1802OpCodesSub15)inst.Low;
                Console.WriteLine("1802 Opcode (Sub 15): " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub15.ADD: Emit_ADD(inst); break;
                    default: EmitNop(); break;
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
                Console.WriteLine("1802 Opcode (Sub 7): " + opcode.ToString());

                switch (opcode)
                {
                    default: EmitNop(); break;
                }

            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private static void EmitSubOpcodes0(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub0 opcode = (C1802OpCodesSub0)inst.Low;
                Console.WriteLine("1802 Opcode (Sub 0): " + opcode.ToString());

                switch (opcode)
                {
                    default: EmitNop(); break;
                }

            }
            catch (ArgumentException)
            {
                return;
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

        private static void EmitReadMemory(int localOffset)
        {
            EmitPushCodeEngine();
            s_ILGen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_CurrentMemory")); // push CodeEngine.CurrentMemory
            s_ILGen.Emit(OpCodes.Ldloc_S, localOffset); // push address variable to stack;
            s_ILGen.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("get_Item")); // push Memory[address]

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
            EmitPushCodeEngine();
            EmitIntConstant((int)selectedR);
            EmitGeneralRegisterReadFunc();
        }

        private static void EmitGeneralRegisterReadFunc()
        {
            s_ILGen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("ReadFake1802Reg"));
        }

        private static void EmitGeneralRegisterWriteFunc()
        {
            s_ILGen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("WriteFake1802Reg"));
        }

        private static void EmitGeneralRegisterWrite(byte selectedR, ushort value)
        {
            EmitPushCodeEngine();
            EmitIntConstant((int)selectedR);
            EmitIntConstant(value);
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

        private static void EmitNop()
        {
            s_ILGen.Emit(OpCodes.Nop);
        }

        private static void EmitPushCodeEngine()
        {
            s_ILGen.Emit(OpCodes.Ldarg_0);
        }

        private static void EmitIncrementPC()
        {
            EmitPushCodeEngine();
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

        #endregion

        #region 1802 Instructions

        private static void Emit_LDA(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);             // Create $address variable
            EmitGeneralRegisterRead(inst.Low);     // Push selected R Registered value on stack
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));    // Store the read register value in $address
            EmitReadMemory(GetResolvedLocal(0));  // Push read memory value on stack
            EmitRegisterWrite(SelectedRegister.D); // Pop and Store memory value in register D
            s_CurrentAddress++;
            EmitNop();
        }

        private static void Emit_SEX(CdpInstruction inst)
        {
            // MMM sex!
            EmitIntConstant((int)inst.Low);
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
            EmitLocal(typeof(int), false);  // result     [0]
            Label brOverflow = s_ILGen.DefineLabel();
            Label brEnd = s_ILGen.DefineLabel();

            // Load reference of the codeEngine
            EmitPushCodeEngine();

            // Load X onto stack
            EmitRegisterRead(SelectedRegister.X);

            // Add (Reg value + D)
            EmitGeneralRegisterReadFunc();
            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Add_Ovf_Un);

            // Store result
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolvedLocal(0));

            // load result of math result > D
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            EmitRegisterRead(SelectedRegister.D);
            s_ILGen.Emit(OpCodes.Cgt_Un);

            // Compare bool result with true
            s_ILGen.Emit(OpCodes.Ldc_I4_1);
            s_ILGen.Emit(OpCodes.Ceq);

            // If (math result > D), goto brOverflow
            s_ILGen.Emit(OpCodes.Brtrue_S, brOverflow);
            EmitNop(); // else

            // Store math result in D
            s_ILGen.Emit(OpCodes.Ldloc_S, GetResolvedLocal(0));
            s_ILGen.Emit(OpCodes.Conv_U1); // cast to byte
            EmitRegisterWrite(SelectedRegister.D);

            // Go to end of opcode code
            EmitNop();
            s_ILGen.Emit(OpCodes.Br_S, brEnd);

            // Overflow path
            s_ILGen.MarkLabel(brOverflow);
            EmitNop();

            // Set D to 0
            s_ILGen.Emit(OpCodes.Ldc_I4_0);
            EmitRegisterWrite(SelectedRegister.D);

            // Set DF carry flag to 1
            s_ILGen.Emit(OpCodes.Ldc_I4_1);
            EmitRegisterWrite(SelectedRegister.DF);

            // End path
            s_ILGen.MarkLabel(brEnd);
            EmitNop();
        }

        #endregion
    }
}
