using System;

namespace Succubus.Bus.Tests.Messages
{
    [Serializable]
    public class BasicRequest
    {
        public string Message { get; set; }
    }
}