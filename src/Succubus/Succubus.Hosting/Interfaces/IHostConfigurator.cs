namespace Succubus.Hosting.Interfaces
{
    public interface IHostConfigurator
    {
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; } 
    }
}