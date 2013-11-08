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
            get; set; 
        }

        public string MessageHostSubscribeAddress
        {
            get; set;
        }


        public void ConfigureForTesting()
        {
            StartMessageHost = true;
            //MessageHostPublishAddress = "inproc://mh-pub";
            //MessageHostSubscribeAddress = "inproc://mh-sub";
            //PublishAddress = MessageHostSubscribeAddress;
            //SubscribeAddress =  MessageHostPublishAddress;
            //messageHost = new MessageHost();
            //(messageHost as MessageHost).Context = this.Context;
        }
    }
}
