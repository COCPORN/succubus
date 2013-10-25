using Succubus.Hosting;
using Succubus.Interfaces;
using Succubus.Interfaces.ResponseContexts;
using Succubus.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace Succubus.Core
{
    public partial class Bus : IBus
    {

        class ResponseContext : IResponseContext
        {
            Bus bus;

            public ResponseContext(Bus bus)
            {
                this.bus = bus;
            }

            public IResponseContext On<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

            public IResponseContext Then<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

        }

        #region Members

        #region ØMQ


        ZmqContext context;
        ZmqSocket publishSocket;
        ZmqSocket subscribeSocket;

        #endregion

        #region Threading

        Thread subscriberThread;

        #endregion

        #region Synchronization stores and event handlers

        Dictionary<Guid, 
            SynchronizationContext> transientSynchronizationContexts = 
            new Dictionary<Guid, SynchronizationContext>();

        Dictionary<Type, 
            Dictionary<Guid, SynchronizationContext>> staticSynchronizationContexts = 
            new Dictionary<Type, Dictionary<Guid, SynchronizationContext>>();

        Dictionary<Type,
            SynchronizationContext> staticSynchronizationPrototypes = new Dictionary<Type, SynchronizationContext>();

        Dictionary<Type, Action<object>> eventHandlers = new Dictionary<Type, Action<object>>();

        Dictionary<Type, Func<object, object>> replyHandlers = new Dictionary<Type, Func<object, object>>();

        #endregion



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
            synchronizationContext.Frames.Add(new SynchronizationFrame<TReq, TRes> { Handler = handler });
            var synchronizedRequest = FrameSynchronously(request);
            transientSynchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
            ObjectPublish(synchronizedRequest);
        }

        // TODO: Decide whether static routes are really necessary, as the tree
        // needs to be built on a per call basis anyway.
        public Guid Call<TReq>(TReq request)
        {
            var synchronizedRequest = FrameSynchronously(request);

            SynchronizationContext prototype;

            if (staticSynchronizationPrototypes.TryGetValue(typeof(TReq), out prototype)) {
                var synchronizationContext = Serialization.ObjectCopier.Clone<SynchronizationContext>(prototype);
                synchronizationContext.Static = true;
                foreach (var frame in synchronizationContext.Frames)
                {
                    frame.Request = request;
                }
                transientSynchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
            }

            ObjectPublish(synchronizedRequest);
            return synchronizedRequest.CorrelationId;
        }

        #endregion

       

        #region Initialization


        bool initialized = false;
        public void Initialize()
        {
            lock (this)
            {
                if (initialized == true)
                {
                    throw new InvalidOperationException("Bus is already initialized");
                }
            }

            if (startMessageHost == true)
            {
                if (messageHost == null) messageHost = new MessageHost();
                messageHost.Start();
            }

            context = ZmqContext.Create();

            ConnectPublisher();

            subscriberThread = new Thread(new ThreadStart(Subscriber));
            subscriberThread.IsBackground = true;
            subscriberThread.Start();
        }

        public void Initialize(Action<IBusConfigurator> initializationHandler)
        {
            initializationHandler(this);
            Initialize();
        }

        #endregion

        private void ConnectPublisher()
        {
            publishSocket = context.CreateSocket(SocketType.PUB);
            publishSocket.Connect(PublishAddress);
        }

        void ConnectSubscriber()
        {
            subscribeSocket.Connect(SubscribeAddress);
            subscribeSocket.SubscribeAll();
        }

    


        #region OnReply<T, ...>


        public IResponseContext OnReply<TReq, T>(Action<TReq, T> handler)
        {
            if (staticSynchronizationPrototypes.ContainsKey(typeof(TReq)))
            {
                throw new InvalidOperationException("Bus already contains a static route for this type");
            }
            
            var synchronizationContext = new SynchronizationContext();
            synchronizationContext.Frames.Add(new SynchronizationFrame<TReq, T> { StaticHandler = handler });
            staticSynchronizationPrototypes.Add(typeof(TReq), synchronizationContext);
            
            return new ResponseContext(this);
        }

        public IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler)
        {            
            Func<object, object> objectHandler = new Func<object, object>((req) => (TRes)handler((TReq)req));
            replyHandlers.Add(typeof(TReq), objectHandler);
        }

        void ObjectPublish(object message)
        {
            lock (publishSocket)
            {
                var typeIndentifier = message.GetType().ToString();
                publishSocket.SendMore(typeIndentifier, Encoding.Unicode);
                var serialized = JsonFrame.Serialize(message);
                publishSocket.Send(serialized, Encoding.Unicode);
            }
        }

        public void Publish<T>(T request)
        {
            ObjectPublish(FrameEvent(request));
        }

        #region On<T>

        public IResponseContext On<T>(Action<T> handler)
        {
            if (eventHandlers.ContainsKey(typeof(T)))
            {
                throw new ArgumentException("Type already has a handler");
            }
            Action<object> myHandler = new Action<object>(response => handler((T)response));
            eventHandlers.Add(typeof(T), myHandler);
            return new ResponseContext(this);
        }

        #endregion

  

    }
}
