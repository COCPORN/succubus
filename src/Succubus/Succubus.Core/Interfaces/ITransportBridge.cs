using System;

namespace Succubus.Core.Interfaces
{
    public interface ITransportBridge
    {
        void ProcessSynchronousMessages(MessageFrames.Synchronous synchronousFrame, string address);
        void ProcessEvents(MessageFrames.Event eventFrame, string address);
        void ProcessCatchAllEvents(MessageFrames.Synchronous eventFrame, string address);
        void UnableToCreateMessage(Exception exception);
        void RawMessage(object o);       
    }
}