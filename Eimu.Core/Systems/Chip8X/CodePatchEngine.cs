using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core.Systems.Chip8X
{
    public sealed class CodePatchEngine
    {
        // TODO: implement load able patch tables into Eimu, for now use hacks for testing

        public bool PatchAddress(CodeEngine engine, ushort address)
        {
            bool a = true;

            switch(address)
            {
                default: a = false; break;
            }

            return a;
        }
    }
}
