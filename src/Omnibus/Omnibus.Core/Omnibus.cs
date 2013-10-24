using Omnibus.Interfaces;
using Omnibus.Interfaces.ResponseContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Core
{
    public class Omnibus : IBus, IBusConfigurator
    {
        class ResponseContext : IResponseContext
        {
            public IResponseContext Then<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

            public IResponseContext Finally<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

            public IResponseContext Execute()
            {
                throw new NotImplementedException();
            }
        }

   

        #region Configuration

        int publishPort = 9000;
        int subscribePort = 9001;
        bool startMessageHost = false;

        public void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool startMessageHost = true)
        {
            this.startMessageHost = startMessageHost;
            this.publishPort = publishPort;
            this.subscribePort = subscribePort;
        }

        public void UseMessageHost(IMessageHost host)
        {

        }
        #endregion

        bool initialized = false;
        public void Initialize()
        {
            lock (this)
            {
                if (initialized == true)
                {
                    throw new InvalidOperationException("Bus is already initialized");
                }
            }

        }

        public void Initialize(Action<IBusConfigurator> initializationHandler)
        {
            initializationHandler(this);
            Initialize();
        }



        public void Call<TReq, TRes>(TReq request, Action<TRes> handler)
        {
            throw new NotImplementedException();
        }

        public void Call<TReq>(TReq request)
        {
            throw new NotImplementedException();
        }

        #region OnReply<T, ...>
        
        public IResponseContext OnReply<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2>(Action<T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2, T3>(Action<T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T request)
        {
            throw new NotImplementedException();
        }

        #region On<T, ...>
        
        public IResponseContext On<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2>(Action<T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2, T3>(Action<T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext On<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            throw new NotImplementedException();
        }

        #endregion

        string networkName;
        public void SetNetwork(string networkName)
        {
            this.networkName = networkName;
        }
    }
}
