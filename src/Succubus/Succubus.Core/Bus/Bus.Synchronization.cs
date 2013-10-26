using Succubus.Interfaces.ResponseContexts;
using Succubus.Serialization;
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
        Dictionary<Guid,
            SynchronizationContext> synchronizationContexts =
            new Dictionary<Guid, SynchronizationContext>();

        /// <summary>
        /// This is used to populate synchronizationcontexts with static
        /// prototypes.
        /// </summary>
        Dictionary<Type,
            SynchronizationContext> staticSynchronizationPrototypes = new Dictionary<Type, SynchronizationContext>();



        /// <summary>
        /// This is used to create "server" logic to respond to synchronous messages.
        /// </summary>
        Dictionary<Type, List<Func<object, object>>> replyHandlers = new Dictionary<Type, List<Func<object, object>>>();

        #endregion


        #region Synchronous messaging

        SynchronousMessageFrame FrameSynchronously(object o, Guid? guid = null)
        {
            return new SynchronousMessageFrame
            {
                Message = JsonFrame.Serialize(o),
                CorrelationId = guid ?? Guid.NewGuid(),
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],
                RequestType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0]
            };
        }

        EventMessageFrame FrameEvent(object o)
        {
            return new EventMessageFrame
            {
                Message = JsonFrame.Serialize(o),
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0]
            };
        }

        SynchronousMessageFrame FrameResponseSynchronously(SynchronousMessageFrame request, object o, Guid? guid = null)
        {
            return new SynchronousMessageFrame
            {
                Message = JsonFrame.Serialize(o),
                CorrelationId = guid ?? Guid.NewGuid(),
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0],
                RequestType = request.RequestType
            };
        }

        public void Call<TReq, TRes>(TReq request, Action<TRes> handler)
        {
            var synchronizationContext = new SynchronizationContext();


            SynchronizationStack stack = new SynchronizationStack(synchronizationContext);
            stack.Frames.Add(new SynchronizationFrame<TReq, TRes> { Handler = handler });
            synchronizationContext.Stacks.Add(stack);

            var synchronizedRequest = FrameSynchronously(request);
            lock (synchronizationContexts)
            {
                synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
            }
            ObjectPublish(synchronizedRequest);
        }

        // TODO: Decide whether static routes are really necessary, as the tree
        // needs to be built on a per call basis anyway.
        public Guid Call<TReq>(TReq request)
        {
            var synchronizedRequest = FrameSynchronously(request);

            SynchronizationContext prototype = null;

            lock (staticSynchronizationPrototypes)
            {
                staticSynchronizationPrototypes.TryGetValue(typeof (TReq), out prototype);
            }
            if (prototype != null)
            {
                var synchronizationContext = Serialization.ObjectCopier.Clone<SynchronizationContext>(prototype);
                synchronizationContext.Static = true;
                foreach (var stack in synchronizationContext.Stacks)
                {
                    foreach (var frame in stack.Frames)
                    {
                        frame.Request = request;
                    }
                }
                lock (synchronizationContexts)
                {
                    synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
                }
            }

            ObjectPublish(synchronizedRequest);
            return synchronizedRequest.CorrelationId;
        }

        #endregion

        #region OnReply<T, ...>


        private void SetupContext<TReq>(out SynchronizationContext synchronizationContext, out SynchronizationStack synchronizationStack)
        {
            bool success = false;
            lock (staticSynchronizationPrototypes)
            {
                success = staticSynchronizationPrototypes.TryGetValue(typeof (TReq), out synchronizationContext);
            }
            if (success == false)
            {
                synchronizationContext = new SynchronizationContext();
                lock (staticSynchronizationPrototypes)
                {
                    staticSynchronizationPrototypes.Add(typeof (TReq), synchronizationContext);
                }
            }

            synchronizationStack = new SynchronizationStack(synchronizationContext);
        }

        public IResponseContext OnReply<TReq, T>(Action<TReq, T> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }



        public IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }



        public IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            SynchronizationContext synchronizationContext;
            SynchronizationStack synchronizationStack;
            SetupContext<TReq>(out synchronizationContext, out synchronizationStack);
            synchronizationStack.Frames.Add(new SynchronizationFrame<TReq, T1, T2, T3, T4, T5, T6, T7> { StaticHandler = handler });
            synchronizationContext.Stacks.Add(synchronizationStack);

            return new ResponseContext(this);
        }

        #endregion

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler)
        {
            Func<object, object> objectHandler = new Func<object, object>((req) => (TRes)handler((TReq)req));
            lock (replyHandlers)
            {
                if (replyHandlers.ContainsKey(typeof (TReq)) == false)
                {
                    var handlers = new List<Func<object, object>>();
                    handlers.Add(objectHandler);
                    replyHandlers.Add(typeof (TReq), handlers);
                }
                else
                {
                    replyHandlers[typeof (TReq)].Add(objectHandler);
                }
            }
        }






    }
}
