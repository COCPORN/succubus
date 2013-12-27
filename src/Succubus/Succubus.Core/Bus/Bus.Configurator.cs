using Succubus.Interfaces;
using System.Configuration;

namespace Succubus.Core
{
    public partial class Bus : IBusConfigurator
    {  
        public string Network { get; set;  }

        public string PublishAddress { get; set;  }

        public string SubscribeAddress { get; set; }

        public void GetFromConfigurationFile()
        {
            PublishAddress = ConfigurationManager.AppSettings["Succubus.PublishAddress"];
            SubscribeAddress = ConfigurationManager.AppSettings["Succubus.SubscribeAddress"];
      
      
            Network = ConfigurationManager.AppSettings["Succubus.Network"];
        }


    


     
    }
}
