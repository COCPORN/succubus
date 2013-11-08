using System;

namespace Succubus.Core
{
    public class SynchronousMessageFrame 
    {
        public Guid CorrelationId { get; set; }

        public string EmbeddedType { get; set; }

        public string RequestType { get; set; }

        public object Request { get; set; }

        public string ParentType { get; set; }

        public string Message { get; set; }

    }
}
