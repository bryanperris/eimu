using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Eimu.Core.Devices;

namespace Eimu
{
    public class FormsInputDevice : InputDevice
    {
        public void SetKeyPress(ChipKeys key, bool pressed)
        {
            if (pressed)
                KeyPress(key);
            else
                KeyRelease(key);
        }
    }
}
