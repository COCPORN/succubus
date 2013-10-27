using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Succubus
{
    [Serializable]
    class SynchronizationStack
    {
        public List<SynchronizationFrame> Frames { get; set; }

        SynchronizationContext ctx;

        internal SynchronizationContext Context { get { return ctx; } }

        public SynchronizationStack(SynchronizationContext ctx)
        {
            Frames = new List<SynchronizationFrame>();
            this.ctx = ctx;
        }        

        internal Action TimeoutHandler;

        public void SetTimeoutHandler<T>(Action<T> timeoutHandler)
        {
            this.TimeoutHandler = () => timeoutHandler((T)ctx.Request);
        }

        public bool ResolveFor(object message)
        {
            if (TimedOut || Frames == null || Frames.Count == 0) return false;
           
            bool unresolvedFrames = false;
            foreach (var frame in Frames)
            {
                if (frame.Resolved == true)
                {
                    continue;
                }

                if (frame.CanHandle(message.GetType()) == false)
                {
                    unresolvedFrames = true;
                    continue;
                }
                else
                {
                    ctx.types.Add(message.GetType());
                }

                if (ctx.responses.ContainsKey(message.GetType()) == false)
                {
                    ctx.responses.Add(message.GetType(), message);
                }

                if (frame.Satisfies(ctx.types))
                {
                    SynchronizationFrame localFrame = frame;
                    Task.Factory.StartNew(() =>
                    {
                        if (ctx.Static == false)
                        {
                            localFrame.CallHandler(ctx.responses);
                        }
                        else
                        {
                            localFrame.CallStaticHandler(ctx.responses);
                        }
                    });
                    frame.Resolved = true;
                }
                else
                {
                    unresolvedFrames = true;
                }   
            }

            if (unresolvedFrames == false)
            {
                // Finished resolving this context
                return true;
            }
            else return false;

        }

        public bool TimedOut { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
