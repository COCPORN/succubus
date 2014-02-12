namespace Succubus.Core.Interfaces
{
    public interface ISubscriptionManager
    {
        void Subscribe(string address); 
        void SubscribeAll();
    }
}