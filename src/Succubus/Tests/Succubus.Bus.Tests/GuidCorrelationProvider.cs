using System;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    class GuidCorrelationProvider : ICorrelationIdProvider
    {
        public string CreateCorrelationId(object o)
        {
            return Guid.NewGuid().ToString();
        }
    }
}