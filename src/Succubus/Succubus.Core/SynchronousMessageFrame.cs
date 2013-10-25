using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Core
{
    public class SynchronousMessageFrame 
    {
        public Guid CorrelationId { get; set; }

        public string EmbeddedType { get; set; }

        public string Message { get; set; }

    }
}
