using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.AzureServiceBus
{
    class Subscriber
    {
        SubscriptionClient subscriptionClient;
        Transport transport;
        MethodInfo genericMethod;
        public Subscriber(SubscriptionClient subscriptionClient, Transport transport, Type type)
        {
            this.subscriptionClient = subscriptionClient;
            this.transport = transport;
            subscriptionClient.OnMessage(HandleMessage);
            MethodInfo method = typeof(SubscriptionClient).GetMethod("GetBody");
            genericMethod = method.MakeGenericMethod(type);
            
        }

        void HandleMessage(BrokeredMessage message)
        {
            object typename;
            if (message.Properties.TryGetValue("Type", out typename))
            {
                string s_typename = typename as string;
                if (s_typename != null)
                {
                    Type coreType = Type.GetType(typename + ", Succubus.Core");
                    var obj = genericMethod.Invoke(message, null);
                }
            }
        }
    }
}
