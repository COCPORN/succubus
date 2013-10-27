using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Succubus.Core
{
    public partial class Bus
    {
        private readonly SortedDictionary<Int64, SynchronizationStack> sortedTimeoutSynchronizationStacks
            = new SortedDictionary<long, SynchronizationStack>();
        readonly AutoResetEvent timeoutResetEvent = new AutoResetEvent(false);

        Dictionary<Guid, List<Int64>> timeoutStacks = new Dictionary<Guid, List<Int64>>();

        private Thread timeoutThread;

        void TimeoutThread()
        {
            int waitmilliseconds = 100;
            while (true)
            {
                //Console.WriteLine("Waiting {0} ms, {1}/{2} entries", waitmilliseconds, sortedTimeoutSynchronizationStacks.Count, timeoutStacks.Count);
                if (waitmilliseconds != 0) timeoutResetEvent.WaitOne(waitmilliseconds);
                var removeKeys = new List<Int64>();
                lock (sortedTimeoutSynchronizationStacks)
                {
                    waitmilliseconds = -1;
                    foreach (var entry in sortedTimeoutSynchronizationStacks)
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
                        timeoutStacks.Remove(entry.Value.CorrelationId);
                    }

                    foreach (var key in removeKeys)
                    {
                        sortedTimeoutSynchronizationStacks.Remove(key);
                    }
                }
            }
        }       

        public void DumpTimeoutTable()
        {
            lock (sortedTimeoutSynchronizationStacks)
            {
                foreach (var entry in sortedTimeoutSynchronizationStacks)
                {
                    Console.WriteLine("In {0}", TimeSpan.FromTicks((entry.Key - DateTime.Now.Ticks)));
                }
            }
        }

        internal Int64 Timeout(SynchronizationStack stack, int milliseconds)
        {
            var timespan = TimeSpan.FromMilliseconds(milliseconds);
            var timeoutDateTime = DateTime.Now + timespan;

            Int64 timeoutTick = timeoutDateTime.Ticks;

            lock (sortedTimeoutSynchronizationStacks)
            {

                while (sortedTimeoutSynchronizationStacks.ContainsKey((timeoutTick)))
                {
                    timeoutTick++;
                }
                sortedTimeoutSynchronizationStacks.Add(timeoutTick, stack);            
            }
            timeoutResetEvent.Set();
            return timeoutTick;
        }

        internal void RemoveTimeout(Guid correlationId)
        {
            lock (sortedTimeoutSynchronizationStacks)
            {
                List<Int64> keys;
                if (timeoutStacks.TryGetValue(correlationId, out keys))
                {
                    foreach (var key in keys)
                    {
                        sortedTimeoutSynchronizationStacks.Remove(key);    
                    }                    
                    timeoutStacks.Remove(correlationId);
                }
            }
        }
    }
}
