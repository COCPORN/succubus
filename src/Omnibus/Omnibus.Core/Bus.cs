using Omnibus.Hosting;
using Omnibus.Interfaces;
using Omnibus.Interfaces.ResponseContexts;
using Omnibus.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroMQ;

namespace Omnibus.Core
{
    public class Bus : IBus, IBusConfigurator
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

        Dictionary<Guid, SynchronizationContext> synchronizationContexts = new Dictionary<Guid, SynchronizationContext>();

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
                EmbeddedType = o.GetType().ToString() + ", " + o.GetType().Assembly.GetName().ToString().Split(',')[0]
            };
        }

        public void Call<TReq, TRes>(TReq request, Action<TRes> handler)
        {
            var synchronizationContext = new SynchronizationContext { Static = false };
            synchronizationContext.Frames.Add(new SynchronizationFrame<TRes> { Handler = handler });
            var synchronizedRequest = FrameSynchronously(request);
            synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
            Publish(synchronizedRequest);
        }

        public Guid Call<TReq>(TReq request)
        {
            var synchronizedRequest = FrameSynchronously(request);
            Publish(synchronizationContexts);
            return synchronizedRequest.CorrelationId;
        }

        #endregion

        #region Configuration

        int publishPort = 9000;
        int subscribePort = 9001;
        bool startMessageHost = false;
        string messageHostname = "localhost";

        IMessageHost messageHost = null;

        public void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool startMessageHost = true)
        {
            this.startMessageHost = startMessageHost;
            this.publishPort = publishPort;
            this.subscribePort = subscribePort;

            this.messageHost = new MessageHost();

        }

        public void UseMessageHost(IMessageHost host)
        {
            this.messageHost = host;
        }

        public void SetMessageHostname(string hostname)
        {
            this.messageHostname = hostname;
        }

        #region Configuration


        string publishAddress = null;
        public string PublishAddress
        {
            get { return publishAddress ?? String.Format("tcp://{0}:{1}", messageHostname, publishPort); }
            set { publishAddress = value; }
        }

        string subscribeAddress = null;
        public string SubscribeAddress
        {
            get { return subscribeAddress ?? String.Format("tcp://{0}:{1}", messageHostname, subscribePort); }
            set { subscribeAddress = value; }
        }

        public string MessageNamespace { get; set; }

        #endregion

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

            if (messageHost != null)
            {
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

        ManualResetEvent subscriberOnline = new ManualResetEvent(false);
        bool run = true;
        /// <summary>
        /// The main subscriber loop. Please note the empty try/catch-blocks
        /// around all calls to message and event handlers, they are there
        /// to keep the subscriber loop from going down even if a handler
        /// throws out of its own context.
        /// </summary>
        void Subscriber()
        {
            try
            {
                using (subscribeSocket = context.CreateSocket(SocketType.SUB))
                {
                    ConnectSubscriber();
                    subscriberOnline.Set();
                    while (run)
                    {
                        string typename = subscribeSocket.Receive(Encoding.Unicode);
                        string serialized = subscribeSocket.Receive(Encoding.Unicode);
                        Type coreType = Type.GetType(typename + ", Omnibus.Core");

                        object coreMessage = JsonFrame.Deserlialize(serialized, coreType);

                        var synchronousFrame = coreMessage as SynchronousMessageFrame;
                        if (synchronousFrame != null)
                        {
                            Type type = Type.GetType(synchronousFrame.EmbeddedType);
                            object message = JsonFrame.Deserlialize(synchronousFrame.Message, type);

                            //Func<IRequest, IResponse> handler;
                            //if (handlers.TryGetValue(request.GetType().ToString(), out handler))
                            //{
                            //    var response = handler(request);
                            //    if (response is SynchronousMessage == false)
                            //    {
                            //        throw new ArgumentException("Response needs to derive from SynchronousMessage");
                            //    }
                            //    (response as SynchronousMessage).CorrelationId = (request as SynchronousMessage).CorrelationId;
                            //    PublishObject(response);
                            //}

                            Func<object, object> replyHandler;
                            if (replyHandlers.TryGetValue(type, out replyHandler))
                            {
                                try
                                {
                                    var response = replyHandler(message);
                                    var framedResponse = FrameSynchronously(response, synchronousFrame.CorrelationId);
                                    Publish(framedResponse);
                                }
                                catch { }
                            }

                            SynchronizationContext ctx = null;
                            if (synchronizationContexts.TryGetValue(synchronousFrame.CorrelationId, out ctx))
                            {
                                Type genericType = ctx.Frames.First().GetType().GetGenericArguments()[0];

                                if (genericType == message.GetType())
                                {
                                    ctx.Frames.First().CallHandler(message);
                                }
                                //if (ctx.Frames.First().Satisfies(new List<Type>{ type }))
                                //{
                                    
                                //}
                            }

                         
                        }

                        #region MyRegion
#if false
                                                IRequest requestMessage = message as IRequest;
                                                IResponse responseMessage = message as IResponse;
                                                SynchronousMessage synchronousMessage = message as SynchronousMessage;

                                                if (message is SynchronousMessage && requestMessage != null)
                                                {
                                                    try
                                                    {
                                                        DoReply(requestMessage);
                                                    }
                                                    catch { }
                                                    subscribedMessages++;
                                                    continue;
                                                }

                                                if (synchronousMessages != null && responseMessage != null)
                                                {
                                                    Action<IResponse> handler;
                                                    lock (synchronousMessages)
                                                    {
                                                        if (synchronousMessages.TryGetValue(synchronousMessage.CorrelationId, out handler))
                                                        {
                                                            synchronousMessages.Remove(synchronousMessage.CorrelationId);
                                                            try
                                                            {
                                                                handler(responseMessage);
                                                            }
                                                            catch { }
                                                        }
                                                    }
                                                }

                                                IEvent eventMessage = message as IEvent;
                                                if (eventMessage != null)
                                                {
                                                    EventHandler<EventEventArgs> eh = Event;
                                                    if (eh != null)
                                                    {
                                                        try
                                                        {
                                                            eh(this, new EventEventArgs { Event = eventMessage });
                                                        }
                                                        catch { }
                                                    }
                                                } 
#endif
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Subscriber caught exception: {0}", ex.ToString());
                Console.ReadLine();
            }
        }


        #region OnReply<T, ...>


        public IResponseContext OnReply<TReq, T>(Action<TReq, T> handler)
        {
            throw new NotImplementedException();
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
            ObjectPublish(request);
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

        string networkName;
        public void SetNetwork(string networkName)
        {
            this.networkName = networkName;
        }

        public void SetMessageNamespace(string space)
        {
            MessageNamespace = space;
        }
    }
}
