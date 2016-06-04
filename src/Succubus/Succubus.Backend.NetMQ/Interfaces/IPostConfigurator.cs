using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Backend.NetMQ
{
    public interface IPostConfigurator
    {
        string SubscribeAddress { get; set; }
    }
}
