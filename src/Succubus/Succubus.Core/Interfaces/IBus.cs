using System;
using System.Threading.Tasks;
using Succubus.Core.Diagnostics;

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
        void Call<TReq, TRes>(TReq request, Action<TRes> handler, string address = null, Action<Action> marshal = null);
        TRes Call<TReq, TRes>(TReq request, string address = null, int timeout = 10000, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);
        Task<TRes> CallAsync<TReq, TRes>(TReq request, string address = null, int timeout = 10000, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);

        // Calls to static routes
        string Call<TReq>(TReq request, Action<TReq> timeoutHandler = null, string address = null, int timeout = 60000, Action<Action> marshal = null);

        

        // Static routes
        //void OnReply<TReq, T>(Action<TReq, T> handler, Action<TReq> timeoutHandler = null, int timeout = 0);
        void OnReply<TReq, T>(Action<TReq, T> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler, Action<Action> marshal = null);
        void OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler, Action<Action> marshal = null);

        // Handle incoming message with a reply, "server side" logic
        void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler, string address = null, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);

        #endregion

        #region Publish/subscribe

        // Post an event on the bus
        void Publish<T>(T request, string address = null, Action<Action> marshal = null);        

        // Act on events
        void On<T>(Action<T> handler, string address = null, Action<Action> marshal = null);
        
        #endregion

        #region Queue

        void Enqueue<T>(T request, string address = null, Action<Action> marshal = null);        
        void Dequeue<T>(Action<T> handler, string address = null, Action<Action> marshal = null);

        #endregion

        #region Error handling

        event EventHandler<ExceptionEventArgs> HandlerException;

        event EventHandler<ExceptionEventArgs> MessageCreationException;

        event EventHandler<ExceptionEventArgs> TransportException;

        event EventHandler<ExceptionEventArgs> Exception;

        #endregion

        #region Diagnostics

        void OnRawMessage(Action<object> handler, Action<Action> marshal = null);
        void OnRawData(Action<string> handler, Action<Action> marshal = null);

        Diagnose GetDiagnose();

        string Name { get; }

        #endregion

    }
}
