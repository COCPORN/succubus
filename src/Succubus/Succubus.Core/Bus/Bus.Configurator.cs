using System;
using Succubus.Hosting;
using Succubus.Interfaces;
using System.Configuration;

namespace Succubus.Core
{
    public partial class Bus : IBusConfigurator
    {

        public bool StartMessageHost { get; set; }
        
        IMessageHost messageHost = null;    

        public string Network { get; set;  }

        public string PublishAddress { get; set;  }

        public string SubscribeAddress { get; set; }

        public string MessageNamespace { get; set; }
       

        public void GetFromConfigurationFile()
        {
            PublishAddress = ConfigurationManager.AppSettings["Succubus.PublishAddress"];
            SubscribeAddress = ConfigurationManager.AppSettings["Succubus.SubscribeAddress"];

            bool startMessageHost = false;
            if (bool.TryParse(ConfigurationManager.AppSettings["Succubus.Hosting.StartMessageHost"], out startMessageHost) == false) {
                StartMessageHost = false;
            }
            StartMessageHost = startMessageHost;

            MessageHostPublishAddress = ConfigurationManager.AppSettings["Succubus.Hosting.PublishAddress"];
            MessageHostSubscribeAddress = ConfigurationManager.AppSettings["Succubus.Hosting.SubscribeAddress"];

            Network = ConfigurationManager.AppSettings["Succubus.Network"];
        }


        public string MessageHostPublishAddress
        {
            get
            {
                if (messageHost != null)
                {
                    return messageHost.PublishAddress;
                }
                else return null;
            }
            set
            {
                if (messageHost != null)
                {
                    messageHost.PublishAddress = value;
                }
            }
        }

        public string MessageHostSubscribeAddress
        {
            get
            {
                if (messageHost != null)
                {
                    return messageHost.SubscribeAddress;
                }
                else return null;
            }
            set
            {
                if (messageHost != null)
                {
                    messageHost.SubscribeAddress = value;
                }
            }
        }
    }
}
