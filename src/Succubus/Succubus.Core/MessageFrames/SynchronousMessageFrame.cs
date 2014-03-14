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

    }
}
