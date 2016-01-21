using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.AzureServiceBus
{
    class Transport : IAzureServiceBusConfigurator, ITransport, ICorrelationIdProvider, ISubscriptionManager
    {
        public string ConnectionString
        {
            get; set;
        }

        NamespaceManager namespaceManager;
        MessagingFactory messagingFactory;
        

        public void Initialize()
        {
            string connectionString =
                CloudConfigurationManager.GetSetting(ConnectionString);

            namespaceManager =
                NamespaceManager.CreateFromConnectionString(connectionString);

            messagingFactory = MessagingFactory.CreateFromConnectionString(ConnectionString); 
        }

        public void BusPublish(object message, string address, Action<Action> marshal = null)
        {
            if (marshal == null) BusPublish(message, address);
            else marshal(() => BusPublish(message, address));
        }

        public void BusPublish(object message, string address)
        {
            var destination = String.IsNullOrEmpty(address) ? message.GetType().Name : address;
            var client = messagingFactory.CreateTopicClient(destination);
            var bmessage = new BrokeredMessage(message);
            bmessage.Properties.Add("Type", message.GetType().ToString());
            client.Send(bmessage);
        }

        public void QueuePublish(object message, string address, Action<Action> marshal = null)
        {
            throw new NotImplementedException();
        }

        public string CreateCorrelationId(object o)
        {
            var message = o as BrokeredMessage;
            if (message != null)
            {
                return message.MessageId;
            }
            throw new InvalidOperationException("Unable to create correlation ID");
        }

        Dictionary<string, Subscriber> subscribers = new Dictionary<string, Subscriber>();

        public void Subscribe(string address)
        {
            lock (subscribers)
            {
                if (subscribers.ContainsKey(address)) return;                
            }
            if (!namespaceManager.TopicExists(address))
            {
                namespaceManager.CreateTopic(address);
            }
            if (!namespaceManager.SubscriptionExists(address, "AllMessages"))
            {
                namespaceManager.CreateSubscription(address, "AllMessages");
            }
            lock (subscribers)
            {
                var client = SubscriptionClient.Create(address, "AllMessages");
                subscribers.Add(address, new Subscriber(client, this));
            }
        }

        public void SubscribeAll()
        {
            throw new InvalidOperationException();
        }
    }
}
