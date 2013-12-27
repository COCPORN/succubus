using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Common.Utils;
using Newtonsoft.Json;
using Succubus.Interfaces;

namespace Succubus.Stores.EventStore
{
    public static class HostConfigurator
    {

        private static IEventStoreConnection connection;
        public static void SetEventStoreConnection(this IMessageHost host, IEventStoreConnection connection)
        {
            HostConfigurator.connection = connection;
        }

        public static void EnableEventStore(this IMessageHost messagehost)
        {           
            messagehost.ProcessedMessage += MessagehostOnProcessedMessage;
        }

        private static void MessagehostOnProcessedMessage(object sender, ProcessedMessageEventArgs eventArgs)
        {
            var type = eventArgs.Message.GetType();
            var classAttributes = type.GetCustomAttributes(true);

            string stream = null;
          
            foreach (var attribute in classAttributes)
            {
                var store = attribute as StoreAttribute;
                if (store != null)
                {
                    stream = store.Stream;
                }
            }

            if (stream != null)
            {
                //connection.AppendToStreamAsync(stream, ExpectedVersion.Any, 
                //    new EventData(Guid.NewGuid(), type.ToString(), true, new object().ToJson(), eventArgs.Message.ToJson())
                //    );
            }
        }

      
    }
}