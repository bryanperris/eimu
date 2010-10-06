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
        public void SetKeyPress(ChipKeys key)
        {
            KeyPress(key);
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

        public override void Shutdown()
        {
            throw new NotImplementedException();
        }

        public override void SetPauseState(bool paused)
        {
            throw new NotImplementedException();
        }
    }
}
