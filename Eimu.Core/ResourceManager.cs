using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    [Serializable]
    public abstract class ResourceManager
    {
        public abstract bool LoadResources();
        public abstract void CloseResources();
    }
}
