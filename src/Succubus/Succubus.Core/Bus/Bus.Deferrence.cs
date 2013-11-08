using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Succubus.Collections;
using Succubus.Interfaces.ResponseContexts;

namespace Succubus.Core
{
    public partial class Bus
    {
        private readonly HashSet<Type> deferredResponseTypes = new HashSet<Type>();
        private readonly HashSet<Type> deferredRequestTypes = new HashSet<Type>(); 

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

        void AddToDeferredRequestTypes(params Type[] types)
        {
            lock (deferredRequestTypes)
            {
                foreach (var type in types)
                {
                    if (deferredRequestTypes.Contains(type) == false)
                    {
                        deferredRequestTypes.Add(type);
                    }
                }
            }
        }

        public IResponseContext Defer<TReq, TRes>()
        {
            Action<Guid, TReq, TRes> handler = (guid, req, res) =>
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
            var synchronizationFrame = new SynchronizationFrame<TReq, TRes>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            AddToDeferredRequestTypes(typeof (TReq));
            AddToDeferredResponseTypes(typeof(TRes));

            return new Bus.ResponseContext(this);


        }

        public IResponseContext Defer<TReq, T1, T2>()
        {
            Action<Guid, TReq, T1, T2> handler = (guid, req, res1, res2) =>
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

        public IResponseContext Defer<TReq, T1, T2, T3>()
        {
            Action<Guid, TReq, T1, T2, T3> handler = (guid, req, res1, res2, res3) =>
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

        public IResponseContext Defer<TReq, T1, T2, T3, T4>()
        {
            Action<Guid, TReq, T1, T2, T3, T4> handler = (guid, req, res1, res2, res3, res4) =>
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

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5>()
        {
            Action<Guid, TReq, T1, T2, T3, T4, T5> handler = (guid, req, res1, res2, res3, res4, res5) =>
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

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6>()
        {
            Action<Guid, TReq, T1, T2, T3, T4, T5, T6> handler = (guid, req, res1, res2, res3, res4, res5, res6) =>
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

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6, T7>()
        {
            Action<Guid, TReq, T1, T2, T3, T4, T5, T6, T7> handler =
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

        Dictionary<Guid, ManualResetEvent> deferredWaitHandles = new Dictionary<Guid, ManualResetEvent>(); 

        private SynchronizationContext GetSynchronizationContext<TReq>(Guid correlationId) where TReq: class
        {

            SynchronizationContext ctx = null;
            lock (synchronizationContexts)
            {
                if (synchronizationContexts.TryGetValue(correlationId, out ctx) == false)
                {
                    throw new InvalidOperationException("Unknown context");
                }
            }
            ctx.DeferredResetEvent.WaitOne(6000);
            return ctx;
        }

        private void RemoveContext(Guid correlationId)
        {
            lock (synchronizationContexts)
            {
                synchronizationContexts.Remove(correlationId);
                timeoutHandler.RemoveTimeout(correlationId);
            }
        }

        public IResponseContext Pickup<TReq, T>(Guid correlationId, Action<TReq, T> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            Type t = typeof (TReq);
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T)ctx.responses[typeof(T)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }




        public IResponseContext Pickup<TReq, T1, T2>(Guid correlationId, Action<TReq, T1, T2> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)], (T2)ctx.responses[typeof(T2)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3>(Guid correlationId, Action<TReq, T1, T2, T3> handler) where TReq : class
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext<TReq>(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4>(Guid correlationId, Action<TReq, T1, T2, T3, T4> handler) where TReq : class
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

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5> handler) where TReq : class
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

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler) where TReq : class
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

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler) where TReq : class
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

}