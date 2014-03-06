using System;
using System.Text;
using Newtonsoft.Json;
using Succubus.Core.Interfaces;
using ZeroMQ;

namespace Succubus.Serialization
{
    public static class JsonFrame 
    {
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static Frame Serialize<T>(T messageObject)
        {
            var message = JsonConvert.SerializeObject(messageObject, Formatting.None, settings);

            return new Frame(Encoding.UTF8.GetBytes(message));
        }

        public static  T DeSerialize<T>(Frame frame)
        {
            var messageObject =
                      JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(frame.Buffer), settings);
            return messageObject;
        }

        public static object Deserlialize(Frame frame)
        {
            var messageObject = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(frame.Buffer), settings);
            return messageObject;
        }

        public static object Deserialize(string serialized, Type type)
        {
            var messageObject = JsonConvert.DeserializeObject(serialized, type, settings);
            return messageObject;
        }

        public static string Serialize(object message)
        {
            return JsonConvert.SerializeObject(message, Formatting.None, settings);
        }

     
    }
}
