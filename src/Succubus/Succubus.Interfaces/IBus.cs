using System.Data.SqlTypes;
using Succubus.Interfaces.ResponseContexts;
using System;

namespace Succubus.Interfaces
{
    public interface IBus
    {
        #region Initialization
        void Initialize();
        void Initialize(Action<IBusConfigurator> initializationHandler);
        #endregion

        #region Synchronous

        // Make a "synchronous" call

        // Throw away calls
        void Call<TReq, TRes>(TReq request, Action<TRes> handler);
        TRes Call<TReq, TRes>(TReq request);

        // Calls to static routes
        Guid Call<TReq>(TReq request, Action<TReq> timeoutHandler = null, int timeout = 60000);

        

        // Static routes
        //IResponseContext OnReply<TReq, T>(Action<TReq, T> handler, Action<TReq> timeoutHandler = null, int timeout = 0);
        IResponseContext OnReply<TReq, T>(Action<TReq, T> handler);
        IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler);
        IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler);
        IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler);

        // Handle incoming message with a reply, "server side" logic
        void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler);

        #endregion

        #region Deferrence
        
        IResponseContext Defer<TReq, T>();
        IResponseContext Defer<TReq, T1, T2>();
        IResponseContext Defer<TReq, T1, T2, T3>();
        IResponseContext Defer<TReq, T1, T2, T3, T4>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6, T7>();

        IResponseContext Pickup<TReq, T>(Guid correlationId, Action<TReq, T> handler);
        IResponseContext Pickup<TReq, T1, T2>(Guid correlationId, Action<TReq, T1, T2> handler);
        IResponseContext Pickup<TReq, T1, T2, T3>(Guid correlationId, Action<TReq, T1, T2, T3> handler);
        IResponseContext Pickup<TReq, T1, T2, T3, T4>(Guid correlationId, Action<TReq, T1, T2, T3, T4> handler);
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5> handler);
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler);
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler);

        #endregion

        #region Publish/subscribe

        // Post an event on the bus
        void Publish<T>(T request);        

        // Act on events
        IResponseContext On<T>(Action<T> handler);
        
        #endregion

        #region Fan out
        #endregion

    }
}
