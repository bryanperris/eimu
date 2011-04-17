using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace Eimu.Core.Dynarec
{
    public sealed class ILDynarec<TILEmitterBase> where TILEmitterBase : ILEmitterBase
    {
        private Dictionary<long, DynamicMethod> m_CodeBlocks;
        private ILEmitterBase m_Emitter;

        public ILDynarec()
        {
            m_Emitter = (ILEmitterBase)Activator.CreateInstance(typeof(TILEmitterBase));
            m_CodeBlocks = new Dictionary<long, DynamicMethod>();
        }

        public void Execute(long address, object coreState)
        {
            //// Debugging: Skip certain syscalls
            //switch (address)
            //{
            //    case 0x40e: return;
            //    default: break;
            //}

            DynamicMethod syscall;

            if (!m_CodeBlocks.TryGetValue(address, out syscall))
            {
                syscall = m_Emitter.GenerateMethod(address, coreState);
                m_CodeBlocks.Add(address, syscall);
            }

            ((EmittedCall)syscall.CreateDelegate(typeof(EmittedCall)))(coreState);
        }

        public void ClearCache()
        {
            m_CodeBlocks.Clear();
        }

        public ILEmitterBase CurrentEmitter
        {
            get { return m_Emitter; }
        }
    }
}
