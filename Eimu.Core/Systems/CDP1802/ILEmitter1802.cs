using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core.Dynarec;
using Eimu.Core.Systems.Chip8X;
using System.Reflection.Emit;

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

    public sealed class ILEmitter1802 : ILEmitterBase
    {
        private bool m_Debug = !false;
        private CodeEngine m_CodeEngine;
        private int m_DebugAddress = 0x559;
        private bool m_DebugRegs = false;
        private bool m_DebugDMA = false;

        public ILEmitter1802()
        {
        }

        private void WriteDebug(string message)
        {
            if (m_Debug)
            {
                Console.Write(message);
            }
        }

        protected override void RunEmitter(long address, object coreState)
        {
            m_CodeEngine = (CodeEngine)coreState;

            WriteDebug("1802: Emitting function: " + address.ToString("x") + "\n");

            WriteDebug("Emitting Registers D,P,X,R,DF\n");
            // First emit fake registers that all functions can use
            EmitLocal(typeof(byte), false); // D local.0
            EmitLocal(typeof(byte), false); // P local.1
            EmitLocal(typeof(byte), false); // X local.2
            EmitLocal(typeof(byte), false); // T local.3
            EmitLocal(typeof(byte), false); // DF local.4

            // Set P to 3
            WriteDebug("Setup P register fake default");
            EmitByteConstant(3);
            EmitRegisterWrite(SelectedRegister.P);

            // Set X to 2
            WriteDebug("\nSetup X register fake default");
            EmitByteConstant(2);
            EmitRegisterWrite(SelectedRegister.X);

            WriteDebug("\nEmitting Opcodes...");
            EmitOpcodes();

            //EmitReportRegsCall();

            WriteDebug("\nEmitting Return code\n");
            EmitNop();
            ILGenerator.Emit(OpCodes.Ret);
        }

        private CdpInstruction GetInstruction(ushort address)
        {
            return new CdpInstruction(m_CodeEngine.Memory[address]);
        }

        private void EmitOpcodes()
        {
            bool end = false;
            ushort funcAddr = (ushort)CurrentAddress;

            if (funcAddr == m_DebugAddress && m_DebugDMA)
                EimtMemoryDebug(true);

            do
            {
                UpdateLocalOffsetBase();
                SaveCurrentAddress();
                CdpInstruction inst = GetInstruction((ushort)(CurrentAddress++));

                try
                {
                    C1802OpCodes opcode = (C1802OpCodes)inst.Hi;
                    WriteDebug("\n1802 Opcode: " + opcode.ToString() + "(" + inst.Data.ToString("X2") + ")");

                    MarkLabel();

                    switch (opcode)
                    {
                        // All called routines must end with SEP pointing to R4!
                        case C1802OpCodes.SEP:
                            {
                                if (inst.Low == 4)
                                {
                                    end = true; break;
                                }
                                else
                                {
                                    WriteDebug(" : Warning: code doesn't point to r4, value: " + inst.Low.ToString("X1") + "\n");
                                    break;
                                }
                            } 

                        case C1802OpCodes.GLO: Emit_GLO(inst); break;
                        case C1802OpCodes.SEX: Emit_SEX(inst); break;
                        case C1802OpCodes.STR: Emit_STR(inst); break;
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
                        default: EmitNop(); WriteDebug("  ...No Emit!"); break;
                    }

                    if (funcAddr == m_DebugAddress && m_DebugRegs)
                        EmitDumpRegsCall();
                }
                catch (ArgumentException)
                {
                    end = true;
                    continue;
                }
            }
            while (!end);

            if (funcAddr == m_DebugAddress && m_DebugDMA)
                EimtMemoryDebug(false);
        }

        private void EmitSubOpcodes15(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub15 opcode = (C1802OpCodesSub15)inst.Low;
                WriteDebug("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub15.ADD: Emit_ADD(inst); break;
                    case C1802OpCodesSub15.LDI: Emit_LDI(inst); break;
                    case C1802OpCodesSub15.LDX: Emit_LDX(inst); break;
                    default: EmitNop(); WriteDebug("  ...No Emit!"); break;
                }

            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private void EmitSubOpcodes3(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub3 opcode = (C1802OpCodesSub3)inst.Low;
                WriteDebug("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub3.BNZ: Emit_BNZ(inst); break;
                    case C1802OpCodesSub3.BR: Emit_BR(inst); break;
                    default: EmitNop(); WriteDebug("  ...No Emit!"); break;
                }
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private void EmitSubOpcodes7(CdpInstruction inst)
        {
            try
            {
                C1802OpCodesSub7 opcode = (C1802OpCodesSub7)inst.Low;
                WriteDebug("  " + opcode.ToString());

                switch (opcode)
                {
                    case C1802OpCodesSub7.ADCI: Emit_ADCI(inst); break;
                    default: EmitNop(); WriteDebug("  ...No Emit!"); break;
                }
            }
            catch (ArgumentException)
            {
                return;
            }
        }

        private void EmitSubOpcodes0(CdpInstruction inst)
        {
            if (inst.Low == 0)
            {
                WriteDebug(" IDL");
                EmitNop(); // Set Idle mode, we don't bother with it   
            }
            else
            {
                WriteDebug("  LDN (" + inst.Low.ToString("x") + ")");
                Emit_LDN(inst);
            }

        }

        private byte GetNextOperand()
        {
            return m_CodeEngine.Memory[(int)(CurrentAddress++)];
        }

        private byte GetMemoryByte(ushort address)
        {
            return m_CodeEngine.Memory[(int)address];
        }

        public void MemoryWrite(CodeEngine engine, ushort address, byte value)
        {
            if (address <= m_CodeEngine.Memory.Size)
            {
                engine.Memory[address] = value;
            }
            else
            {
                WriteDebug("1802 Dynarec: Invalid Memory Write Access " + address.ToString("x") + "\n");
            }
        }

        public byte MemoryRead(CodeEngine engine, ushort address)
        {
            if (address <= m_CodeEngine.Memory.Size)
            {
                return engine.Memory[address];
            }
            else
            {
                WriteDebug("1802 Dynarec: Invalid Memory Read Access " + address.ToString("x") + "\n");
                return 0;
            }
        }

        #region Common Emit Tools

        private void EmitReadMemory(int localOffset)
        {
            EmitCoreStateObject();
            ILGenerator.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_Memory")); // push CodeEngine.Memory
            ILGenerator.Emit(OpCodes.Ldloc_S, localOffset); // push address variable to stack;
            ILGenerator.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("get_Item")); // push Memory[address]
        }

        private void EmitWriteMemory(int localOffset_Address, int localOffset_Value)
        {
            EmitCoreStateObject();
            ILGenerator.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_Memory")); // push CodeEngine.Memory
            ILGenerator.Emit(OpCodes.Ldloc_S, localOffset_Address); // push address variable to stack;
            ILGenerator.Emit(OpCodes.Ldloc_S, localOffset_Value); // push value variable to stack;
            ILGenerator.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("set_Item")); // push Memory[address]
        }

        private void EmitGeneralRegisterRead(byte selectedR)
        {
            WriteDebug(" : (read) R" + selectedR.ToString("X1"));
            EmitCoreStateObject();
            EmitByteConstant(selectedR);
            EmitGeneralRegisterReadFunc();
        }

        private void EmitGeneralRegisterWrite(byte selectedR, ushort value)
        {
            WriteDebug(" : (write) R" + selectedR.ToString("X1"));
            EmitCoreStateObject();
            EmitByteConstant(selectedR);
            EmitUshortConstant(value);
            EmitGeneralRegisterWriteFunc();
        }

        private void EmitGeneralRegisterReadFunc()
        {
            ILGenerator.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("Read1802Register"));
        }

        private void EmitGeneralRegisterWriteFunc()
        {
            ILGenerator.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("Set1802Register"));
        }

        private void EmitRegisterWrite(SelectedRegister reg)
        {
            WriteDebug(" : (write) R-" + reg.ToString());

            switch (reg)
            {
                case SelectedRegister.D: ILGenerator.Emit(OpCodes.Stloc_0); break;
                case SelectedRegister.P: ILGenerator.Emit(OpCodes.Stloc_1); break;
                case SelectedRegister.X: ILGenerator.Emit(OpCodes.Stloc_2); break;
                case SelectedRegister.T: ILGenerator.Emit(OpCodes.Stloc_3); break;
                case SelectedRegister.DF: ILGenerator.Emit(OpCodes.Stloc_S, 4); break;
                default: break;
            }
        }

        private void EmitRegisterRead(SelectedRegister reg)
        {
            WriteDebug(" : (read) R-" + reg.ToString());

            switch (reg)
            {
                case SelectedRegister.D: ILGenerator.Emit(OpCodes.Ldloc_0); break;
                case SelectedRegister.P: ILGenerator.Emit(OpCodes.Ldloc_1); break;
                case SelectedRegister.X: ILGenerator.Emit(OpCodes.Ldloc_2); break;
                case SelectedRegister.T: ILGenerator.Emit(OpCodes.Ldloc_3); break;
                case SelectedRegister.DF: ILGenerator.Emit(OpCodes.Ldloc_S, 4); break;
                default: break;
            }
        }

        private void EimtMemoryDebug(bool enabled)
        {
            EmitCoreStateObject();
            ILGenerator.Emit(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("get_Memory")); // push CodeEngine.Memory
            EmitByteConstant((byte)(enabled ? 1 : 0));
            ILGenerator.Emit(OpCodes.Callvirt, typeof(Memory).GetMethod("set_IsCDP1802AccessDebugEnabled"));
        }

        private void EmitIncrementPC()
        {
            EmitCoreStateObject();
            ILGenerator.EmitCall(OpCodes.Callvirt, typeof(CodeEngine).GetMethod("IncrementPC"), null); // Increment the PC
            ILGenerator.Emit(OpCodes.Nop);
        }

        public void EmitDumpRegsCall()
        {
            EmitUshortConstant((ushort)LastAddress);
            EmitCoreStateObject();
            EmitRegisterRead(SelectedRegister.D);
            EmitRegisterRead(SelectedRegister.P);
            EmitRegisterRead(SelectedRegister.X);
            EmitRegisterRead(SelectedRegister.T);
            EmitRegisterRead(SelectedRegister.DF);
            ILGenerator.Emit(OpCodes.Call, typeof(ILEmitter1802).GetMethod("DumpRegs"));
        }

        public void EmitReportRegsCall()
        {
            EmitCoreStateObject();
            ILGenerator.Emit(OpCodes.Call, typeof(ILEmitter1802).GetMethod("ReportRegs"));
        }

        public static void DumpRegs(ushort address, CodeEngine engine, ushort d, ushort p, ushort x, ushort t, ushort df)
        {
            Console.WriteLine("\n======== Register State (" + address.ToString("x") + ") =========");
            Console.WriteLine("Register D: " + d.ToString("x"));
            Console.WriteLine("Register P: " + p.ToString("x"));
            Console.WriteLine("Register X: " + x.ToString("x"));
            Console.WriteLine("Register T: " + t.ToString("x"));
            Console.WriteLine("Register DF: " + df.ToString("x"));
            Console.WriteLine("Register R0: " + engine.Read1802Register(0).ToString("x"));
            Console.WriteLine("Register R1: " + engine.Read1802Register(1).ToString("x"));
            Console.WriteLine("Register R2: " + engine.Read1802Register(2).ToString("x"));
            Console.WriteLine("Register R3: " + engine.Read1802Register(3).ToString("x"));
            Console.WriteLine("Register R4: " + engine.Read1802Register(4).ToString("x"));
            Console.WriteLine("Register R5: " + engine.Read1802Register(5).ToString("x"));
            Console.WriteLine("Register R6: " + engine.Read1802Register(6).ToString("x"));
            Console.WriteLine("Register R7: " + engine.Read1802Register(7).ToString("x"));
            Console.WriteLine("Register R8: " + engine.Read1802Register(8).ToString("x"));
            Console.WriteLine("Register R9: " + engine.Read1802Register(9).ToString("x"));
            Console.WriteLine("Register RA: " + engine.Read1802Register(10).ToString("x"));
            Console.WriteLine("Register RB: " + engine.Read1802Register(11).ToString("x"));
            Console.WriteLine("Register RC: " + engine.Read1802Register(12).ToString("x"));
            Console.WriteLine("Register RD: " + engine.Read1802Register(13).ToString("x"));
            Console.WriteLine("Register RE: " + engine.Read1802Register(14).ToString("x"));
            Console.WriteLine("Register RF: " + engine.Read1802Register(15).ToString("x"));
            Console.WriteLine("=================================\n");
        }

        public static void ReportRegs(CodeEngine engine)
        {
            Console.WriteLine("\n=== New Code Engine State ====");
            Console.WriteLine("PC: " + engine.Read1802Register(5).ToString("x"));
            Console.WriteLine("Timers: " + engine.Read1802Register(8).ToString("x"));
            Console.WriteLine("Address Register: " + engine.Read1802Register(10).ToString("x"));
            Console.WriteLine("==============================\n");
        }

        #endregion

        #region 1802 Instructions

        private void Emit_LDA(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitLocalStore(0);
            EmitReadMemory(GetResolvedLocal(0));
            EmitRegisterWrite(SelectedRegister.D);
            EmitCoreStateObject();
            EmitByteConstant(inst.Low);
            EmitUshortConstant(1);
            EmitGeneralRegisterRead(inst.Low);
            ILGenerator.Emit(OpCodes.Add);
            EmitGeneralRegisterWriteFunc();
            EmitNop();
        }

        private void Emit_STR(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // address [0]
            EmitLocal(typeof(ushort), false); // value   [1]
            EmitGeneralRegisterRead(inst.Low);
            EmitRegisterRead(SelectedRegister.D);
            EmitLocalStore(1);
            EmitLocalStore(0);
            EmitWriteMemory(GetResolvedLocal(0), GetResolvedLocal(1));
            EmitNop();
        }

        private void Emit_SEX(CdpInstruction inst)
        {
            // MMM sex!
            EmitByteConstant(inst.Low);
            EmitRegisterWrite(SelectedRegister.X); // Sex X to point to a register
            EmitNop();
        }

        private void Emit_GLO(CdpInstruction inst)
        {
            EmitGeneralRegisterRead(inst.Low);
            EmitLowByte();
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private void Emit_ADD(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);  // result  [0]
            EmitLocal(typeof(ushort), false);  // addr    [1]
            Label brOverflow = ILGenerator.DefineLabel();
            Label brEnd = ILGenerator.DefineLabel();

            // Load reference of the codeEngine
            EmitCoreStateObject();

            // Load X onto stack
            EmitRegisterRead(SelectedRegister.X);

            // Add (Memory Byte + D)
            EmitGeneralRegisterReadFunc();
            EmitLocalStore(1);
            EmitReadMemory(GetResolvedLocal(1));
            EmitRegisterRead(SelectedRegister.D);
            ILGenerator.Emit(OpCodes.Add);

            // Store result
            EmitLocalStore(0);

            // load result of math result > 255
            EmitLocalLoad(0);
            EmitUshortConstant(0xFF);
            ILGenerator.Emit(OpCodes.Cgt);

            // If (math result > 255), goto brOverflow
            ILGenerator.Emit(OpCodes.Brtrue_S, brOverflow);
            EmitNop(); // else

            // Store math result in D
            EmitLocalLoad(0);
            ILGenerator.Emit(OpCodes.Conv_U1); // cast to byte
            EmitRegisterWrite(SelectedRegister.D);

            // Set DF carry flag to 0
            ILGenerator.Emit(OpCodes.Ldc_I4_0);
            EmitRegisterWrite(SelectedRegister.DF);

            // Go to end of opcode code
            EmitNop();
            ILGenerator.Emit(OpCodes.Br_S, brEnd);

            // Overflow path
            ILGenerator.MarkLabel(brOverflow);
            EmitNop();

            // Set D to result & 0xFF
            EmitLocalLoad(0);
            EmitUshortConstant(0xFF);
            ILGenerator.Emit(OpCodes.And);
            ILGenerator.Emit(OpCodes.Conv_U1); // cast to byte
            EmitRegisterWrite(SelectedRegister.D);

            // Set DF carry flag to 1
            ILGenerator.Emit(OpCodes.Ldc_I4_1);
            EmitRegisterWrite(SelectedRegister.DF);

            // End path
            ILGenerator.MarkLabel(brEnd);
            EmitNop();
        }

        private void Emit_PLO(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // New reg value [0]

            EmitGeneralRegisterRead(inst.Low);
            EmitUshortConstant(0xFF00);
            ILGenerator.Emit(OpCodes.And);

            EmitRegisterRead(SelectedRegister.D);
            ILGenerator.Emit(OpCodes.Or);
            EmitLocalStore(0);

            EmitCoreStateObject();
            EmitByteConstant(inst.Low);
            WriteDebug(" (write) R" + inst.Low.ToString("X1"));
            EmitLocalLoad(0);
            EmitGeneralRegisterWriteFunc();
        }

        private void Emit_INC(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitByteConstant(1);
            ILGenerator.Emit(OpCodes.Add);
            EmitLocalStore(0);
            EmitCoreStateObject();
            EmitByteConstant(inst.Low);
            EmitLocalLoad(0);
            EmitGeneralRegisterWriteFunc();
        }

        private void Emit_GHI(CdpInstruction inst)
        {
            EmitGeneralRegisterRead(inst.Low);
            EmitHighByte();
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private void Emit_ADCI(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // result       [0]
            Label brOverflow = ILGenerator.DefineLabel();
            Label brEnd = ILGenerator.DefineLabel();

            // Store result of operand + D + DF
            EmitByteConstant(GetNextOperand());
            EmitRegisterRead(SelectedRegister.DF);
            ILGenerator.Emit(OpCodes.Add);
            EmitRegisterRead(SelectedRegister.D);
            ILGenerator.Emit(OpCodes.Add);
            EmitLocalStore(0);

            // Compare result > 255
            EmitLocalLoad(0);
            EmitUshortConstant(0xFF);
            ILGenerator.Emit(OpCodes.Cgt);

            // Branch path
            ILGenerator.Emit(OpCodes.Brtrue, brOverflow);

            // Not Greater Than
            EmitNop();
            EmitByteConstant(0);
            EmitRegisterWrite(SelectedRegister.DF);
            EmitLocalLoad(0);
            EmitRegisterWrite(SelectedRegister.D);
            ILGenerator.Emit(OpCodes.Br_S, brEnd);

            //Else Greater Than
            ILGenerator.MarkLabel(brOverflow);
            EmitNop();
            EmitByteConstant(1);
            EmitRegisterWrite(SelectedRegister.DF);
            EmitLocalLoad(0);
            EmitUshortConstant(0xFF);
            ILGenerator.Emit(OpCodes.And);
            ILGenerator.Emit(OpCodes.Conv_U1);
            EmitRegisterWrite(SelectedRegister.D);

            // End
            ILGenerator.MarkLabel(brEnd);
            EmitNop();
        }

        private void Emit_PHI(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // New reg value [0]

            EmitUshortConstant(0xFF);
            EmitGeneralRegisterRead(inst.Low);
            ILGenerator.Emit(OpCodes.And);

            EmitRegisterRead(SelectedRegister.D);
            ILGenerator.Emit(OpCodes.Conv_U2);
            ILGenerator.Emit(OpCodes.Ldc_I4_S, 8);
            ILGenerator.Emit(OpCodes.Shl);
            ILGenerator.Emit(OpCodes.Or);
            EmitLocalStore(0);

            EmitCoreStateObject();
            EmitByteConstant(inst.Low);
            EmitLocalLoad(0);
            EmitGeneralRegisterWriteFunc();
        }

        private void Emit_LDI(CdpInstruction inst)
        {
            byte val = GetNextOperand();
            WriteDebug(" " + val.ToString("X1") + " ");
            EmitByteConstant(val);
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private void Emit_BNZ(CdpInstruction inst)
        {
            ushort address = (ushort)((CurrentAddress & 0xF00) | GetMemoryByte((ushort)(CurrentAddress++)));
            Label lb = GetLabel(address);

            // Compare D to 0
            EmitRegisterRead(SelectedRegister.D);
            EmitByteConstant(0);
            ILGenerator.Emit(OpCodes.Ceq);
            EmitNop();

            // Branch
            ILGenerator.Emit(OpCodes.Brfalse, lb);

            EmitNop();
        }

        private void Emit_DEC(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitByteConstant(1);
            ILGenerator.Emit(OpCodes.Sub);
            EmitLocalStore(0);
            EmitCoreStateObject();
            EmitByteConstant(inst.Low);
            EmitLocalLoad(0);
            EmitGeneralRegisterWriteFunc();
        }

        private void Emit_LDN(CdpInstruction inst)
        {
            // Yeah baby!!!
            EmitLocal(typeof(ushort), false);
            EmitGeneralRegisterRead(inst.Low);
            EmitLocalStore(0);
            EmitReadMemory(GetResolvedLocal(0));
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        private void Emit_BR(CdpInstruction inst)
        {
            // Lets just change the address to the another section of code and keep emitting
            CurrentAddress = (ushort)((CurrentAddress & 0xFF00) | GetMemoryByte((ushort)(CurrentAddress++)));
        }

        private void Emit_LDX(CdpInstruction inst)
        {
            EmitLocal(typeof(ushort), false); // Address [0]
            EmitCoreStateObject();
            EmitRegisterRead(SelectedRegister.X);
            EmitGeneralRegisterReadFunc();
            EmitLocalStore(0);
            EmitReadMemory(GetResolvedLocal(0));
            EmitRegisterWrite(SelectedRegister.D);
            EmitNop();
        }

        #endregion
    }
}
