using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Plugin
{
    public abstract class PropertyBase
    {
        private Dictionary<string, string> m_Props;

        public PropertyBase()
        {
            m_Props = new Dictionary<string, string>();
        }

        protected void SetProperty(string name, string value)
        {
            if (m_Props == null)
                throw new InvalidOperationException();

            if (!m_Props.ContainsKey(name))
            {
                m_Props.Add(name, value);
            }
            else
            {
                m_Props[name] = value;
            }
        }

        protected string GetProperty(string name)
        {
            if (m_Props == null)
                throw new InvalidOperationException();

            if (!m_Props.ContainsKey(name))
            {
                return m_Props[name];
            }
            else
            {
                throw new ArgumentException("name");
            }
        }

    }
}
