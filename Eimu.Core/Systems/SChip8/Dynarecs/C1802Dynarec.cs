using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

namespace Eimu.Core.Systems.SChip8.Dynarecs
{
    public delegate void MachineCall(CodeEngine engine);

    public sealed class C1802Dynarec
    {
        private Dictionary<ushort, MachineCall> m_CallLookup = new Dictionary<ushort, MachineCall>();
        private CodeEngine m_CodeEngine;
        private ushort[] regs = new ushort[16];
        private int m_LocalCount;

        public C1802Dynarec(CodeEngine engine)
        {
            m_CodeEngine = engine;
        }

        public void Call(HLEMode mode, ushort address)
        {
            MachineCall call;

            if (!m_CallLookup.TryGetValue(address, out call))
            {
                call = GenerateFunction(address);
                m_CallLookup.Add(address, call);
            }

            //try
            //{
                call(m_CodeEngine);
            //}
            //catch (System.Exception)
            //{

            //}
        }

        private MachineCall GenerateFunction(ushort address)
        {
            DynamicMethod function = new DynamicMethod("SysFunc_" + address.ToString(), typeof(void), new Type[] { typeof(CodeEngine) });

            EmitFunction(address, function.GetILGenerator());

            return (MachineCall)function.CreateDelegate(typeof(MachineCall));
        }

        private CdpInstruction GetInstruction(ushort address)
        {
            return new CdpInstruction(m_CodeEngine.CurrentMemory[address]);
        }

        private void EmitFunction(ushort address, ILGenerator gen)
        {
            // First emit fake registers that all functions can use
            EimtLocal(gen, typeof(byte), false); // D local.0
            EimtLocal(gen, typeof(byte), false); // P local.1
            EimtLocal(gen, typeof(byte), false); // X local.2
            EimtLocal(gen, typeof(byte), false); // T local.3


            bool end = false;

            while (!end)
            {
                CdpInstruction inst = GetInstruction(address++);

                switch (inst.Hi)
                {
                    case 0x4: Emit_LDA(gen, inst); break;
                    case 0xD: end = true; break; // SEP
                    default: break;
                }
            }

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            m_LocalCount = 0;
        }

        private void EimtLocal(ILGenerator gen, Type type, bool pinned)
        {
            m_LocalCount++;
            gen.DeclareLocal(type, pinned);
            gen.Emit(OpCodes.Nop);
        }

        private int ResolveLocalOffset(int offset)
        {
            return ((m_LocalCount - 1) + offset);
        }


        #region opcodes

        private void Emit_LDA(ILGenerator gen, CdpInstruction inst)
        {
            EimtLocal(gen, typeof(ushort), false); // ushort address;
            gen.Emit(OpCodes.Ldc_I4_0); // Push 0 on stack
            gen.Emit(OpCodes.Stloc_S, ResolveLocalOffset(0)); // Pop 0 off stack and store in local 4 (address)
            gen.Emit(OpCodes.Ldarg_1); // Push CodeEngine param into stack
            gen.Emit(OpCodes.Ldc_I4_S, inst.Low); // Push selected reg on stack
            gen.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("ReadReg"));
            gen.Emit(OpCodes.Stloc_S, ResolveLocalOffset(0)); // Store the read address to local 4 (address)
            gen.Emit(OpCodes.Ldarg_1); // Push CodeEngine param into stack
            gen.EmitCall(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_CurrentMemory"), null); // push CodeEngine.CurrentMemory
            gen.Emit(OpCodes.Ldloc_1); // push address variable to stack;
            gen.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("get_Item")); // push Memory[address]
            gen.Emit(OpCodes.Stloc_3); // Store read byte in local 3 (D)
            gen.Emit(OpCodes.Ldarg_1); // Push CodeEngine param into stack
            gen.EmitCall(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("IncrementPC"), null); // Increment the PC
            gen.Emit(OpCodes.Nop);
        }

        #endregion
    }
}
