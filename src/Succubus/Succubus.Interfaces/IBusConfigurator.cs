namespace Succubus.Interfaces
{
    public interface IBusConfigurator
    {        
        void UseMessageHost(int publishPort = 9000, int subscribePort = 9001);
        void UseMessageHost(IMessageHost messageHost);
        void SetNetwork(string networkName);
        void SetMessageHostname(string hostname);
        void StartupMessageHost();
    }
}
