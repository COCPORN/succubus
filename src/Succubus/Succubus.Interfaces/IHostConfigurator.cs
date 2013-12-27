namespace Succubus.Interfaces
{
    public interface IHostConfigurator
    {
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; } 
    }
}