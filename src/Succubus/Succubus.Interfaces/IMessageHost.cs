namespace Succubus.Interfaces
{
    public interface IMessageHost
    {
        int PublishPort { get; set; }

        int SubscribePort { get; set; }

        void Start();
        void Stop();
    }
}
