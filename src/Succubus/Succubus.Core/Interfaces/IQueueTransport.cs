using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus.Core.Interfaces
{
    public interface IQueueTransport
    {
        void QueuePublish(object message, string address, Action<Action> marshal = null);     
    }
}
