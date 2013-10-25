using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Interfaces
{
    public interface IMessageHost
    {
        int PublishPort { get; set; }

        int SubscribePort { get; set; }

        void Start();
        void Stop();
    }
}
