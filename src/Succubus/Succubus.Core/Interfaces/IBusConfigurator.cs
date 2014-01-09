namespace Succubus.Core.Interfaces
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
