using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class ChipResources : ResourceManager
    {
        private Stream m_RomSource;
        private Stream m_FontSource;
        private Stream m_SuperFontSource;
        private Chip8XMachine m_Machine;

        public ChipResources(Chip8XMachine machine)
        {
            m_Machine = machine;
        }

        public override bool LoadResources()
        {
            try
            {
                int read;
                int pos = 0;
                m_FontSource.Position = 0;
                m_SuperFontSource.Position = 0;
                Memory mem =  m_Machine.SystemMemory;

                while ((read = m_FontSource.ReadByte()) != -1)
                {
                   mem[pos++] = (byte)read;
                }

                while ((read = m_SuperFontSource.ReadByte()) != -1)
                {
                    mem[pos++] = (byte)read;
                }

                if (this.m_RomSource == null)
                    return false;

                if (!this.m_RomSource.CanRead)
                {
                    Console.WriteLine("Source can't be read!");
                    return false;
                }

                this.m_RomSource.Position = 0;
                read = 0;
                pos = Chip8XMachine.PROGRAM_ENTRY_POINT;

                while ((read = m_RomSource.ReadByte()) != -1)
                {
                    mem[pos++] = (byte)read;
                }

                return true;

            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public override void CloseResources()
        {
            m_RomSource.Close();
            m_FontSource.Close();
            m_SuperFontSource.Close();
        }

        public Stream ProgramSource
        {
            get { return m_RomSource; }
            set { m_RomSource = value; }
        }

        public Stream FontSource
        {
            get { return m_FontSource; }
            set { m_FontSource = value; }
        }

        public Stream SuperFontSource
        {
            get { return m_SuperFontSource; }
            set { m_SuperFontSource = value; }
        }
    }
}
