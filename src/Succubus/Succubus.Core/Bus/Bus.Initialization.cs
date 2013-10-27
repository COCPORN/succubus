using Succubus.Hosting;
using Succubus.Interfaces;
using Succubus.Interfaces.ResponseContexts;
using System;
using System.Threading;
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

            timeoutThread = new Thread(TimeoutThread);
            timeoutThread.IsBackground = true;
            timeoutThread.Start();
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

    


       
  

    }
}
