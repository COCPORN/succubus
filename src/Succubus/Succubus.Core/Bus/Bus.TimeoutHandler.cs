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

        Dictionary<Guid, Int64>  timeoutStacks = new Dictionary<Guid, long>();

        private Thread timeoutThread;

        void TimeoutThread()
        {
            int resetEventTimeout = -1;
            while (true)
            {
                timeoutResetEvent.WaitOne(resetEventTimeout);

                lock (sortedTimeoutSynchronizationStacks)
                {
                    if (sortedTimeoutSynchronizationStacks.Count != 0)
                    {
                        bool moreStacks = true;

                        Int64 currentTick = DateTime.Now.Ticks;
                        while (moreStacks)
                        {
                            if (sortedTimeoutSynchronizationStacks.Count == 0)
                            {
                                moreStacks = false;
                                resetEventTimeout = -1; // If we empty the dictionary, wait forever
                                continue;
                            }

                            var stack = sortedTimeoutSynchronizationStacks.FirstOrDefault();

                            if (stack.Key < currentTick)
                            {
                                stack.Value.TimedOut = true;
                                {
                                    if (stack.Value.TimeoutHandler != null) stack.Value.TimeoutHandler();
                                }
                                sortedTimeoutSynchronizationStacks.Remove(stack.Key);
                                timeoutStacks.Remove(stack.Value.CorrelationId);
                            }
                            else
                            {
                                // If we have more frames to be timed out
                                resetEventTimeout = (int) (stack.Key - currentTick)/10;
                                moreStacks = false;
                            }
                        }

                    }
                    else
                    {
                        // We have zero stacks to wait on, wait indefinitely
                        resetEventTimeout = -1;
                    }
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
                Int64 key;
                if (timeoutStacks.TryGetValue(correlationId, out key))
                {
                    sortedTimeoutSynchronizationStacks.Remove(key);
                    timeoutStacks.Remove(correlationId);
                }
            }
        }
    }
}
