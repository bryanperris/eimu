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
using Eimu.Core;

namespace Eimu
{
    public static class Config
    {
        public const string CHIP8_FONT_PATH = "./sys/c8fnt.bin";
        public const string CONFIGPATH = "./config.xml";
        public static string C8FileROMPath { get; set; }
        public static bool UseC8Interpreter { get; set; }
        public static RgbColor C8BackColor { get; set; }
        public static RgbColor C8ForeColor { get; set; }

        static Config()
        {
        }

        public static void LoadConfigFile()
        {
            if (File.Exists(CONFIGPATH))
            {
                FileStream file = new FileStream(CONFIGPATH, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlTextReader reader = new XmlTextReader(file);


                C8BackColor = new RgbColor(0, 0, 64);
                C8ForeColor = new RgbColor(202, 232, 255);

                while (reader.Read())
                {
                    reader.MoveToContent();

                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name.Equals("filerompath"))
                        {
                            C8FileROMPath = reader.GetAttribute("path");
                        }
                        else if (reader.Name.Equals("useinterpreter"))
                        {
                            UseC8Interpreter = reader.GetAttribute("enabled") == "True" ? true : false;
                        }
                        else if (reader.Name.Equals("chip8backcolor"))
                        {
                            C8BackColor = new RgbColor(
                                byte.Parse(reader.GetAttribute("r")),
                                byte.Parse(reader.GetAttribute("g")),
                                byte.Parse(reader.GetAttribute("b")));
                        }
                        else if (reader.Name.Equals("chip8forecolor"))
                        {
                            C8ForeColor = new RgbColor(
                                byte.Parse(reader.GetAttribute("r")),
                                byte.Parse(reader.GetAttribute("g")),
                                byte.Parse(reader.GetAttribute("b")));
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
            writer.WriteAttributeString("path", C8FileROMPath);
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("useinterpreter");
            writer.WriteAttributeString("enabled", UseC8Interpreter ? "True" : "False");
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("chip8backcolor");
            writer.WriteAttributeString("r", C8BackColor.Red.ToString());
            writer.WriteAttributeString("g", C8BackColor.Green.ToString());
            writer.WriteAttributeString("b", C8BackColor.Blue.ToString());
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteStartElement("chip8forecolor");
            writer.WriteAttributeString("r", C8ForeColor.Red.ToString());
            writer.WriteAttributeString("g", C8ForeColor.Green.ToString());
            writer.WriteAttributeString("b", C8ForeColor.Blue.ToString());
            writer.WriteEndElement();
            writer.WriteRaw("\n");

            writer.WriteFullEndElement();
            writer.Close();
            file.Close();
        }
    }
}
