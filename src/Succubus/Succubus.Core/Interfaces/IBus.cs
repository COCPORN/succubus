using System;
using System.Threading.Tasks;
using Succubus.Core.Diagnostics;

namespace Succubus.Core.Interfaces
{
    public interface IBus : IDisposable
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

        

        #endregion

        #region Publish/subscribe

        // Post an event on the bus
        void Publish<T>(T request, string address = null, Action<Action> marshal = null);        

   
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


        Diagnose GetDiagnose();

        string Name { get; }

        #endregion

    }
}
