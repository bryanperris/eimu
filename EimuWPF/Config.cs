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

namespace EimuWPF
{
    public static class Config
    {
        public const string FONT_PATH = "./sys/c8fnt.bin";
        public const string CONFIGPATH = "./config.xml";
        public static string FileROMPath { get; set; }
        public static int SelectedAudioPlugin { get; set; }
        public static int SelectedGraphicsPlugin { get; set; }
        public static int SelectedInputPlugin { get; set; }
        public static bool UseInterpreter { get; set; }

        static Config()
        {
        }

        public static void LoadConfigFile()
        {
            if (File.Exists(CONFIGPATH))
            {
                FileStream file = new FileStream(CONFIGPATH, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlTextReader reader = new XmlTextReader(file);
                
                while (reader.Read())
                {
                    reader.MoveToContent();

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("filerompath"))
                        {
                            FileROMPath = reader.GetAttribute("path");
                        }
                        else if (reader.Name.Equals("useinterpreter"))
                        {
                            UseInterpreter = reader.GetAttribute("enabled") == "True" ? true : false;
                        }
                        else if (reader.Name.Equals("selectedgraphicsplugin"))
                        {
                            SelectedGraphicsPlugin = int.Parse(reader.GetAttribute("index"));
                        }
                        else if (reader.Name.Equals("selectedaudioplugin"))
                        {
                            SelectedAudioPlugin = int.Parse(reader.GetAttribute("index"));
                        }
                        else if (reader.Name.Equals("selectedinputplugin"))
                        {
                            SelectedInputPlugin = int.Parse(reader.GetAttribute("index"));
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                reader.Close();
                file.Close();
            }
        }

        public static void SaveConfigFile()
        {
            FileStream file = new FileStream(CONFIGPATH, FileMode.Create, FileAccess.Write, FileShare.Read);
            XmlTextWriter writer = new XmlTextWriter(file, new UTF8Encoding());
            writer.WriteStartElement("config");

            writer.WriteRaw("\n");
            writer.WriteStartElement("filerompath");
            writer.WriteAttributeString("path", FileROMPath);
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("useinterpreter");
            writer.WriteAttributeString("enabled", UseInterpreter ? "True" : "False");
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("selectedgraphicsplugin");
            writer.WriteAttributeString("index", SelectedGraphicsPlugin.ToString());
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("selectedaudioplugin");
            writer.WriteAttributeString("index", SelectedAudioPlugin.ToString());
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("selectedinputplugin");
            writer.WriteAttributeString("index", SelectedInputPlugin.ToString());
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteFullEndElement();
            writer.Close();
            file.Close();
        }
    }
}
