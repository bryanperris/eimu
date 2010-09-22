using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Eimu.Core.Devices;
using System.Reflection;

namespace Eimu.Plugins
{
    public sealed class PluginManager
    {
        List<Type> m_AudioDeviceList;
        List<Type> m_GraphicsDeviceList;
        List<Type> m_InputDeviceList;

        public PluginManager()
        {
            m_AudioDeviceList = new List<Type>();
            m_GraphicsDeviceList = new List<Type>();
            m_InputDeviceList = new List<Type>();
        }

        public void LoadPluginsFromAssembly(Assembly assembly)
        {
            List<object> plugins = new List<object>();
            Type[] types = assembly.GetTypes();

            // Make a list a IPlugins
            foreach (Type type in types)
            {
                if (type.GetInterface(typeof(IPlugin).ToString()) == typeof(IPlugin))
                {
                    plugins.Add(type);
                    break;
                }
            }

            foreach (object plugin in plugins)
            {
                Type type = plugin.GetType().BaseType;

                if (type.IsAssignableFrom(typeof(AudioDevice).GetType()))
                    this.m_AudioDeviceList.Add((Type)plugin);
                else if (type.IsAssignableFrom(typeof(GraphicsDevice).GetType()))
                    this.m_GraphicsDeviceList.Add((Type)plugin);
                else if (type.IsAssignableFrom(typeof(InputDevice).GetType()))
                    this.m_InputDeviceList.Add((Type)plugin);
                else
                    continue;
            }
        }

        public List<Type> AudioDeviceList
        {
            get { return this.m_AudioDeviceList; }
        }

        public List<Type> GraphicsDeviceList
        {
            get { return this.m_GraphicsDeviceList; }
        }

        public List<Type> InputDeviceDeviceList
        {
            get { return this.m_InputDeviceList; }
        }
    }
}
