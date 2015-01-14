using System;
using System.IO;

namespace Succubus.Core.Interfaces
{
    public interface IBusConfigurator
    {
        bool IncludeMessageOriginator { get; set; }
        TextWriter LogWriter { get; set; }
        LogLevel LogLevel { get; set; }
        string Name { get; set; }
        ITransport Transport { get; set; }
        ISubscriptionManager SubscriptionManager { get; set; }
        ICorrelationIdProvider CorrelationIdProvider { get; set; }
        ITransportBridge Bridge { get; set; }

        #region Handlers

        #region Synchronous
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

        #region Events

        // Act on events
        void On<T>(Action<T> handler, string address = null, Action<Action> marshal = null);

        #endregion

        #region Diagnostics

        void OnRawMessage(Action<object> handler, Action<Action> marshal = null);
        void OnRawData(Action<string> handler, Action<Action> marshal = null);

        #endregion

        #endregion
    }
}
