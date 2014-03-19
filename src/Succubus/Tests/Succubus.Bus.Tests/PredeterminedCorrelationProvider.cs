using System.Collections.Generic;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    class PredeterminedCorrelationProvider : ICorrelationIdProvider
    {
        private List<string> correlationIds = new List<string>
        {
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1",
            "cid1"
        };

        public string CreateCorrelationId(object o)
        {
#if false
            string cid = correlationIds[0];
            correlationIds.RemoveAt(0);
            return cid;
#else
            return "cid1";
#endif
        }
    }
}