namespace Succubus.Core.Interfaces
{
    public interface ITransport
    {
        void ObjectPublish(object message, string address);
    }
}