using Succubus.Serialization;
using System.Text;
using ZeroMQ;

namespace Succubus.Core
{
    public partial class Bus
    {
        void ObjectPublish(object message, string address)
        {
            lock (publishSocket)
            {
                publishSocket.SendMore(Encoding.ASCII.GetBytes(address));
                var typeIndentifier = message.GetType().ToString();
                publishSocket.SendMore(typeIndentifier, Encoding.Unicode);
                var serialized = JsonFrame.Serialize(message);
                publishSocket.Send(serialized, Encoding.Unicode);
            }
        }


    }
}
