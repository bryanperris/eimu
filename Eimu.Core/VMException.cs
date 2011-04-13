using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eimu.Core
{
    public class VMException : Exception
    {
        public VMException() : base() { }
        public VMException(string message) : base(message) { }
        public VMException(string message, Exception innerException) : base (message,innerException){}
    }
}
