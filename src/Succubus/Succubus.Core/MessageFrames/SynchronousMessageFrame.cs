using System;
using Succubus.Core.Interfaces;

namespace Succubus.Core.MessageFrames
{
    public class Synchronous : IMessageFrame
    {
        public string CorrelationId { get; set; }

        public string EmbeddedType { get; set; }

        public string RequestType { get; set; }

        public object Request { get; set; }

        public string ParentType { get; set; }

        public object Message { get; set; }

        public override string ToString()
        {
            return String.Format("Sync {2} CId: {0} Type: {1} ", CorrelationId, EmbeddedType,
                Request == null ? "Request" : "Response");
        }
    }
}
