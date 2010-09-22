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
                    this.m_AudioDeviceList.Add(type);

                else if (typeof(GraphicsDevice) == a)
                    this.m_GraphicsDeviceList.Add(type);

                else if (typeof(InputDevice) == a)
                    this.m_InputDeviceList.Add(type);

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
