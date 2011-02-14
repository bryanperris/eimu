using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

namespace Eimu.Core.Systems.SChip8.Dynarecs
{
    public sealed class C1802Dynarec
    {
        private Dictionary<ushort, MachineCall> m_CallLookup = new Dictionary<ushort, MachineCall>();
        private CodeEngine m_CodeEngine;
        private ushort[] regs = new ushort[16];

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

            try
            {
                call(m_CodeEngine);
            }
            catch (System.Exception)
            {

            }
        }

        private MachineCall GenerateFunction(ushort address)
        {
            DynamicMethod function = new DynamicMethod("SysFunc_" + address.ToString(), typeof(void), new Type[] { typeof(CodeEngine) });

            EmitCode(address, function.GetILGenerator());

            return (MachineCall)function.CreateDelegate(typeof(MachineCall));
        }

        private CdpInstruction GetInstruction(ushort address)
        {
            return new CdpInstruction(m_CodeEngine.CurrentMemory[address]);
        }

        private ushort ReadReg(byte num)
        {
            switch (num)
            {
                case 0: return regs[0];
                case 1: return regs[1];
                case 2: return (ushort)m_CodeEngine.m_Stack.Count;
                default: return 0;
            }
        }

        private void EmitCode(ushort address, ILGenerator gen)
        {

            // First emit fake registers that all functions can use
            gen.DeclareLocal(typeof(byte), false); // D local.0
            gen.DeclareLocal(typeof(byte), false); // P local.1
            gen.DeclareLocal(typeof(byte), false); // X local.2
            gen.DeclareLocal(typeof(byte), false); // T local.3



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
        }


        #region opcodes

        private void Emit_LDA(ILGenerator gen, CdpInstruction inst)
        {
            // Load Selected Reg Byte on stack
            gen.Emit(OpCodes.Ldc_I4_S, inst.Low);

            // Call the GetReg value
            gen.EmitCall(OpCodes.Call, this.GetType().GetMethod("ReadReg"), new Type[] { typeof(byte) });

            // NOP
            gen.Emit(OpCodes.Nop);

            // Convert value to int
            gen.Emit(OpCodes.Conv_I4);

            // Call Load Memory
            gen.EmitCall(OpCodes.Call, m_CodeEngine.CurrentMemory.GetType().GetMethod("GetByte"), new Type[] { typeof(int) });

            // NOP
            gen.Emit(OpCodes.Nop);

            // Push the retuend valued to D
            gen.Emit(OpCodes.Stloc_0);

            // NOP
            gen.Emit(OpCodes.Nop);
        }

        #endregion
    }
}
