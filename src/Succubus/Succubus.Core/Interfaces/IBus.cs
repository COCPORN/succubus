using System;
using System.Threading.Tasks;

namespace Succubus.Core.Interfaces
{
    public interface IBus
    {
        #region Initialization
        void Initialize(Action<IBusConfigurator> initializationHandler);
        #endregion

        #region Synchronous

        // Make a "synchronous" call

        // Throw away calls
        void Call<TReq, TRes>(TReq request, Action<TRes> handler, string address = null);
        TRes Call<TReq, TRes>(TReq request, string address = null, int timeout = 10000);
        Task<TRes> CallAsync<TReq, TRes>(TReq request, string address = null, int timeout = 10000);

        // Calls to static routes
        Guid Call<TReq>(TReq request, Action<TReq> timeoutHandler = null, string address = null, int timeout = 60000);

        

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
        void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler, string address = null);

        #endregion

        #region Deferrence
        
        IResponseContext Defer<TReq, T>();
        IResponseContext Defer<TReq, T1, T2>();
        IResponseContext Defer<TReq, T1, T2, T3>();
        IResponseContext Defer<TReq, T1, T2, T3, T4>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6>();
        IResponseContext Defer<TReq, T1, T2, T3, T4, T5, T6, T7>();

        IResponseContext Pickup<TReq, T>(Guid correlationId, Action<TReq, T> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2>(Guid correlationId, Action<TReq, T1, T2> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2, T3>(Guid correlationId, Action<TReq, T1, T2, T3> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2, T3, T4>(Guid correlationId, Action<TReq, T1, T2, T3, T4> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6> handler) where TReq : class;
        IResponseContext Pickup<TReq, T1, T2, T3, T4, T5, T6, T7>(Guid correlationId, Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler) where TReq : class;

        #endregion

        #region Publish/subscribe

        // Post an event on the bus
        void Publish<T>(T request, string address = null);        

        // Act on events
        IResponseContext On<T>(Action<T> handler, string address = null);
        
        #endregion

        #region Fan out
        #endregion

    }
}
