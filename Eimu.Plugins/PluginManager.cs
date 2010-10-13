using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Reflection;
using Eimu.Core.Devices;

namespace Eimu.Plugins
{
    public static class PluginManager
    {
        static List<Type> s_AudioDeviceList;
        static List<Type> s_GraphicsDeviceList;
        static List<Type> s_InputDeviceList;

        static PluginManager()
        {
            s_AudioDeviceList = new List<Type>();
            s_GraphicsDeviceList = new List<Type>();
            s_InputDeviceList = new List<Type>();
            ClearPluginLists();
        }

        public static void ClearPluginLists()
        {
            s_AudioDeviceList.Clear();
            s_GraphicsDeviceList.Clear();
            s_InputDeviceList.Clear();
            AudioPluginIndex = -1;
            InputPluginIndex = -1;
            GraphicsPluginIndex = -1;
        }

        public static void LoadPluginsFromFile(string folderpath)
        {
            // DLL plugins
            DirectoryInfo dir = new DirectoryInfo(folderpath);
            FileInfo[] dlls = dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            foreach (FileInfo dll in dlls)
            {
                try
                {
                    Assembly ass = Assembly.LoadFile(dll.FullName);
                    object[] attrs = ass.GetCustomAttributes(typeof(EimuPluginAssembly), false);

                    if (attrs.Length > 0)
                        LoadPluginsFromAssembly(ass);
                }
                catch (BadImageFormatException)
                {
                    continue;
                }
            }
        }

        public static void LoadPluginsFromAssembly(Assembly assembly)
        {
            List<Type> plugins = new List<Type>();
            Type[] types = assembly.GetTypes();

            // Make a list a IPlugins
            foreach (Type type in types)
            {
                Type[] interfaces = type.GetInterfaces();

                foreach (Type face in interfaces)
                {
                    if (face == typeof(IPlugin))
                    {
                       plugins.Add(type);
                       break;
                    }
                }
            }

            foreach (Type type in plugins)
            {
                Type a = type.BaseType;

                if (typeof(AudioDevice) == a)
                    s_AudioDeviceList.Add(type);

                else if (typeof(GraphicsDevice) == a)
                    s_GraphicsDeviceList.Add(type);

                else if (typeof(InputDevice) == a)
                    s_InputDeviceList.Add(type);

                else
                    continue;
            }
        }

        public static void LoadPluginsFromCallingAssembly()
        {
            LoadPluginsFromAssembly(Assembly.GetCallingAssembly());
        }

        public static PluginInfo GetPluginInfo(Type type)
        {
            if (type.IsDefined(typeof(PluginInfo), false))
            {
                return (PluginInfo)type.GetCustomAttributes(typeof(PluginInfo), false)[0];
            }
            else
            {
                return null;
            }
        }

        public static List<Type> AudioDeviceList
        {
            get { return s_AudioDeviceList; }
        }

        public static List<Type> GraphicsDeviceList
        {
            get { return s_GraphicsDeviceList; }
        }

        public static List<Type> InputDeviceDeviceList
        {
            get { return s_InputDeviceList; }
        }

        public static IntPtr WindowHandle { get; set; }

        public static IntPtr RenderContext { get; set; }

        public static void SetSelectedPlugins(int audioIndex, int graphicsIndex, int inputIndex)
        {
            if (audioIndex > s_AudioDeviceList.Count || audioIndex < 0)
                throw new ArgumentOutOfRangeException("audioIndex");

            AudioPluginIndex = audioIndex;

            if (graphicsIndex > s_GraphicsDeviceList.Count || graphicsIndex < 0)
                throw new ArgumentOutOfRangeException("graphicsIndex");

            GraphicsPluginIndex = graphicsIndex;

            if (inputIndex > s_InputDeviceList.Count || inputIndex < 0)
                throw new ArgumentOutOfRangeException("inputIndex");

            InputPluginIndex = inputIndex;
        }

        public static Type SelectedAudioDevice
        {
            get { return s_AudioDeviceList[AudioPluginIndex]; }
        }

        public static Type SelectedGraphicsDevice
        {
            get { return s_GraphicsDeviceList[GraphicsPluginIndex]; }
        }

        public static Type SelectedInputDevice
        {
            get { return s_InputDeviceList[InputPluginIndex]; }
        }

        public static int AudioPluginIndex { get; set; }

        public static int InputPluginIndex { get; set; }

        public static int GraphicsPluginIndex { get; set; }

        public static bool EnableDoubleBuffer { get; set; }
    }
}
