using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Devices
{
    public interface IDevice
    {
        void Initialize();
        void Shutdown();
        void SetState(RunState state);
    }
}
