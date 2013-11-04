using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Succubus.Core
{
    public partial class Bus
    {
        private readonly SortedDictionary<Int64, SynchronizationContext> sortedTimeoutSynchronizationContexts
            = new SortedDictionary<long, SynchronizationContext>();
        readonly AutoResetEvent timeoutResetEvent = new AutoResetEvent(false);

        Dictionary<Guid, List<Int64>> timeoutContexts = new Dictionary<Guid, List<Int64>>();

        private Thread timeoutThread;

        void TimeoutThread()
        {
            int waitmilliseconds = 100;
            while (true)
            {
                //Console.WriteLine("Waiting {0} ms, {1}/{2} entries", waitmilliseconds, sortedTimeoutSynchronizationStacks.Count, timeoutStacks.Count);
                if (waitmilliseconds != 0) timeoutResetEvent.WaitOne(waitmilliseconds);
                var removeKeys = new List<Int64>();
                lock (sortedTimeoutSynchronizationContexts)
                {
                    waitmilliseconds = -1;
                    foreach (var entry in sortedTimeoutSynchronizationContexts)
                    {                  
                        var timespan = TimeSpan.FromTicks(entry.Key - DateTime.Now.Ticks);
                        var comparison = timespan.CompareTo(new TimeSpan(0, 0, 0));
                        if (comparison == 1)
                        {
                            waitmilliseconds = timespan.Milliseconds;
                            break;
                        }
                        entry.Value.TimedOut = true;
                        {
                            if (entry.Value.TimeoutHandler != null) entry.Value.TimeoutHandler();
                        }
                        removeKeys.Add(entry.Key);
                        timeoutContexts.Remove(entry.Value.CorrelationId);
                    }

                    foreach (var key in removeKeys)
                    {
                        sortedTimeoutSynchronizationContexts.Remove(key);
                    }
                }
            }
        }       

        public void DumpTimeoutTable()
        {
            lock (sortedTimeoutSynchronizationContexts)
            {
                foreach (var entry in sortedTimeoutSynchronizationContexts)
                {
                    Console.WriteLine("In {0}", TimeSpan.FromTicks((entry.Key - DateTime.Now.Ticks)));
                }
            }
        }

        internal Int64 Timeout(SynchronizationContext context, int milliseconds)
        {
            var timespan = TimeSpan.FromMilliseconds(milliseconds);
            var timeoutDateTime = DateTime.Now + timespan;

            Int64 timeoutTick = timeoutDateTime.Ticks;

            lock (sortedTimeoutSynchronizationContexts)
            {

                while (sortedTimeoutSynchronizationContexts.ContainsKey((timeoutTick)))
                {
                    timeoutTick++;
                }
                sortedTimeoutSynchronizationContexts.Add(timeoutTick, context);            
            }
            timeoutResetEvent.Set();
            return timeoutTick;
        }

        internal void RemoveTimeout(Guid correlationId)
        {
            lock (sortedTimeoutSynchronizationContexts)
            {
                List<Int64> keys;
                if (timeoutContexts.TryGetValue(correlationId, out keys))
                {
                    foreach (var key in keys)
                    {
                        sortedTimeoutSynchronizationContexts.Remove(key);    
                    }                    
                    timeoutContexts.Remove(correlationId);
                }
            }
        }
    }
}
