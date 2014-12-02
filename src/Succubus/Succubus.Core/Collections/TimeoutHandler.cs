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

    /// <summary>
    /// A general purpose timeout handler
    /// </summary>
    /// <typeparam name="TKey">the key type</typeparam>
    /// <typeparam name="TValue">the value type</typeparam>
    public class TimeoutHandler<TKey, TValue> where TValue : IExpiring<TKey>
    {
        private readonly SortedDictionary<Int64, TValue> sortedItems
            = new SortedDictionary<Int64, TValue>();

        private readonly AutoResetEvent timeoutResetEvent = new AutoResetEvent(false);
        private Dictionary<TKey, List<Int64>> timeoutContexts = new Dictionary<TKey, List<Int64>>();
        Thread timeoutThread;

        public TimeoutHandler()
        {
            timeoutThread = new Thread(TimeoutThread) { IsBackground = true };
            timeoutThread.Start();
        }

        // This thread runs through the sorted dictionary
        // to find expired entries. As it is sorted on ticks, it can start
        // in the beginning, and knows when to wait.

        // The amount of time to wait is calculated from the last remaining
        // unexpired entry and current time
        void TimeoutThread()
        {
            // Just an arbitrary value to start with
            int waitmilliseconds = 100;
            while (true)
            {                
                if (waitmilliseconds != 0) timeoutResetEvent.WaitOne(waitmilliseconds);

                // Because you can't change a set of data while iterating over it, copy out values
                var removeKeys = new List<Int64>();


                lock (sortedItems)
                {

                    // Assume we want to wait indefinitely. This is what will happen with an empty
                    // sequence
                    waitmilliseconds = -1;
                    foreach (var entry in sortedItems)
                    {
                        // Check to see whether or not the entry has expired. This should
                        // be changed to using INow, as described over
                        var timespan = TimeSpan.FromTicks(entry.Key - DateTime.Now.Ticks);

                        var comparison = timespan.CompareTo(new TimeSpan(0, 0, 0));

                        // The head of the list isn't yet expired, so beak out of the loop
                        if (comparison == 1)
                        {
                            // How long am I supposed to wait now?
                            waitmilliseconds = timespan.Milliseconds;
                            break;
                        }
                        entry.Value.TimedOut = true;
                        entry.Value.OnTimeout();
                        
                        if (entry.Value.TimeoutHandler != null)
                        {
                            entry.Value.TimeoutHandler();
                        }
 
                        // Add key to set of keys to be removed from the sorted items
                        removeKeys.Add(entry.Key);
                        timeoutContexts.Remove(entry.Value.Id);
                    }

                    foreach (var key in removeKeys)
                    {
                        sortedItems.Remove(key);
                    }
                // End of lock on sorted items
                
                }
            }
        }

        /// <summary>
        /// A debug helper to visualize the current timeout table
        /// </summary>
        public void DumpTimeoutTable()
        {
            lock (sortedItems)
            {
                foreach (var entry in sortedItems)
                {
                    Console.WriteLine("In {0}", TimeSpan.FromTicks((entry.Key - DateTime.Now.Ticks)));
                }
            }
        }

        /// <summary>
        /// The count of how many items are waiting to time out
        /// </summary>
        /// <returns></returns>
        public int NumberOfItemsForTimeout()
        {
            lock (sortedItems)
            {
                return sortedItems.Count;
            }
        }

        /// <summary>
        /// Set an entity to time out
        /// </summary>
        /// <param name="value">the entity</param>
        /// <param name="milliseconds">the timeout</param>
        /// <returns></returns>
        public Int64 Timeout(TValue value, int milliseconds)
        {
            var timespan = TimeSpan.FromMilliseconds(milliseconds);
            var timeoutDateTime = DateTime.Now + timespan;

            Int64 timeoutTick = timeoutDateTime.Ticks;

            lock (sortedItems)
            {
                // Make sure the sorted dictionary doesn't already contain an entry
                // at this tick
                while (sortedItems.ContainsKey((timeoutTick)))
                {
                    timeoutTick++;
                }

                // Add the value to the sorted list the thread is working on expiring
                sortedItems.Add(timeoutTick, value);

                // Because a single item can have multiple timeouts assigned to it,
                // we need to keep track of all of them, as there is an option
                // of removing the timeout
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

            // Because the timeout thread could be blocking indefinitely waiting
            // on -1, adding a new entry to the sorted list forces us to signal 
            // it to wake up. This will allow the timeout thread to recalculate
            // its sleep time
            timeoutResetEvent.Set();
            return timeoutTick;
        }

        /// <summary>
        /// Remove timeout for a given entry, this will stop timeout handlers from getting called
        /// </summary>
        /// <param name="id">the entry</param>
        public void RemoveTimeout(TKey id)
        {
            lock (sortedItems)
            {
                List<Int64> keys;
                if (timeoutContexts.TryGetValue(id, out keys))
                {
                    foreach (var key in keys)
                    {
                        sortedItems.Remove(key);
                    }
                    timeoutContexts.Remove(id);
                }
            }
        }

    }
}