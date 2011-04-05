using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace Eimu.Core.Dynarec
{
    public abstract class ILEmitterBase
    {
        private int m_LocalCount;
        private int m_LocalOffset;
        private long m_CurrentAddress;
        private long m_LastAddress;
        private ILGenerator m_CodeGenerator;
        private Dictionary<long, Label> m_Labels;
        private bool m_DumpCode = false;

        public DynamicMethod GenerateMethod(long address, object coreState)
        {
            ResetRecompilerState();

            DynamicMethod function = new DynamicMethod(
                "SysFunc_" + address.ToString("X8"),
                typeof(void),
                new Type[] { typeof(object) },
                typeof(ILEmitterBase).Module);

            m_CodeGenerator = function.GetILGenerator();

            ResetRecompilerState();
            m_CurrentAddress = address;
            RunEmitter(address, coreState);

            if (m_DumpCode)
            {
                AssemblyName assn = new AssemblyName("GeneratedCode_" + address.ToString("X8"));
                AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(assn, AssemblyBuilderAccess.Save);
                ModuleBuilder mb = ab.DefineDynamicModule(assn.Name, assn.Name + ".dll");
                TypeBuilder tb = mb.DefineType("MyTestType");
                ConstructorBuilder cb = tb.DefineDefaultConstructor(MethodAttributes.Public);
                MethodBuilder mtb = tb.DefineMethod("SysFunc_" + address.ToString("x"),
                    MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(void), new Type[] { coreState.GetType() });
                m_CodeGenerator = mtb.GetILGenerator();
                ResetRecompilerState();
                m_CurrentAddress = address;
                RunEmitter(address, coreState);
                tb.CreateType();
                ab.Save(assn.Name + ".dll");
            }

            return function;
        }

        protected abstract void RunEmitter(long address, object coreState);

        protected void ResetRecompilerState()
        {
            m_LocalCount = 0;
            m_LocalOffset = 0;
            m_CurrentAddress = 0;
            m_LastAddress = 0;
            m_Labels = new Dictionary<long, Label>();
        }

        protected void SaveCurrentAddress()
        {
            m_LastAddress = m_CurrentAddress;
        }

        protected void UpdateLocalOffsetBase()
        {
            m_LocalOffset = m_LocalCount;
        }

        protected void MarkLabel()
        {
            Label lb = m_CodeGenerator.DefineLabel();
            m_CodeGenerator.MarkLabel(lb);
            m_Labels.Add(m_LastAddress, lb);
        }

        protected Label GetLabel(ushort address)
        {
            Label lb;
            m_Labels.TryGetValue(address, out lb);
            return lb;
        }

        #region Common Emit Tools

        protected void EmitLocal(Type type, bool pinned)
        {
            m_LocalCount++;
            m_CodeGenerator.DeclareLocal(type, pinned);
        }

        protected int GetResolvedLocal(int offset)
        {
            return (m_LocalOffset + offset);
        }

        protected void EmitUshortConstant(ushort value)
        {
            m_CodeGenerator.Emit(OpCodes.Ldc_I4, value);
            m_CodeGenerator.Emit(OpCodes.Conv_U2);
        }

        protected void EmitByteConstant(byte value)
        {
            m_CodeGenerator.Emit(OpCodes.Ldc_I4_S, value);
            m_CodeGenerator.Emit(OpCodes.Conv_U1);
        }

        protected void EmitIntConstant(int value)
        {
            switch (value)
            {
                case 0: m_CodeGenerator.Emit(OpCodes.Ldc_I4_0); break;
                case 1: m_CodeGenerator.Emit(OpCodes.Ldc_I4_1); break;
                case 2: m_CodeGenerator.Emit(OpCodes.Ldc_I4_2); break;
                case 3: m_CodeGenerator.Emit(OpCodes.Ldc_I4_3); break;
                case 4: m_CodeGenerator.Emit(OpCodes.Ldc_I4_4); break;
                case 5: m_CodeGenerator.Emit(OpCodes.Ldc_I4_5); break;
                case 6: m_CodeGenerator.Emit(OpCodes.Ldc_I4_6); break;
                case 7: m_CodeGenerator.Emit(OpCodes.Ldc_I4_7); break;
                case 8: m_CodeGenerator.Emit(OpCodes.Ldc_I4_8); break;
                default: m_CodeGenerator.Emit(OpCodes.Ldc_I4_S, value); break;
            }
        }

        protected void EmitLowByte()
        {
            m_CodeGenerator.Emit(OpCodes.Ldc_I4_S, (ushort)0xFF);
            m_CodeGenerator.Emit(OpCodes.And);
            m_CodeGenerator.Emit(OpCodes.Conv_U1);
        }

        protected void EmitHighByte()
        {
            m_CodeGenerator.Emit(OpCodes.Ldc_I4_S, (ushort)0xFF);
            m_CodeGenerator.Emit(OpCodes.Conv_I2);
            m_CodeGenerator.Emit(OpCodes.And);
            EmitByteConstant(8);
            m_CodeGenerator.Emit(OpCodes.Shr_Un);
            m_CodeGenerator.Emit(OpCodes.Conv_U1);
        }

        protected void EmitNop()
        {
            m_CodeGenerator.Emit(OpCodes.Nop);
        }

        protected void EmitCoreStateObject()
        {
            m_CodeGenerator.Emit(OpCodes.Ldarg_0);
        }

        #endregion

        #region Properties

        public bool IsCodeDumped
        {
            get { return m_DumpCode; }
            set { m_DumpCode = true; }
        }

        public ILGenerator ILGenerator
        {
            get { return m_CodeGenerator; }
        }

        protected long CurrentAddress
        {
            get { return m_CurrentAddress; }
            set { m_CurrentAddress = value; }
        }

        protected long LastAddress
        {
            get { return m_LastAddress; }
        }

        #endregion
    }
}
