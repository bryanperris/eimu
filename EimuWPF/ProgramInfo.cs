using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu
{
    public class ProgramInfo
    {
        private string m_Name;
        private string m_Description;
        private string m_Hash;
        private long m_Size;

        public ProgramInfo(string name, string hash, long size)
        {
            m_Description = "";
            m_Hash = hash;
            m_Name = name;
            m_Size = size;
        }

        public string Name
        {
            get { return this.m_Name; }
        }

        public string Description
        {
            get { return this.m_Description; }
            set { this.m_Description = value; }
        }

        public string HashCode
        {
            get { return this.m_Hash; }
        }

        public long Size
        {
            get { return this.m_Size; }
        }
    }
}
