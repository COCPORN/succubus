using System;
using System.Collections.Generic;
using Succubus.Core.Interfaces;
using Succubus.Hosting.Interfaces;
using Succubus.Backend.NetMQ;

namespace Succubus.Hosting
{
    public static class Configuration
    {
        static Dictionary<INetMQConfigurator, MessageHost> messageHosts = new Dictionary<INetMQConfigurator, MessageHost>();

        public static void StartMessageHost(this INetMQConfigurator busConfigurator)
        {
            StartMessageHost(busConfigurator, host =>
            {
                host.PublishAddress = "tcp://*:9001";
                host.SubscribeAddress = "tcp://*:9000";
            });

        }

        public static void StartMessageHost(this INetMQConfigurator busConfigurator,
            Action<IHostConfigurator> hostConfigurator)
        {

            lock (messageHosts)
            {
                var messageHost = new MessageHost();
                hostConfigurator(messageHost);
                messageHost.Initialize(host =>
                {
                    host.PublishAddress = messageHost.PublishAddress;
                    host.SubscribeAddress = messageHost.SubscribeAddress;
                });

                messageHost.Start();
                messageHosts.Add(busConfigurator, messageHost);
                messageHost.InitializationDone.WaitOne(0);
            }
        }


    }
}