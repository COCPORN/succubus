using System.ComponentModel.Composition;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Succubus.Core.Interfaces;
using Succubus.Hosting.Interfaces;
using Succubus.Serialization;
using ZeroMQ;

namespace Succubus.Hosting
{
    [Export(typeof(IMessageHost))]
    public class MessageHost : IMessageHost, IHostConfigurator
    {
        Thread serverThread;        

        ZmqSocket subscribeSocket;
        ZmqSocket publishSocket;

        readonly ManualResetEvent initializationDone = new ManualResetEvent(false);

        public ManualResetEvent InitializationDone { get { return initializationDone; } }

        public void Start()
        {
            serverThread = new Thread(ServerThread);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        public void Stop()
        {
            publishSocket.Close();
            publishSocket.Dispose();
            subscribeSocket.Close();
            subscribeSocket.Dispose();
        }

        public MessageHost()
        {
            PublishAddress = "tcp://*:9001";
            SubscribeAddress = "tcp://*:9000";
        }

        private ZmqContext context = null;
        public ZmqContext Context
        {
            get { return context;  }
            set { context = value; }
        }

        void ServerThread()
        {
            using (context ?? (context = ZmqContext.Create()))
            using (subscribeSocket = context.CreateSocket(SocketType.XSUB))
            using (publishSocket = context.CreateSocket(SocketType.XPUB))
            {
                publishSocket.Bind(PublishAddress);
                subscribeSocket.Bind(SubscribeAddress);

                publishSocket.ReceiveReady += publishSocket_ReceiveReady;
                subscribeSocket.ReceiveReady += subscribeSocket_ReceiveReady;

                var poller = new Poller(new List<ZmqSocket> { subscribeSocket, publishSocket });

                InitializationDone.Set();

                while (true)
                {         
                    poller.Poll();
                }
            }
        }

        void publishSocket_ReceiveReady(object sender, SocketEventArgs e)
        {            
            var message = e.Socket.ReceiveMessage();
            if (ProcessedMessage != null)
            {
                string address = subscribeSocket.Receive(Encoding.ASCII);
                string typename = subscribeSocket.Receive(Encoding.Unicode);
                string serialized = subscribeSocket.Receive(Encoding.Unicode);
                Type coreType = Type.GetType(typename + ", Succubus.Core");

                object coreMessage = JsonFrame.Deserialize(serialized, coreType);
                if (coreMessage != null)
                {
                    ProcessedMessage(this, new ProcessedMessageEventArgs
                    {
                        Address = address,
                        Message = coreMessage,
                        RawJson = serialized

                    });
                }
            }
            subscribeSocket.SendMessage(message);
        }

        void subscribeSocket_ReceiveReady(object sender, SocketEventArgs e)
        {            
            var message = e.Socket.ReceiveMessage();
            publishSocket.SendMessage(message);
        }


        public string PublishAddress { get; set; }
        
        public string SubscribeAddress { get; set; }


        public void Initialize()
        {
          
        }

        public void Initialize(Action<IHostConfigurator> initializationHandler)
        {
            initializationHandler(this);
        }

        public event EventHandler<ProcessedMessageEventArgs> ProcessedMessage;
    }
}




