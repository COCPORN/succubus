using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroMQ;

namespace Succubus.Serialization
{
    public static class JsonFrame
    {
        public static Frame Serialize<T>(T messageObject)
        {
            var message = JsonConvert.SerializeObject(messageObject);

            return new Frame(Encoding.UTF8.GetBytes(message));
        }

        public static T DeSerialize<T>(Frame frame)
        {
            var messageObject =
                      JsonConvert.DeserializeObject<T>(System.Text.Encoding.UTF8.GetString(frame.Buffer));
            return messageObject;
        }

        public static object Deserlialize(Frame frame)
        {
            object messageObject = JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(frame.Buffer));
            return messageObject;
        }

        public static object Deserlialize(string serialized, Type type)
        {
            object messageObject = JsonConvert.DeserializeObject(serialized, type);
            return messageObject;
        }

        public static string Serialize(object message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}
