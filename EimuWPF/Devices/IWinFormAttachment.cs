using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Devices
{
    public interface IWinFormAttachment
    {
        void SetPanelHandle(IntPtr handle);
        void SetWindowHandle(IntPtr handle);
        bool UseDoubleBugger
        {
            get;
            set;
        }
    }
}
