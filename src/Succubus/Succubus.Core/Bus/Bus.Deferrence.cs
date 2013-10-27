using System;
using Succubus.Interfaces.ResponseContexts;

namespace Succubus.Core
{
    public partial class Bus
    {




        public IResponseContext Defer<TReq, T>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2, T3>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2, T3, T4>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6, T7>()
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T>(Guid correlationId, Action<TReq, T> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2>(Guid correlationId, Action<TReq, T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2, T3>(Guid correlationId, Action<TReq, T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4>(Guid correlationId, Action<TReq, T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            throw new NotImplementedException();
        }
    }
    
}