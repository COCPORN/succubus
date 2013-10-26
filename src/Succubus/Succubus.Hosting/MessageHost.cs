using Succubus.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using ZeroMQ;

namespace Succubus.Hosting
{
    public class MessageHost : IMessageHost
    {
        public int PublishPort
        {
            get;
            set;
        }

        public int SubscribePort
        {
            get;
            set;
        }
     
        Thread serverThread;        

        ZmqSocket subscribeSocket;
        ZmqSocket publishSocket;

        #region Metrics
        int publishesForwarded = 0;
        int subscribesForwarded = 0;
        #endregion

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

        public string PublishAddress { get { return String.Format("tcp://*:{0}", PublishPort); } }
        public string SubscribeAddress { get { return String.Format("tcp://*:{0}", SubscribePort); } }

        public MessageHost()
        {
            PublishPort = 9001;
            SubscribePort = 9000;
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
            publishesForwarded++;
            var message = e.Socket.ReceiveMessage();
            subscribeSocket.SendMessage(message);
        }

        void subscribeSocket_ReceiveReady(object sender, SocketEventArgs e)
        {
            subscribesForwarded++;
            var message = e.Socket.ReceiveMessage();
            publishSocket.SendMessage(message);
        }        

    }
}




