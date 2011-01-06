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
using System.Text;
using System.IO;
using System.Xml;

namespace Eimu.Core.Plugin
{
    public class Configuration
    {
        private string[] m_Options;
        private string m_ConfigName;

        public Configuration(string[] options, string configName)
        {
            this.m_Options = options;
            m_ConfigName = configName;
        }

        public void SaveCofig(string[] values)
        {
            if (values.Length != m_Options.Length)
                throw new ArgumentException("the length of values does not match length of options");

            FileStream file = new FileStream("./" + m_ConfigName + ".xml", FileMode.Create, FileAccess.Write, FileShare.Read);
            XmlTextWriter writer = new XmlTextWriter(file, new ASCIIEncoding());

            writer.WriteStartElement("config");
            writer.WriteRaw("\n");

            for (int i = 0; i < m_Options.Length; i++)
            {
                writer.WriteStartElement("option");
                writer.WriteAttributeString("name", m_Options[i]);
                writer.WriteAttributeString("value", values[i]);
                writer.WriteEndElement();
                writer.WriteRaw("\n");
            }

            writer.WriteFullEndElement();

            writer.Close();
            file.Close();
        }

        //public Dictionary<string, string> LoadConfig()
        //{
        //    //Dictionary<string, string> loolup = new Dictionary<string, string>();

        //    //FileStream file = new FileStream("./" + m_ConfigName + ".xml", FileMode.Open, FileAccess.Read, FileShare.Read);
        //    //XmlTextReader reader = new XmlTextReader(file);

        //    //while (reader.Read())
        //    //{
        //    //    if (reader.Name == "option")
        //    //    {
        //    //        // TODO:
        //    //    }
        //    //}
        //}
    }
}
