using Succubus.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Bus.Tests
{
    interface IFactory
    {
        IBus CreateBus(bool reportRaw = false);
        IBus CreateBusWithHosting(bool reportRaw = false);        
    }
}
