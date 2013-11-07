using System;
using System.Collections.Generic;
using System.Threading;
using Succubus.Collections.Interfaces;

namespace Succubus.Core
{
    public class TimeoutHandler<Key, Value> where Value : IExpiring<Key>
    {
        private readonly SortedDictionary<Int64, Value> sortedTimeoutSynchronizationContexts
            = new SortedDictionary<Int64, Value>();

        private readonly AutoResetEvent timeoutResetEvent = new AutoResetEvent(false);
        public Dictionary<Key, List<Int64>> timeoutContexts = new Dictionary<Key, List<Int64>>();
        public Thread timeoutThread;

        public void TimeoutThread()
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
                        timeoutContexts.Remove(entry.Value.Id);
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

        internal Int64 Timeout(Value context, int milliseconds)
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

        public void RemoveTimeout(Key correlationId)
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