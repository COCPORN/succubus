using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.AzureServiceBus
{
    public interface IAzureServiceBusConfigurator
    {
        #region Connection string

        string ConnectionString { get; set; }

        #endregion

    }
}
