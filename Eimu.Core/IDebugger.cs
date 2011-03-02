using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    public interface IDebugger
    {
        void StartDebugging(VirtualMachine currentMachine);
        void Report();
        void StopDebugging();
    }
}
