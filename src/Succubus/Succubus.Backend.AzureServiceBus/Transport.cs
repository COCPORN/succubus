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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IAzureServiceBusTopicConfigurator WithTopic(string topicName)
        {
            throw new NotImplementedException();
        }

        public IAzureServiceBusQueueConfigurator WithQueue(string queueName)
        {
            throw new NotImplementedException();
        }

        public void BusPublish(object message, string address, Action<Action> marshal = null)
        {
            throw new NotImplementedException();
        }

        public void QueuePublish(object message, string address, Action<Action> marshal = null)
        {
            throw new NotImplementedException();
        }

        public string CreateCorrelationId(object o)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string address)
        {
            throw new NotImplementedException();
        }

        public void SubscribeAll()
        {
            throw new NotImplementedException();
        }
    }
}
