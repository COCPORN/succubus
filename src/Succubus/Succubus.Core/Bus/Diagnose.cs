namespace Succubus.Core
{
    /// <summary>
    /// A snapshot of the current bus state
    /// </summary>
    public class Diagnose
    {
        public int SynchronizationContexts { get; set; }

        public int NumberOfItemsForTimeout { get; set; }

        public ulong ReceivedMessages { get; set; }
        public ulong SentMessages { get; set;  }
    }
}