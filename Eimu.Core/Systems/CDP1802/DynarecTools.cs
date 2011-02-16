using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using Eimu.Core.Systems.SChip8;

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

    public static class DynarecTools
    {
        public static void EmitReadMemory(ILGenerator gen, int localOffset)
        {
            EmitPushCodeEngine(gen);
            gen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_CurrentMemory")); // push CodeEngine.CurrentMemory
            gen.Emit(OpCodes.Ldloc_S, localOffset); // push address variable to stack;
            gen.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("get_Item")); // push Memory[address]

        }

        public static void EmitRRegisterRead(ILGenerator gen, byte selectedR)
        {
            EmitPushCodeEngine(gen); 

            switch (selectedR)
            {
                case 0: gen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: gen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: gen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: gen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: gen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: gen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: gen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: gen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: gen.Emit(OpCodes.Ldc_I4_8); break;
                default: gen.Emit(OpCodes.Ldc_I4_S, selectedR); break;
            }

            gen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("ReadFake1802Reg"));
        }

        public static void EmitRRegisterWrite(ILGenerator gen, byte selectedR, ushort value)
        {
            EmitPushCodeEngine(gen);

            switch (selectedR)
            {
                case 0: gen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: gen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: gen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: gen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: gen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: gen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: gen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: gen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: gen.Emit(OpCodes.Ldc_I4_8); break;
                default: gen.Emit(OpCodes.Ldc_I4_S, selectedR); break;
            }

            switch (value)
            {
                case 0: gen.Emit(OpCodes.Ldc_I4_0); break;
                case 1: gen.Emit(OpCodes.Ldc_I4_1); break;
                case 2: gen.Emit(OpCodes.Ldc_I4_2); break;
                case 3: gen.Emit(OpCodes.Ldc_I4_3); break;
                case 4: gen.Emit(OpCodes.Ldc_I4_4); break;
                case 5: gen.Emit(OpCodes.Ldc_I4_5); break;
                case 6: gen.Emit(OpCodes.Ldc_I4_6); break;
                case 7: gen.Emit(OpCodes.Ldc_I4_7); break;
                case 8: gen.Emit(OpCodes.Ldc_I4_8); break;
                default: gen.Emit(OpCodes.Ldc_I4_S, value); break;
            }

            gen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("WriteFake1802Reg"));
        }

        public static void EmitRegisterStore(ILGenerator gen, SelectedRegister reg)
        {
            switch (reg)
            {
                case SelectedRegister.D: gen.Emit(OpCodes.Stloc_0); break;
                case SelectedRegister.P: gen.Emit(OpCodes.Stloc_1); break;
                case SelectedRegister.X: gen.Emit(OpCodes.Stloc_2); break;
                case SelectedRegister.T: gen.Emit(OpCodes.Stloc_3); break;
                default: break;
            }
        }

        public static void EmitRegisterLoad(ILGenerator gen, SelectedRegister reg)
        {
            switch (reg)
            {
                case SelectedRegister.D: gen.Emit(OpCodes.Ldloc_0); break;
                case SelectedRegister.P: gen.Emit(OpCodes.Ldloc_1); break;
                case SelectedRegister.X: gen.Emit(OpCodes.Ldloc_2); break;
                case SelectedRegister.T: gen.Emit(OpCodes.Ldloc_3); break;
                default: break;
            }
        }

        public static void EmitPushCodeEngine(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);
        }

        public static void EmitIncrementPC(ILGenerator gen)
        {
            EmitPushCodeEngine(gen);
            gen.EmitCall(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("IncrementPC"), null); // Increment the PC
            gen.Emit(OpCodes.Nop);
        }

        public static void EmitTestObject(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, typeof(DynarecTools).GetMethod("TestObject"));
        }

        public static void TestObject(object engine)
        {
            Console.WriteLine(engine.ToString());
        }
    }
}
