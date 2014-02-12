using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Succubus.Core.Interfaces;

namespace Succubus.Core
{
    public partial class Bus
    {
        public ITransport Transport { get; set; }
        public ISubscriptionManager SubscriptionManager { get; set; }
        public ICorrelationIdProvider CorrelationIdProvider { get; set; }
    }
}
