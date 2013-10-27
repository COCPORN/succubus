using System.Security.Cryptography.X509Certificates;

namespace Succubus.Interfaces
{
    public interface IBusConfigurator
    {
        #region Communication        
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; }
        #endregion

        #region Hosting
        string MessageHostPublishAddress { get; set; }
        string MessageHostSubscribeAddress { get; set; }

        void StartupMessageHost();
        #endregion

        #region Filtering
        void SetNetwork(string networkName);
        #endregion

        #region Configuration sources
        void GetFromConfigurationFile();
        #endregion
        
    }
}
