using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Plugins
{
    public class PluginName : Attribute
    {
        private string m_Name;

        public PluginName(string name)
        {
            this.m_Name = name;
        }

        public string Name
        {
            get { return this.m_Name; }
        }
    }
}
