namespace Succubus.Backend.ZeroMQ
{
    public interface IZeroMQConfigurator
    {

        #region Communication
        string PublishAddress { get; set; }
        string SubscribeAddress { get; set; }
        #endregion


        #region Filtering
        string Network { get; set; }
        #endregion

        #region Configuration sources
        void GetFromConfigurationFile();
        #endregion

        #region Raw
        bool ReportRaw { get; set; }
        #endregion

    }
}