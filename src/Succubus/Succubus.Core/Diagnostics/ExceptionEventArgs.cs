using System;

namespace Succubus.Core.Diagnostics
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; } 

    }
}