using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Interfaces
{
    public interface IBusConfigurator
    {        
        void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool setupHost = true);
        void UseMessageHost(IMessageHost messageHost);
        void SetNetwork(string networkName);
 
    }
}
