using Succubus.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Succubus.Core
{
    public partial class Bus
    {
        void ObjectPublish(object message)
        {
            lock (publishSocket)
            {
                var typeIndentifier = message.GetType().ToString();
                publishSocket.SendMore(typeIndentifier, Encoding.Unicode);
                var serialized = JsonFrame.Serialize(message);
                publishSocket.Send(serialized, Encoding.Unicode);
            }
        }


    }
}
