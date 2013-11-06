using Newtonsoft.Json;
using System;
using System.Text;
using ZeroMQ;

namespace Succubus.Serialization
{
    public static class JsonFrame
    {
        private static JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static Frame Serialize<T>(T messageObject)
        {
            var message = JsonConvert.SerializeObject(messageObject, Formatting.None, settings);

            return new Frame(Encoding.UTF8.GetBytes(message));
        }

        public static T DeSerialize<T>(Frame frame)
        {
            var messageObject =
                      JsonConvert.DeserializeObject<T>(System.Text.Encoding.UTF8.GetString(frame.Buffer), settings);
            return messageObject;
        }

        public static object Deserlialize(Frame frame)
        {
            object messageObject = JsonConvert.DeserializeObject(System.Text.Encoding.UTF8.GetString(frame.Buffer), settings);
            return messageObject;
        }

        public static object Deserlialize(string serialized, Type type)
        {
            object messageObject = JsonConvert.DeserializeObject(serialized, type, settings);
            return messageObject;
        }

        public static string Serialize(object message)
        {
            return JsonConvert.SerializeObject(message, Formatting.None, settings);
        }
    }
}
