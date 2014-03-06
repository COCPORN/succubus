using System;
using Succubus.Core.Diagnostics;

namespace Succubus.Core
{
    public partial class Bus
    {
        public event EventHandler<ExceptionEventArgs> HandlerException;

        void RaiseExceptionEvent(Exception ex)
        {
            EventHandler<ExceptionEventArgs> eh = HandlerException;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs {Exception = ex});
            }
        }
    }
}