﻿using Succubus.Hosting;
using Succubus.Interfaces;
using Succubus.Interfaces.ResponseContexts;
using System;
using System.Threading;
using ZeroMQ;

namespace Succubus.Core
{
    public partial class Bus : IBus
    {

        private class ResponseContext : IResponseContext
        {
            private Bus bus;

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


        private ZmqContext context;
        private ZmqSocket publishSocket;
        private ZmqSocket subscribeSocket;

        #endregion

        #region Threading

        private Thread subscriberThread;

        #endregion


        #endregion

        #region Initialization

        public Bus()
        {
            PublishAddress = "tcp://localhost:9000";
            SubscribeAddress = "tcp://localhost:9001";
            context = ZmqContext.Create();
        }

        public ZmqContext Context { get { return context; } set { context = value; } }

        private bool initialized = false;

        public void Initialize()
        {
            lock (this)
            {
                if (initialized == true)
                {
                    throw new InvalidOperationException("Bus is already initialized");
                }
                initialized = true;
            }

        

            if (StartMessageHost == true)
            {
                if (messageHost == null) messageHost = new MessageHost();
                if (MessageHostPublishAddress != null) messageHost.PublishAddress = MessageHostPublishAddress;
                if (MessageHostSubscribeAddress != null) messageHost.SubscribeAddress = MessageHostSubscribeAddress;
                messageHost.Start();
            }

          

            ConnectPublisher();

            subscriberThread = new Thread(Subscriber) { IsBackground = true };
            subscriberThread.Start();

            timeoutHandler.timeoutThread = new Thread(timeoutHandler.TimeoutThread) { IsBackground = true };
            timeoutHandler.timeoutThread.Start();
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

        private void ConnectSubscriber()
        {
            subscribeSocket.Connect(SubscribeAddress);
            subscribeSocket.SubscribeAll();
        }


    }




}
