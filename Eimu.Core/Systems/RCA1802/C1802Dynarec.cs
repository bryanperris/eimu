﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Eimu.Core.Systems.Chip8X;
using System.Reflection.Emit;

namespace Eimu.Core.Systems.RCA1802
{
    public delegate void MachineCall(CodeEngine engine);

    public sealed class C1802Dynarec
    {
        private Dictionary<ushort, DynamicMethod> m_CallLookup;

        public C1802Dynarec()
        {
            m_CallLookup = new Dictionary<ushort, DynamicMethod>();
        }

        public void Call(ushort address, CodeEngine engine)
        {
            //// Debugging: Skip certain syscalls
            //switch (address)
            //{
            //    case 0x40e: return;
            //    default: break;
            //}

            DynamicMethod syscall;

            if (!m_CallLookup.TryGetValue(address, out syscall))
            {
                syscall = C1802ILEmitter.CreateSyscallMethod(address, engine);
                m_CallLookup.Add(address, syscall);
            }

            ((MachineCall)syscall.CreateDelegate(typeof(MachineCall)))(engine);
        }
    }
}
