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
        private Dictionary<Guid, ManualResetEvent> deferredResetEvents = new Dictionary<Guid, ManualResetEvent>();


        public IResponseContext Defer<TReq, T>()
        {
            Action<Guid, TReq, T> handler = (guid, req, res) =>
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
            var synchronizationFrame = new SynchronizationFrame<TReq, T>
            {
                CorrelationHandler = handler
            };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

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

            return new Bus.ResponseContext(this);
        }

        private SynchronizationContext GetSynchronizationContext(Guid correlationId)
        {
            
            SynchronizationContext ctx = null;
            lock (synchronizationContexts)
            {
                if (synchronizationContexts.TryGetValue(correlationId, out ctx) == false)
                {
                    throw new InvalidOperationException("Unknown correlation id");
                }
            }
            ctx.DeferredResetEvent.WaitOne();
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

        public IResponseContext Pickup<TReq, T>(Guid correlationId, Action<TReq, T> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T)ctx.responses[typeof (T)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }




        public IResponseContext Pickup<TReq, T1, T2>(Guid correlationId, Action<TReq, T1, T2> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)], (T2)ctx.responses[typeof(T2)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3>(Guid correlationId, Action<TReq, T1, T2, T3> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)], 
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4>(Guid correlationId, Action<TReq, T1, T2, T3, T4> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)],
                (T5)ctx.responses[typeof(T5)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
            handler((TReq)ctx.Request, (T1)ctx.responses[typeof(T1)],
                (T2)ctx.responses[typeof(T2)],
                (T3)ctx.responses[typeof(T3)],
                (T4)ctx.responses[typeof(T4)],
                (T5)ctx.responses[typeof(T5)],
                (T6)ctx.responses[typeof(T6)]);
            RemoveContext(correlationId);
            return new ResponseContext(this);
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            if (handler == null) throw new ArgumentException("Pickup needs a handler");
            var ctx = GetSynchronizationContext(correlationId);
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