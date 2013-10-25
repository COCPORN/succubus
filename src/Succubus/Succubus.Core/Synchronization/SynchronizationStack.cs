using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    if (ctx.Static == false)
                    {
                        frame.CallHandler(ctx.responses);
                    }
                    else
                    {
                        frame.CallStaticHandler(ctx.responses);
                    }
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
