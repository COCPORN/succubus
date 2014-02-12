using System;

namespace Succubus.Core.MessageFrames
{
    public class Synchronous
    {
        public Guid CorrelationId { get; set; }

        public string EmbeddedType { get; set; }

        public string RequestType { get; set; }

        public object Request { get; set; }

        public string ParentType { get; set; }

        public object Message { get; set; }

    }
}
