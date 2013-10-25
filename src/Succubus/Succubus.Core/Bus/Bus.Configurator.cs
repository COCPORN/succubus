using Succubus.Hosting;
using Succubus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Core
{
    public partial class Bus : IBusConfigurator
    {
        
        int publishPort = 9000;
        int subscribePort = 9001;
        bool startMessageHost = false;
        string messageHostname = "localhost";

        IMessageHost messageHost = null;

        public void UseMessageHost(int publishPort = 9000, int subscribePort = 9001)
        {            
            this.publishPort = publishPort;
            this.subscribePort = subscribePort;

            if (startMessageHost == true)
            {
                this.messageHost = new MessageHost();
            }

        }

        public void UseMessageHost(IMessageHost host)
        {
            this.messageHost = host;
        }

        public void SetMessageHostname(string hostname)
        {
            this.messageHostname = hostname;
        }


        string networkName;
        public void SetNetwork(string networkName)
        {
            this.networkName = networkName;
        }

        string publishAddress = null;
        public string PublishAddress
        {
            get { return publishAddress ?? String.Format("tcp://{0}:{1}", messageHostname, publishPort); }
            set { publishAddress = value; }
        }

        string subscribeAddress = null;
        public string SubscribeAddress
        {
            get { return subscribeAddress ?? String.Format("tcp://{0}:{1}", messageHostname, subscribePort); }
            set { subscribeAddress = value; }
        }

        public string MessageNamespace { get; set; }

        public void StartupMessageHost()
        {
            startMessageHost = true;
        }

        
    }
}
