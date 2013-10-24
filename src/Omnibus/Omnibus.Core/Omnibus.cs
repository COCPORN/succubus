using Omnibus.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Core
{
    public class Omnibus : IBus, IBusConfigurator
    {
        public void Call<TReq, TRes>(TReq request, Action<TRes> handler)
        {
            throw new NotImplementedException();
        }

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(T request)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T>(Action<T> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2>(Action<T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2, T3>(Action<T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2, T3, T4>(Action<T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public Interfaces.ResponseContexts.IResponseContext On<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        #region Configuration       

        int publishPort = 9000;
        int subscribePort = 9001;
        bool startMessageHost = false;

        public void UseMessageHost(int publishPort = 9000, int subscribePort = 9001)
        {
            startMessageHost = true;
            this.publishPort = publishPort;
            this.subscribePort = subscribePort;
        }
        #endregion

        bool initialized = false;
        public void Initialize()
        {
            lock (this) {
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
    }
}
