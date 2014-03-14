using Succubus.Core.Interfaces;

namespace Succubus.Core.MessageFrames
{
    public class Event : IMessageFrame
    {        
        public string EmbeddedType { get; set; }

        public object Message { get; set; }
    
    }
}
