using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Plugins
{
    public sealed class PluginInfo : Attribute
    {
        private string m_Author;
        private string m_Description;
        private string m_Name;
        private string m_Version;

        public PluginInfo(string name, string version, string authorName, string description)
        {
            this.m_Author = authorName;
            this.m_Description = description;
            this.m_Name = name;
            m_Version = version;
        }

        public string Version
        {
            get { return this.m_Version; }
        }

        public string Name
        {
            get { return this.m_Name; }
        }

        public string Description
        {
            get { return this.m_Description; }
        }

        public string Author
        {
            get { return this.m_Author; }
        }
    }
}
