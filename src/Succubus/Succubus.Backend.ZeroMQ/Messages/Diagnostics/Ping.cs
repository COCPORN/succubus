using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.ZeroMQ.Messages.Diagnostics
{
    class Ping : IDiagnostics
    {
        public DateTime Timestamp { get; set; }
    }
}
