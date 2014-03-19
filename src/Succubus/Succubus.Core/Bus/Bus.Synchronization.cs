using System.Data.Odbc;
using System.Threading;
using System.Threading.Tasks;
using Succubus.Collections;
using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Succubus.Core
{
    public partial class Bus
    {

        #region Synchronization stores and event handlers

        /// <summary>
        /// These are used to store synchronization contexts. Whenever
        /// a synchronization context has been fulfilled, it should be removed
        /// from this dictionary.
        /// </summary>
        Dictionary<string,
            SynchronizationContext> synchronizationContexts =
            new Dictionary<string, SynchronizationContext>();

        /// <summary>
        /// This is used to populate synchronizationcontexts with static
        /// prototypes.
        /// </summary>
        Dictionary<Type,
            SynchronizationContext> staticSynchronizationPrototypes = new Dictionary<Type, SynchronizationContext>();



        /// <summary>
        /// This is used to create "server" logic to respond to synchronous messages.
        /// </summary>
        Dictionary<Type, List<SynchronousBlock>> replyHandlers = new Dictionary<Type, List<SynchronousBlock>>();

        private readonly TimeoutHandler<string, SynchronizationContext> timeoutHandler = new TimeoutHandler<string, SynchronizationContext>();

        #endregion


        #region Synchronous messaging

        MessageFrames.Synchronous FrameSynchronously(object o)
        {
            return new MessageFrames.Synchronous
            {
                Message = o,
                CorrelationId = CorrelationIdProvider.CreateCorrelationId(o),
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],
                RequestType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],

            };
        }

        MessageFrames.Event FrameEvent(object o)
        {
            return new MessageFrames.Event
            {
                Message = o,
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],

            };
        }

        MessageFrames.Synchronous FrameResponseSynchronously(MessageFrames.Synchronous request, object o, string guid)
        {
            return new MessageFrames.Synchronous
            {
                Message = o,
                CorrelationId = guid ?? CorrelationIdProvider.CreateCorrelationId(o),
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],
                RequestType = request.RequestType,
                Request = o
            };
        }


        public void Call<TReq, TRes>(TReq request, Action<TRes> handler, string address = null,
            Action<Action> marshal = null)
        {
            InternalCall(request, handler, address, marshal);
        }

        string InternalCall<TReq, TRes>(TReq request, Action<TRes> handler, string address = null, Action<Action> marshal = null)
        {
            SetupReplySubscription();
            var synchronizationContext = new SynchronizationContext();


            SynchronizationStack stack = new SynchronizationStack(synchronizationContext);
            stack.Frames.Add(new SynchronizationFrame<TReq, TRes> { Handler = handler });
            synchronizationContext.Stacks.Add(stack);

            var synchronizedRequest = FrameSynchronously(request);
            SynchronizationContext existing = null;
            lock (synchronizationContexts)
            {
                if (synchronizationContexts.TryGetValue(synchronizedRequest.CorrelationId, out existing) == false)
                {
                    synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
                }
            }

            // Wait for existing synchronization contexts to finish in case of crashing correlation ids
            while (existing != null)
            {           
                existing.ResolvedResetEvent.WaitOne(10000);
                lock (synchronizationContexts)
                {
                    if (synchronizationContexts.TryGetValue(synchronizedRequest.CorrelationId, out existing) == false)
                    {
                        synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
                    }
                }
            }

            Transport.ObjectPublish(synchronizedRequest, address ?? "__BROADCAST", marshal);
            return synchronizedRequest.CorrelationId;
        }

        public TRes InternalCall<TReq, TRes>(TReq request, string address = null, int timeout = 10000)
        {
            SetupReplySubscription();
            var mre = new ManualResetEvent(false);
            var result = default(TRes);

            var sc = InternalCall<TReq, TRes>(request, res =>
            {
                result = res;
                mre.Set();                
            }, address);

            if (mre.WaitOne(timeout))
            {
                return result;
            }
            else
            {
                lock (synchronizationContexts)
                {
                    SynchronizationContext existing = null;
                    if (synchronizationContexts.TryGetValue(sc, out existing) == true)
                    {
                        existing.ResolvedResetEvent.Set();

                        synchronizationContexts.Remove(sc);
                    }
                }
                throw new TimeoutException("Timeout waiting for synchronous call");
            }

        }

        public TRes Call<TReq, TRes>(TReq request, string address = null, int timeout = 10000,
            Func<Func<TReq, TRes>, TReq, TRes> marshal = null)
        {
            if (marshal == null) return InternalCall<TReq, TRes>(request, address, timeout);
            else return marshal((req) => InternalCall<TReq, TRes>(req, address, timeout), request);
            
        }

        public Task<TRes> CallAsync<TReq, TRes>(TReq request, string address = null, int timeout = 10000, Func<Func<TReq, TRes>, TReq, TRes> marshal = null)
        {
            return Task.Factory.StartNew(() => Call<TReq, TRes>(request, address, timeout, marshal));
        }

        // TODO: Decide whether static routes are really necessary, as the tree
        // needs to be built on a per call basis anyway.
        public string Call<TReq>(TReq request, Action<TReq> timeoutHandler = null, string address = null, int timeout = 0, Action<Action> marshal = null)
        {
            SetupReplySubscription();
            var synchronizedRequest = FrameSynchronously(request);

            SynchronizationContext ctx = InstantiatePrototype(request, timeoutHandler, timeout, synchronizedRequest.CorrelationId);
            if (ctx != null) ctx.Request = request;

            Transport.ObjectPublish(synchronizedRequest, address ?? "__BROADCAST", marshal);
            return synchronizedRequest.CorrelationId;
        }

        private SynchronizationContext InstantiatePrototype<TReq>(TReq request, Action<TReq> timeoutHandler, int timeout,
            string correlationId)
        {
            SynchronizationContext prototype;

            lock (staticSynchronizationPrototypes)
            {
                staticSynchronizationPrototypes.TryGetValue(request.GetType(), out prototype);
            }
            if (prototype != null)
            {
                //var synchronizationContext = Serialization.ObjectCopier.Clone(prototype);
                var synchronizationContext = SynchronizationContext.Clone(prototype);

                if (timeout != 0)
                {
                    if (timeoutHandler != null) synchronizationContext.SetTimeoutHandler(timeoutHandler);
                    synchronizationContext.CorrelationId = correlationId;

                    this.timeoutHandler.Timeout(synchronizationContext, timeout);
                }
                // The static route implementation of timeouts needs to wait because
                // anonymous methods cannot be serialized. This will be implemented in the
                // future when I find a better way of cloning the prototypes.

                //else
                //{
                //    foreach (var stack in synchronizationContext.Stacks)
                //    {
                //        if (timeoutHandler != null) stack.SetTimeoutHandler(timeoutHandler);
                //        stack.CorrelationId = synchronizedRequest.CorrelationId;
                //        timeoutStacks.Add(synchronizedRequest.CorrelationId, Timeout(stack, stack.TimeoutMilliseconds));
                //    }
                //}
                foreach (var stack in synchronizationContext.Stacks)
                {
                    foreach (var frame in stack.Frames)
                    {
                        frame.CorrelationId = correlationId;
                        frame.Request = request;
                    }
                }
                lock (synchronizationContexts)
                {
                    synchronizationContexts.Add(correlationId, synchronizationContext);
                }
                return synchronizationContext;
            }
            return null;
        }

        #endregion

        #region OnReply<T, ...>


        private void SetupContext<TReq>(out SynchronizationContext synchronizationContext, out SynchronizationStack synchronizationStack, ContextType type)
        {
            bool success = false;
            lock (staticSynchronizationPrototypes)
            {
                success = staticSynchronizationPrototypes.TryGetValue(typeof(TReq), out synchronizationContext);
                if (success && synchronizationContext.ContextType != type)
                {
                    throw new InvalidOperationException("This response set is already handled with a different response type");
                }
            }
            if (success == false)
            {
                synchronizationContext = new SynchronizationContext();
                synchronizationContext.ContextType = type;
                lock (staticSynchronizationPrototypes)
                {
                    staticSynchronizationPrototypes.Add(typeof(TReq), synchronizationContext);
                }
            }

            synchronizationStack = new SynchronizationStack(synchronizationContext);
        }

        public IResponseContext OnReply<TReq, T>(Action<TReq, T> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            var synchronizationFrame = new SynchronizationFrame<TReq, T> { StaticHandler = handler, Marshal = marshal };

            synchronizationStack.Frames.Add(synchronizationFrame);
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }



        public IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }



        public IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler, Action<Action> marshal = null)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack, ContextType.Static);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6, T7> { StaticHandler = handler, Marshal = marshal });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new Bus.ResponseContext(this);
        }

        #endregion

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler, string address = null, Func<Func<TReq, TRes>, TReq, TRes> marshal = null)
        {
           
            //SetupReplySubscription();
            SetupSubscriber(address);
            SynchronousBlock objectHandler = new SynchronousBlock { Handler = (req) =>
            {
                try
                {
                    if (marshal == null)
                    {
                        return (TRes) handler((TReq) req);
                    }
                    else
                    {
                        return marshal(handler, (TReq)req);
                    }
                }
                catch (Exception ex)
                {
                    RaiseExceptionEvent(ex);
                    return default(TRes);
                }
            }, Address = address ?? "__BROADCAST" };
            lock (replyHandlers)
            {
                if (replyHandlers.ContainsKey(typeof(TReq)) == false)
                {
                    var handlers = new List<SynchronousBlock>();
                    handlers.Add(objectHandler);
                    replyHandlers.Add(typeof(TReq), handlers);
                }
                else
                {
                    replyHandlers[typeof(TReq)].Add(objectHandler);
                }
            }
        }






    }

    internal class SynchronousBlock
    {
        internal Func<object, object> Handler;
        internal string Address;
    }
}
