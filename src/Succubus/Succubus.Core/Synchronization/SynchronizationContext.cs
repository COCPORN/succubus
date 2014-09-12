using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using Succubus.Collections.Interfaces;
using Succubus.Core;
using Succubus.Core.Interfaces;

namespace Succubus
{
    enum ContextType
    {
        Transient,
        Static,
        Deferred
    }


    [Serializable]
    class SynchronizationContext : IExpiring<string>
    {


        public static SynchronizationContext Clone(SynchronizationContext context)
        {
            var newContext = new SynchronizationContext();
            newContext.CorrelationId = context.CorrelationId;
            newContext.Request = context.Request;
            newContext.ContextType = context.ContextType;
            newContext.TimedOut = context.TimedOut;
            newContext.TimeoutMilliseconds = context.TimeoutMilliseconds;

            foreach (var stack in context.Stacks)
            {
                newContext.Stacks.Add(stack.CloneFor(newContext));
            }


            return newContext;

        }

        public ContextType ContextType { get; set; }
        public ManualResetEvent DeferredResetEvent { get; set; }
        public ManualResetEvent ResolvedResetEvent { get; set; }

        public Bus Bus { get; set; }

        public string CorrelationId { get; set; }
        public string Id { get { return CorrelationId; } }

        public List<SynchronizationStack> Stacks;

        internal Dictionary<Type, object> responses = new Dictionary<Type, object>();
        internal Dictionary<Type, object> castMessages = null;

        public object Request { get; set; }

        public SynchronizationContext()
        {
            Stacks = new List<SynchronizationStack>();
            DeferredResetEvent = new ManualResetEvent(false);
            ResolvedResetEvent = new ManualResetEvent(false);
        }

        private bool timedOut = false;

        public bool TimedOut
        {
            get
            {
                return timedOut;
            }
            set
            {
                if (value == true)
                {
                    ResolvedResetEvent.Set();
                }
                timedOut = value;

            }
        }

        public Action TimeoutHandler { get; set; }

        public void SetTimeoutHandler<T>(Action<T> timeoutHandler)
        {
            this.TimeoutHandler = () => timeoutHandler((T)Request);
           
        }

        public int TimeoutMilliseconds { get; set; }

        public bool ResolveFor(object message)
        {
            if (TimedOut == true) return false;
            bool unresolvedFrames = false;

            foreach (var synchronizationStack in Stacks)
            {
                if (synchronizationStack.ResolveFor(message) == false)
                {
                    unresolvedFrames = true;
                }
            }

            if (unresolvedFrames) return false;

            ResolvedResetEvent.Set();
            return true;
        }

        public bool Finished
        {
            get
            {

                foreach (var synchronizationStack in Stacks)
                {
                    //if (synchronizationStack.TimedOut == true) continue;
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



        public void OnTimeout()
        {
            lock (Bus.synchronizationContexts)
            {
                SynchronizationContext existing = null;
                if (Bus.synchronizationContexts.TryGetValue(Id, out existing) == true)
                {
                    existing.ResolvedResetEvent.Set();

                    Bus.synchronizationContexts.Remove(Id);
                }
            }
        }
    }
}
