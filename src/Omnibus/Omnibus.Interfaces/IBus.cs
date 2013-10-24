﻿using Omnibus.Interfaces.ResponseContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Interfaces
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

        // Calls to static routes
        void Call<TReq>(TReq request);

        // Static routes
        IResponseContext OnReply<T>(Action<T> handler);
        IResponseContext OnReply<T1, T2>(Action<T1, T2> handler);
        IResponseContext OnReply<T1, T2, T3>(Action<T1, T2, T3> handler);
        IResponseContext OnReply<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler);
        IResponseContext OnReply<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler);
        IResponseContext OnReply<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> handler);
        IResponseContext OnReply<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> handler);

        // Handle incoming message with a reply, "server side" logic
        void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler);

        #endregion

        #region Publish/subscribe

        // Post an event on the bus
        void Publish<T>(T request);      

        // Act on events
        IResponseContext On<T>(Action<T> handler);
        IResponseContext On<T1, T2>(Action<T1, T2> handler);
        IResponseContext On<T1, T2, T3>(Action<T1, T2, T3> handler);
        IResponseContext On<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler);
        IResponseContext On<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler);
        IResponseContext On<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> handler);
        IResponseContext On<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> handler);
        
        #endregion

        #region Fan out
        #endregion

    }
}
