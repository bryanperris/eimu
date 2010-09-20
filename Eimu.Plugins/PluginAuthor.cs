using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Plugins
{
    public class PluginAuthor : Attribute
    {
        private string m_Author;

        public PluginAuthor(string authorName)
        {
            this.m_Author = authorName;
        }

        public string Author
        {
            get { return this.m_Author; }
        }
    }
}
