using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.ZeroMQ.Messages.Diagnostics
{
    public class Pong : IDiagnostics
    {
        public DateTime RequestTime { get; set; }
        public DateTime ResponseTime { get; set; }
        public TimeSpan RequestRoundTripTime { get { return ResponseTime - RequestTime; } }
    }
}
