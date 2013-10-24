using Omnibus.Hosting;
using Omnibus.Interfaces;
using Omnibus.Interfaces.ResponseContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus.Core
{
    public class Bus : IBus, IBusConfigurator
    {
        class ResponseContext : IResponseContext
        {
            Bus bus;

            public ResponseContext(Bus bus)
            {
                this.bus = bus;
            }

            public IResponseContext On<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

            public IResponseContext Then<T>(Action<T> handler)
            {
                throw new NotImplementedException();
            }

        }

        #region Synchronous messaging

        Dictionary<Guid, SynchronizationContext> synchronizationContexts = new Dictionary<Guid,SynchronizationContext>();

        SynchronousMessageFrame Frame(object o)
        {
            return new SynchronousMessageFrame { Message = o, CorrelationId = Guid.NewGuid() };
        }

        public void Call<TReq, TRes>(TReq request, Action<TRes> handler)
        {
            var synchronizationContext = new SynchronizationContext { Static = false };
            synchronizationContext.Frames.Add(new SynchronizationFrame<TRes> { Handler = handler });
            var synchronizedRequest = Frame(request);
            synchronizationContexts.Add(synchronizedRequest.CorrelationId, synchronizationContext);
            Publish(synchronizedRequest);
        }

        public Guid Call<TReq>(TReq request)
        {
            var synchronizedRequest = Frame(request);
            Publish(synchronizationContexts);
            return synchronizedRequest.CorrelationId;
        }


        #endregion

        #region Configuration

        int publishPort = 9000;
        int subscribePort = 9001;
        bool startMessageHost = false;

        IMessageHost messageHost;

        public void UseMessageHost(int publishPort = 9000, int subscribePort = 9001, bool startMessageHost = true)
        {
            this.startMessageHost = startMessageHost;
            this.publishPort = publishPort;
            this.subscribePort = subscribePort;

            this.messageHost = new MessageHost();

        }

        public void UseMessageHost(IMessageHost host)
        {
            this.messageHost = host;
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



 
        #region OnReply<T, ...>


        public IResponseContext OnReply<TReq, T>(Action<TReq, T> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2>(Action<TReq, T1, T2> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3>(Action<TReq, T1, T2, T3> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4>(Action<TReq, T1, T2, T3, T4> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5>(Action<TReq, T1, T2, T3, T4, T5> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6>(Action<TReq, T1, T2, T3, T4, T5, T6> handler)
        {
            throw new NotImplementedException();
        }

        public IResponseContext OnReply<TReq, T1, T2, T3, T4, T5, T6, T7>(Action<TReq, T1, T2, T3, T4, T5, T6, T7> handler)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void ReplyTo<TReq, TRes>(Func<TReq, TRes> handler)
        {
            throw new NotImplementedException();
        }

        void Publish(object request)
        {

        }

        public void Publish<T>(T request)
        {
            Publish(request);
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
