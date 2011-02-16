using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;
using Eimu.Core.Systems.SChip8;

namespace Eimu.Core.Systems.CDP1802
{
    public delegate void MachineCall(CodeEngine engine);

    public sealed class C1802Dynarec
    {
        private Dictionary<ushort, MachineCall> m_CallLookup = new Dictionary<ushort, MachineCall>();
        private CodeEngine m_CodeEngine;
        private ushort[] regs = new ushort[16];
        private int m_LocalCount;
        private int m_LocalOffset;
        private ushort m_CurrentAddress;

        public C1802Dynarec(CodeEngine engine)
        {
            m_CodeEngine = engine;
        }

        public void Call(ushort address)
        {
            MachineCall call;

            if (!m_CallLookup.TryGetValue(address, out call))
            {
                call = GenerateFunction(address);
                m_CallLookup.Add(address, call);
            }

            call(m_CodeEngine);
        }

        private MachineCall GenerateFunction(ushort address)
        {
            DynamicMethod function = new DynamicMethod(
                "SysFunc_" + address.ToString(),
                typeof(void),
                new Type[] { typeof(CodeEngine) },
                this.GetType().Module);

            EmitFunction(address, function.GetILGenerator());

            function.DefineParameter(1, ParameterAttributes.In, "codeEngine");

            //AssemblyName assn = new AssemblyName("TestAssembly");
            //AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(assn, AssemblyBuilderAccess.Save);
            //ModuleBuilder mb = ab.DefineDynamicModule(assn.Name, assn.Name + ".dll");
            //TypeBuilder tb = mb.DefineType("MyTestType");
            //ConstructorBuilder cb = tb.DefineDefaultConstructor(MethodAttributes.Public);
            //MethodBuilder mtb = tb.DefineMethod("SysFunc_" + address.ToString(),
            //    MethodAttributes.Public, CallingConventions.Standard, typeof(void), new Type[] { typeof(CodeEngine) });
            //EmitFunction(address, mtb.GetILGenerator());
            //tb.CreateType();
            //ab.Save(assn.Name + ".dll");


            return (MachineCall)function.CreateDelegate(typeof(MachineCall));
        }

        private CdpInstruction GetInstruction(ushort address)
        {
            return new CdpInstruction(m_CodeEngine.CurrentMemory[address]);
        }

        private void EmitFunction(ushort address, ILGenerator gen)
        {
            m_CurrentAddress = address;
            m_LocalCount = 0;
            m_LocalOffset = 0;

            // First emit fake registers that all functions can use
            EmitLocal(gen, typeof(byte), false); // D local.0
            EmitLocal(gen, typeof(byte), false); // P local.1
            EmitLocal(gen, typeof(byte), false); // X local.2
            EmitLocal(gen, typeof(byte), false); // T local.3

            bool end = false;

            while (!end)
            {
                m_LocalOffset = m_LocalCount;
                CdpInstruction inst = GetInstruction(m_CurrentAddress++);

                switch (inst.Hi)
                {
                    case 0x4: Emit_LDA(gen, inst); break;
                    case 0xD: end = true; break; // SEP
                    default: gen.Emit(OpCodes.Nop); break;
                }
            }

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);
        }

        private void EmitLocal(ILGenerator gen, Type type, bool pinned)
        {
            m_LocalCount++;
            gen.DeclareLocal(type, pinned);
        }

        private int GetResolveLocal(int offset)
        {
            return (m_LocalOffset + offset);
        }

        #region opcodes

        private void Emit_LDA(ILGenerator gen, CdpInstruction inst)
        {
            EmitLocal(gen, typeof(ushort), false);             // Create $address variable
            DynarecTools.EmitRRegisterRead(gen, inst.Low);     // Push selected R Registered value on stack
            gen.Emit(OpCodes.Stloc_S, GetResolveLocal(0));    // Store the read register value in $address
            DynarecTools.EmitReadMemory(gen, GetResolveLocal(0));  // Push read memory value on stack
            DynarecTools.EmitRegisterStore(gen, SelectedRegister.D); // Pop and Store memory value in register D
            m_CurrentAddress++;
            gen.Emit(OpCodes.Nop);
        }

        #endregion
    }
}
