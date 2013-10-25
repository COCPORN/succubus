using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus
{
    class SynchronizationContext
    {
        public List<SynchronizationFrame> Frames { get; set; }

        HashSet<Type> types = new HashSet<Type>();

        public SynchronizationContext()
        {
            Frames = new List<SynchronizationFrame>();
        }

        public bool ResolveFor(object message)
        {
            if (Frames == null || Frames.Count == 0) return false;

            types.Add(message.GetType());

            bool unresolvedFrames = false;
            foreach (var frame in Frames)
            {
                if (frame.Resolved == true) continue;
                unresolvedFrames = true;
                if (frame.Satisfies(types))
                {
                    frame.CallHandler(message);
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
