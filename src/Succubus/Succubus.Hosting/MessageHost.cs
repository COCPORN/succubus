using Succubus.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using ZeroMQ;

namespace Succubus.Hosting
{
    public class MessageHost : IMessageHost
    {
        Thread serverThread;        

        ZmqSocket subscribeSocket;
        ZmqSocket publishSocket;

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

        void ServerThread()
        {
            using (var context = ZmqContext.Create())
            using (subscribeSocket = context.CreateSocket(SocketType.XSUB))
            using (publishSocket = context.CreateSocket(SocketType.XPUB))
            {
                publishSocket.Bind(PublishAddress);
                subscribeSocket.Bind(SubscribeAddress);

                publishSocket.ReceiveReady += publishSocket_ReceiveReady;
                subscribeSocket.ReceiveReady += subscribeSocket_ReceiveReady;

                var poller = new Poller(new List<ZmqSocket> { subscribeSocket, publishSocket });               

                while (true)
                {         
                    poller.Poll();
                }
            }
        }

        void publishSocket_ReceiveReady(object sender, SocketEventArgs e)
        {            
            var message = e.Socket.ReceiveMessage();
            subscribeSocket.SendMessage(message);
        }

        void subscribeSocket_ReceiveReady(object sender, SocketEventArgs e)
        {            
            var message = e.Socket.ReceiveMessage();
            publishSocket.SendMessage(message);
        }


        public string PublishAddress { get; set; }
        
        public string SubscribeAddress { get; set; }
        
    }
}




