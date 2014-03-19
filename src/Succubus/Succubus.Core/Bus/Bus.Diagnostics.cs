namespace Succubus.Core
{
    public partial class Bus
    {

        public Diagnose GetDiagnose()
        {
            lock (this)
                lock (synchronizationContexts)
                {
                    return new Diagnose()
                    {
                        NumberOfItemsForTimeout = timeoutHandler.NumberOfItemsForTimeout(),
                        SynchronizationContexts = synchronizationContexts.Count
                    };
                }
        }


    }
}