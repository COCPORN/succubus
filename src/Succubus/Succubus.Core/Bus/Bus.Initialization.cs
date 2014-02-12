using System.ComponentModel.Composition;
using Succubus.Core.Interfaces;
using System;
using System.Threading;

namespace Succubus.Core
{
    [Export(typeof(IBus))]
    public partial class Bus : IBus
    {

        private class ResponseContext : IResponseContext
        {
            private Bus bus;

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

        #region Members



        #endregion

        #region Initialization




        void Initialize()
        {
            if (Transport == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing transport"));
            //if (Serializer == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing serializer"));
            if (SubscriptionManager == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing subscription manager"));
        }

        public void Initialize(Action<IBusConfigurator> initializationHandler)
        {
            Bridge = this;
            initializationHandler(this);
            Initialize();
        }

        #endregion



    }




}
