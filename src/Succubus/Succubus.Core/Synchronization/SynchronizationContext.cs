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


        public List<SynchronizationStack> Stacks;

        internal HashSet<Type> types = new HashSet<Type>();

        internal Dictionary<Type, object> responses = new Dictionary<Type, object>();        


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
            else return true;
        }
      
    }
}
