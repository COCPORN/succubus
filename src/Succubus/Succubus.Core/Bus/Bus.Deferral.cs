using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Succubus.Collections;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
#if false
    public partial class Bus
    {
        private readonly HashSet<Type> deferredResponseTypes = new HashSet<Type>();

        private readonly HashSet<Type> deferredRequestTypes =
            new HashSet<Type>(); 

        void AddToDeferredResponseTypes(params Type[] types)
        {
            lock (deferredResponseTypes)
            {
                foreach (var type in types)
                {
                    if (deferredResponseTypes.Contains(type) == false)
                    {
                        deferredResponseTypes.Add(type);
                    }
                }
            }
        }

        void AddToDeferredRequestTypes(Type type)
        {
            lock (deferredRequestTypes)
            {
              
                    if (deferredRequestTypes.Contains(type) == false)
                    {
                        deferredRequestTypes.Add(type);
                    }
               
            }
        }

        public void Defer<TReq, TRes>()
        {
            Action<string, TReq, TRes> handler = (guid, req, res) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                    else
                    {
                        throw new InvalidOperationException("Unable to get synchronization context");
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, TRes>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(TRes));

            return new Bus.ResponseContext(this);


        }

        public void Defer<TReq, T1, T2>()
        {
            Action<string, TReq, T1, T2> handler = (guid, req, res1, res2) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2));

            return new Bus.ResponseContext(this);
        }

        public void Defer<TReq, T1, T2, T3>()
        {
            Action<string, TReq, T1, T2, T3> handler = (guid, req, res1, res2, res3) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2, T3>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2), typeof(T3));

            return new Bus.ResponseContext(this);
        }

        public void Defer<TReq, T1, T2, T3, T4>()
        {
            Action<string, TReq, T1, T2, T3, T4> handler = (guid, req, res1, res2, res3, res4) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2, T3, T4>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

            return new Bus.ResponseContext(this);
        }

        public void Defer<TReq, T1, T2, T3, T4, T5>()
        {
            Action<string, TReq, T1, T2, T3, T4, T5> handler = (guid, req, res1, res2, res3, res4, res5) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2, T3, T4, T5>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

            return new Bus.ResponseContext(this);
        }

        public void Defer<TReq, T1, T2, T3, T4, T5, T6>()
        {
            Action<string, TReq, T1, T2, T3, T4, T5, T6> handler = (guid, req, res1, res2, res3, res4, res5, res6) =>
            {
                SynchronizationContext ctx = null;
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(guid, out ctx))
                    {
                        ctx.DeferredResetEvent.Set();
                    }
                }
            };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

            return new Bus.ResponseContext(this);
        }

        public void Defer<TReq, T1, T2, T3, T4, T5, T6, T7>()
        {
            Action<string, TReq, T1, T2, T3, T4, T5, T6, T7> handler =
                (guid, req, res1, res2, res3, res4, res5, res6, res7) =>
                {
                    SynchronizationContext ctx = null;
                    lock (synchronizationContexts)
                    {
                        if (synchronizationContexts.TryGetValue(guid, out ctx))
                        {
                            ctx.DeferredResetEvent.Set();
                        }
                    }
                };

            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Deferred);
            var synchronizationFrame = new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6, T7>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof(TReq));
            AddToDeferredResponseTypes(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

            return new Bus.ResponseContext(this);
        }

        Dictionary<string, ManualResetEvent> deferredWaitHandles = new Dictionary<string, ManualResetEvent>();

        private SynchronizationContext GetSynchronizationContext<TReq>(string correlationId) where TReq : class
        {

            SynchronizationContext ctx = null;
            bool setupWaithandle = false;
            lock (synchronizationContexts)
            {
                if (synchronizationContexts.TryGetValue(correlationId, out ctx) == false)
                {
                    setupWaithandle = true;
                }
            }
            if (setupWaithandle)
            {
                lock (deferredWaitHandles)
                {
                    var mre = new ManualResetEvent(false);
                    deferredWaitHandles.Add(correlationId, mre);
                    if (mre.WaitOne(6000) == false)
                    {
                        throw new InvalidOperationException("Unable to find context");
                    }
                    lock (synchronizationContexts)
                    {
                        if (synchronizationContexts.TryGetValue(correlationId, out ctx) == false)
                        {
                            throw new InvalidOperationException("Unable to find context after signal");
                        }
                    }
                }
            }
            if (ctx.DeferredResetEvent.WaitOne(6000) == false)
            {
                throw new InvalidOperationException("Context unsatisfied");
            }
            return ctx;
        }

        private void RemoveContext(string correlationId)
        {
            lock (synchronizationContexts)
            {
                synchronizationContexts.Remove(correlationId);
                timeoutHandler.RemoveTimeout(correlationId);
            }
        }

        public void Pickup<TReq, T>(string correlationId, Action<TReq, T> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T)ctx.castMessages[typeof(T)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }




        public void Pickup<TReq, T1, T2>(string correlationId, Action<TReq, T1, T2> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)], (T2)ctx.responses[typeof(T2)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public void Pickup<TReq, T1, T2, T3>(string correlationId, Action<TReq, T1, T2, T3> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public void Pickup<TReq, T1, T2, T3, T4>(string correlationId, Action<TReq, T1, T2, T3, T4> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public void Pickup<TReq, T1, T2, T3, T4, T5>(string correlationId, Action<TReq, T1, T2, T3, T4, T5> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)],
                (T5)ctx.responses[typeof(T5)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public void Pickup<TReq, T1, T2, T3, T4, T5, T6>(string correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)],
                (T5)ctx.responses[typeof(T5)],
                (T6)ctx.responses[typeof(T6)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public void Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(string correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)],
                (T5)ctx.responses[typeof(T5)],
                (T6)ctx.responses[typeof(T6)],
                (T7)ctx.responses[typeof(T7)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }
    }
#endif
}