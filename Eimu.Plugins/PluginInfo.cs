/*  
Eimu - Chip-8 Emulator
Copyright (C) 2010  http://code.google.com/p/eimu

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_Name);
            sb.Append(" ");
            sb.Append(m_Version);

            return sb.ToString();
        }
    }
}
