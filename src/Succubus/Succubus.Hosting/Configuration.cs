using System;
using System.Collections.Generic;
using Succubus.Backend.ZeroMQ;
using Succubus.Core.Interfaces;
using Succubus.Hosting.Interfaces;

namespace Succubus.Hosting
{
    public static class Configuration
    {
        static Dictionary<IZeroMQConfigurator, MessageHost> messageHosts = new Dictionary<IZeroMQConfigurator, MessageHost>();

        public static void StartMessageHost(this IZeroMQConfigurator busConfigurator)
        {
            StartMessageHost(busConfigurator, host =>
            {
                host.PublishAddress = "tcp://*:9001";
                host.SubscribeAddress = "tcp://*:9000";
            });

        }

        public static void StartMessageHost(this IZeroMQConfigurator busConfigurator,
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