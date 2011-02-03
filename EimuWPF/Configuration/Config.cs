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
using System.IO;
using System.Xml;
using System.Security.Cryptography;
using System.Reflection;
using Eimu.Core;

namespace Eimu.Configuration
{
    public static class Config
    {
        public const string systemDirectory = "./sys/";
        public const string ConfigPath = "./config.xml";
        public static string romFilePath = "";

        // TODO: Implement serialization for non primtive types

        private static void LoadObjectData(Type type, XmlTextReader reader)
        {
            try
            {
                if (!reader.ReadToFollowing(type.Name))
                    return;
            }
            catch (System.Xml.XmlException)
            {
                return;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            foreach (FieldInfo info in fields)
            {
                try
                {
                    if (!reader.ReadToFollowing(info.Name))
                        continue;

                    Type t = Type.GetType(reader.GetAttribute("type"));
                    object data = reader.ReadElementContentAs(t, null);
                    info.SetValue(type, data);
                }
                catch (FieldAccessException)
                {
                    continue;
                }
            }
        }

        private static void SaveObjectData(Type type, XmlTextWriter writer)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

            writer.WriteRaw("\r\n\t");
            writer.WriteStartElement(type.Name);
            

            foreach (FieldInfo info in fields)
            {
                writer.WriteRaw("\r\n\t\t");
                writer.WriteStartElement(info.Name);
                writer.WriteAttributeString("type", info.FieldType.FullName);

                object data = info.GetValue(type);

                if (data != null)
                {
                    writer.WriteValue(data);
                }
                else
                {
                    writer.WriteValue("null");
                }
                
                writer.WriteFullEndElement();
            }

            writer.WriteRaw("\r\n\t");
            writer.WriteFullEndElement();
        }

        public static void SaveConfig()
        {
            FileStream file = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            XmlTextWriter writer = new XmlTextWriter(file, new UTF8Encoding());
            writer.WriteStartElement("EimuConfig");
            SaveObjectData(typeof(Config), writer);
            SaveObjectData(typeof(SchipConfig), writer);
            writer.WriteRaw("\r\n");
            writer.WriteFullEndElement();
            writer.Close();
            file.Close();
        }

        public static void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
                SaveConfig();

            FileStream file = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlTextReader reader = new XmlTextReader(file);

            try
            {
                // Test Read for root node
                reader.Read();
            }
            catch(XmlException)
            {
                reader.Close();
                file.Close();
                SaveConfig();
                // Retry
                LoadConfig();
            }

            LoadObjectData(typeof(Config), reader);
            LoadObjectData(typeof(SchipConfig), reader);
            reader.Close();
            file.Close();
        }
    }
}
