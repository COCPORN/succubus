using System;
using System.Collections.Generic;
using System.Threading;
using Succubus.Collections.Interfaces;

namespace Succubus.Collections
{
    class DefaultNow : INow
    {
        public DateTime Now { get { return DateTime.Now; } }
    }

    public class TimeoutHandler<TKey, TValue> where TValue : IExpiring<TKey>
    {
        private readonly SortedDictionary<Int64, TValue> sortedTimeoutSynchronizationContexts
            = new SortedDictionary<Int64, TValue>();

        private readonly AutoResetEvent timeoutResetEvent = new AutoResetEvent(false);
        private Dictionary<TKey, List<Int64>> timeoutContexts = new Dictionary<TKey, List<Int64>>();
        Thread timeoutThread;

        public TimeoutHandler()
        {
            timeoutThread = new Thread(TimeoutThread) { IsBackground = true };
            timeoutThread.Start();
        }

        void TimeoutThread()
        {
            int waitmilliseconds = 100;
            while (true)
            {
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

                        if (entry.Value.TimeoutHandler != null)
                        {
                            entry.Value.TimeoutHandler();
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

        public int NumberOfItemsForTimeout()
        {
            lock (sortedTimeoutSynchronizationContexts)
            {
                return sortedTimeoutSynchronizationContexts.Count;
            }
        }

        public Int64 Timeout(TValue value, int milliseconds)
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
                sortedTimeoutSynchronizationContexts.Add(timeoutTick, value);
                List<Int64> keys;
                if (timeoutContexts.TryGetValue(value.Id, out keys) == false)
                {
                    keys = new List<long>();
                    keys.Add(timeoutTick);
                    timeoutContexts.Add(value.Id, keys);
                }
                else
                {
                    keys.Add(timeoutTick);
                }
            }
            timeoutResetEvent.Set();
            return timeoutTick;
        }

        public void RemoveTimeout(TKey correlationId)
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

        public IEnumerable<TValue> GetValues(TKey correlationId)
        {
            List<TValue> values = new List<TValue>();
            lock (sortedTimeoutSynchronizationContexts)
            {
               List<Int64> keys;
                if (timeoutContexts.TryGetValue(correlationId, out keys))
                {
                    foreach (var key in keys)
                    {
                        values.Add(sortedTimeoutSynchronizationContexts[key]);
                    }
                }
              
            }
            foreach (var value in values)
            {
                yield return value;
            }
        }

        public IEnumerable<TValue> GetAndRemoveValues(TKey correlationId)
        {
            List<TValue> values = new List<TValue>();
            lock (sortedTimeoutSynchronizationContexts)
            {
                List<Int64> keys;
                if (timeoutContexts.TryGetValue(correlationId, out keys))
                {
                    foreach (var key in keys)
                    {
                        values.Add(sortedTimeoutSynchronizationContexts[key]);
                    }
                }

            }
            RemoveTimeout(correlationId);
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }
}