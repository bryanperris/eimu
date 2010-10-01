using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eimu.Core;

namespace Eimu.Plugins
{
    public interface IPlugin
    {
        void ShowConfigDialog();
        string[] GetOptionsList();
        void SetOption(string name, string value);
        string GetOption(string name);
    }
}
