using System;
using Succubus.Core.Diagnostics;

namespace Succubus.Core
{
    public partial class Bus
    {
        public event EventHandler<ExceptionEventArgs> Exception;

        public event EventHandler<ExceptionEventArgs> HandlerException;

        void RaiseExceptionEvent(Exception ex)
        {
            EventHandler<ExceptionEventArgs> eh = HandlerException;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs {Exception = ex});
            }
            eh = Exception;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs { Exception = ex });
            }
        }

        public event EventHandler<ExceptionEventArgs> MessageCreationException;

        public void UnableToCreateMessage(Exception ex)
        {
            EventHandler<ExceptionEventArgs> eh = MessageCreationException;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs { Exception = ex });
            }
            eh = Exception;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs { Exception = ex });
            }
        }
     
        public event EventHandler<ExceptionEventArgs> TransportException;

        public void GeneralTransportException(Exception ex)
        {
            EventHandler<ExceptionEventArgs> eh = TransportException;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs { Exception = ex });
            }
            eh = Exception;
            if (eh != null)
            {
                eh(this, new ExceptionEventArgs { Exception = ex });
            }
        }

    }
}