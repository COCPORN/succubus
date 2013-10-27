using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Succubus
{
    [Serializable]
    class SynchronizationContext
    {
        public bool Static { get; set; }


        public List<SynchronizationStack> Stacks;

        internal HashSet<Type> types = new HashSet<Type>();

        internal Dictionary<Type, object> responses = new Dictionary<Type, object>();        

        public object Request { get; set; }

        public SynchronizationContext()
        {
            Stacks = new List<SynchronizationStack>();
        }

        public bool ResolveFor(object message)
        {
            bool unresolvedFrames = false;

            foreach (var synchronizationStack in Stacks)
            {
                if (synchronizationStack.ResolveFor(message) == false)
                {
                    unresolvedFrames = true;
                }
            }

            if (unresolvedFrames) return false;
            return true;
        }

        public bool Finished
        {
            get
            {
                foreach (var synchronizationStack in Stacks)
                {
                    if (synchronizationStack.TimedOut == true) continue;
                    foreach (var frame in synchronizationStack.Frames)
                    {
                        if (frame.Resolved == false)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
      
    }
}
