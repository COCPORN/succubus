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

        public SynchronizationContext()
        {
            Frames = new List<SynchronizationFrame>();
        }

        public void ResolveFor(object message)
        {
            if (Frames == null || Frames.Count == 0) return;
            Type genericType = Frames.First().GetType().GetGenericArguments()[0];
            
            if (genericType == message.GetType())
            {
                Frames.First().CallHandler(message);
            }
        }
    }
}
