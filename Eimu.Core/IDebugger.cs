using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    public interface IDebugger
    {
        void StartDebugging(object currentMachine);
        void Report(object state);
        void StopDebugging();
    }
}
