namespace Succubus.Core.MessageFrames
{
    public abstract class MessageBase
    {
        public string Originator { get; set; }

        public string Responder { get; set; }

        public string EmbeddedType { get; set; }

        public object Message { get; set; } 
    }
}