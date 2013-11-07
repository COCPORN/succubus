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

        public SynchronizationStack(SynchronizationContext ctx)
        {
            Frames = new List<SynchronizationFrame>();
            this.ctx = ctx;
        }

        internal SynchronizationStack CloneFor(SynchronizationContext ctx)
        {
            SynchronizationStack stack = new SynchronizationStack(ctx);
            foreach (var frame in Frames)
            {
                stack.Frames.Add(frame.Clone());
            }
            return stack;
        }
        
        public bool ResolveFor(object message)
        {
            if (Frames == null || Frames.Count == 0) return false;
           
            bool unresolvedFrames = false;
            foreach (var frame in Frames)
            {
                if (frame.Resolved == true)
                {
                    continue;
                }

                if (frame.CanHandle(message) == false)
                {
                    unresolvedFrames = true;
                    continue;
                }
              

                if (ctx.responses.ContainsKey(message.GetType()) == false)
                {
                    ctx.responses.Add(message.GetType(), message);
                }
                Dictionary<Type, object> castMessages = null;
                if (frame.Satisfies(ctx.responses, out castMessages))
                {
                    SynchronizationFrame localFrame = frame;
                    Task.Factory.StartNew(() =>
                    {
                        switch (ctx.ContextType)
                        {
                            case ContextType.Transient:
                                localFrame.CallHandler(castMessages);
                                break;
                            case ContextType.Static:
                                localFrame.CallStaticHandler(castMessages);
                                break;
                            case ContextType.Deferred:
                                localFrame.CallDeferredHandler(castMessages);
                                break;
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

      

        
    }
}
