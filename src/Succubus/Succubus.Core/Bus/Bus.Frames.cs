using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    internal class FrameBlock
    {
        internal Action<IMessageFrame> Handler;
        internal Action<Action> Marshal;
    }

    public partial class Bus
    {
        private readonly List<FrameBlock> frameHandlers = new List<FrameBlock>();    


        public void FrameMessage(IMessageFrame o)
        {
            foreach (var eventHandler in frameHandlers)
            {
                var handler = eventHandler;

                try
                {
                    if (handler.Marshal == null)
                    {
                        Task.Factory.StartNew(() => handler.Handler(o));
                    }
                    else
                    {
                        handler.Marshal(() => handler.Handler(o));
                    }
                }
                catch (Exception ex)
                {
                    RaiseExceptionEvent(ex);
                }

            }
        }
    }
}