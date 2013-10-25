using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus
{
    [Serializable]
    class SynchronizationContext
    {
        public bool Static { get; set; }

        public List<SynchronizationFrame> Frames { get; set; }

        HashSet<Type> types = new HashSet<Type>();

        Dictionary<Type, object> responses = new Dictionary<Type, object>();        


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
                if (frame.Resolved == true)
                {
                    continue;
                }
                
                if (frame.CanHandle(message.GetType()) == false)
                {
                    unresolvedFrames = true;
                    continue;
                }

                responses.Add(message.GetType(), message);

                if (frame.Satisfies(types))
                {
                    if (Static == false)
                    {
                        frame.CallHandler(responses);
                    }
                    else
                    {
                        frame.CallStaticHandler(responses);
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
