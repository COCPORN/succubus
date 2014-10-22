using Succubus.Core.Interfaces;
using System.Configuration;

namespace Succubus.Core
{
    public partial class Bus : IBusConfigurator
    {

        public ITransportBridge Bridge { get; set; }

     

    }
}
