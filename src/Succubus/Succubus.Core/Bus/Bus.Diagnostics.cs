using System;

namespace Succubus.Core
{
    public partial class Bus
    {

        private ulong sentMessages = 0;
        private ulong receivedMessages = 0;

        public Diagnose GetDiagnose()
        {
            lock (this)
                lock (synchronizationContexts)
                {
                    return new Diagnose()
                    {
                        NumberOfItemsForTimeout = timeoutHandler.NumberOfItemsForTimeout(),
                        SynchronizationContexts = synchronizationContexts.Count,
                        SentMessages = sentMessages,
                        ReceivedMessages = receivedMessages
                    };
                }
        }


    }
}