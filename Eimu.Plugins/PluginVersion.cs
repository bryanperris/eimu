using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Plugins
{
    public class PluginVersion : Attribute
    {
        private string m_Version;

        public PluginVersion(string version)
        {
            m_Version = version;
        }

        public string Version
        {
            get { return this.m_Version; }
        }
    }
}
