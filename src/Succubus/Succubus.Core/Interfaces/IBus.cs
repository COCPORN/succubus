using System;
using System.Threading.Tasks;
using Succubus.Core.Diagnostics;

namespace Succubus.Core.Interfaces
{
    public interface IBus
    {
        // --- INITIALIZATION ---

        void Initialize(Action<IBusConfigurator> initializationHandler);

        // --- EVENT PROCESSING ---

        // Post an event on the bus
        void Publish<T>(T request, string address = null, Action<Action> marshal = null);        

        // Act on events
        IResponseContext On<T>(Action<T> handler, string address = null, Action<Action> marshal = null);        

        // --- SYNCHRONOUS PROCESSING ---

        // One-to-one, only first response is honored
        void Call<TReq, TRes>(TReq request, Action<TRes> handler, string address = null, Action<Action> marshal = null);
        TRes Call<TReq, TRes>(TReq request, string address = null, int timeout = 10000, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);
        Task<TRes> CallAsync<TReq, TRes>(TReq request, string address = null, int timeout = 10000, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);

        // One-to-N+
        string Call<TReq>(TReq request, Action<TReq> timeoutHandler = null, string address = null, int timeout = 60000, Action<Action> marshal = null);

        // Static routes    
        IResponseContext OnReply<TReq, T>(Action<TReq, T> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler, Action<Action> marshal = null);
        IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler, Action<Action> marshal = null);

        // Handle incoming message with a reply, "server side" logic
        void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler, string address = null, Func<Func<TReq, TRes>, TReq, TRes> marshal = null);

        // --- WORKLOAD MANAGEMENT ---

        // Add new work item
        void Queue<T>(T request, string address = null, Action<Action> marshal = null);

        // Get new work item
        IResponseContext Dequeue<T>(Action<T> handler, string address = null, Action<Action> marshal = null);

     
        // --- ERROR HANDLING ---

        event EventHandler<ExceptionEventArgs> HandlerException;

        event EventHandler<ExceptionEventArgs> MessageCreationException;

        // --- DIAGNOSTICS ---

        IResponseContext OnRaw(Action<object> handler, Action<Action> marshal = null);

        Diagnose GetDiagnose();

    }
}
