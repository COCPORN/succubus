using System.Security.Cryptography.X509Certificates;

namespace Succubus.Interfaces
{
    public interface IBusConfigurator
    {
        #region Communication        
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; }
        #endregion


        #region Filtering
        string Network { get; set;  }        
        #endregion

        #region Configuration sources
        void GetFromConfigurationFile();
        #endregion


    }
}
