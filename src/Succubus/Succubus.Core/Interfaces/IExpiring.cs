using System;
using System.Security.Cryptography.X509Certificates;

namespace Succubus.Collections.Interfaces
{
    public interface IExpiring<T>
    {
        bool TimedOut { get; set; }
        Action TimeoutHandler { get; }
        T Id { get; }
        void OnTimeout();
    }
}