namespace Succubus.Interfaces
{
    public interface IMessageHost
    {
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; }

        void Start();
        void Stop();
    }
}
