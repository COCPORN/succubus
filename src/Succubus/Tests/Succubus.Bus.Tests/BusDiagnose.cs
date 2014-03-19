using NUnit.Framework;
using Succubus.Core.Interfaces;

namespace Succubus.Bus.Tests
{
    public class BusDiagnose
    {
        public static void CheckDiagnose(IBus bus)
        {
            var diagnose = bus.GetDiagnose();
            Assert.AreEqual(0, diagnose.NumberOfItemsForTimeout, "Items for timeout");
            Assert.AreEqual(0, diagnose.SynchronizationContexts, "Synchronization contexts");
        }
    }
}