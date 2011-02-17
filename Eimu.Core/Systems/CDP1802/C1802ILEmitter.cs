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
        T
    }

    public enum C1802OpCodeSet0 : byte
    {
        STPC = 0,
        DTC = 1,
        SPM2 = 2,
        SCM2 = 3,
        SPM1 = 4,
        SCM1 = 5,
        LDC = 6,
        STM = 7,
        GEC = 8,
        ETQ = 9,
        XIE = 10,
        XID = 11,
        CIE = 12
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
            s_CurrentAddress = address;
            s_LocalCount = 0;
            s_LocalOffset = 0;
            s_ILGen = gen;

            // First emit fake registers that all functions can use
            EmitLocal(typeof(byte), false); // D local.0
            EmitLocal(typeof(byte), false); // P local.1
            EmitLocal(typeof(byte), false); // X local.2
            EmitLocal(typeof(byte), false); // T local.3

            bool end = false;

            while (!end)
            {
                s_LocalOffset = s_LocalCount;
                CdpInstruction inst = GetInstruction(s_CurrentAddress++);

                switch (inst.Hi)
                {
                    case 0x4: Emit_LDA(inst); end = true; break;
                    case 0xD: end = true; break; // SEP
                    default: s_ILGen.Emit(OpCodes.Nop); break;
                }
            }

            s_ILGen.Emit(OpCodes.Nop);
            s_ILGen.Emit(OpCodes.Ret);
        }

        #region Common Emit Tools

        private static void EmitLocal(Type type, bool pinned)
        {
            s_LocalCount++;
            s_ILGen.DeclareLocal(type, pinned);
        }

        private static int GetResolveLocal(int offset)
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

        private static void EmitGeneralRegisterRead(byte selectedR)
        {
            EmitPushCodeEngine();

            switch (selectedR)
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
                default: s_ILGen.Emit(OpCodes.Ldc_I4_S, selectedR); break;
            }

            s_ILGen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("ReadFake1802Reg"));
        }

        private static void EmitGeneralRegisterWrite(byte selectedR, ushort value)
        {
            EmitPushCodeEngine();

            switch (selectedR)
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
                default: s_ILGen.Emit(OpCodes.Ldc_I4_S, selectedR); break;
            }

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

            s_ILGen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("WriteFake1802Reg"));
        }

        private static void EmitRegisterWrite(SelectedRegister reg)
        {
            switch (reg)
            {
                case SelectedRegister.D: s_ILGen.Emit(OpCodes.Stloc_0); break;
                case SelectedRegister.P: s_ILGen.Emit(OpCodes.Stloc_1); break;
                case SelectedRegister.X: s_ILGen.Emit(OpCodes.Stloc_2); break;
                case SelectedRegister.T: s_ILGen.Emit(OpCodes.Stloc_3); break;
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
                default: break;
            }
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
            s_ILGen.Emit(OpCodes.Stloc_S, GetResolveLocal(0));    // Store the read register value in $address
            EmitReadMemory(GetResolveLocal(0));  // Push read memory value on stack
            EmitRegisterWrite(SelectedRegister.D); // Pop and Store memory value in register D
            s_CurrentAddress++;
            s_ILGen.Emit(OpCodes.Nop);
        }

        #endregion
    }
}
