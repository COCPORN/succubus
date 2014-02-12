using System;

namespace Succubus.Core.Interfaces
{
    public interface ISerializer
    {
        string Serialize(object o);
        object Deserialize(string message, Type type);

    }
}