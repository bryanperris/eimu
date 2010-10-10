using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Eimu.Plugins
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
