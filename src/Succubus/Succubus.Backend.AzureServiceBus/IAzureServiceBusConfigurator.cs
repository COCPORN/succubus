using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.AzureServiceBus
{
    public interface IAzureServiceBusConfigurator
    {
        #region Connection string

        string ConnectionString { get; set; }

        #endregion

        #region Topic creation

        IAzureServiceBusTopicConfigurator WithTopic(string topicName);

        #endregion

        #region Queue creation

        IAzureServiceBusQueueConfigurator WithQueue(string queueName);
            
        #endregion

    }
}
