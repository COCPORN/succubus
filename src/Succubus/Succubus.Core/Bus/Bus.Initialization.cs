﻿using System.ComponentModel.Composition;
using Succubus.Core.Interfaces;
using System;
using System.Threading;

namespace Succubus.Core
{
    [Export(typeof(IBus))]
    public partial class Bus : IBus
    {

        public bool IncludeMessageOriginator { get; set; }

        private string machineName = null;

        private string MachineName
        {
            get
            {
                return machineName ?? 
                    (IncludeMessageOriginator ? (machineName = Environment.MachineName) : (machineName = String.Empty ));
            }
        }

        #region Initialization




        void Initialize()
        {
            if (Transport == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing transport"));
            if (CorrelationIdProvider == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing correlation id provider"));
            if (SubscriptionManager == null) throw new TypeInitializationException(this.GetType().FullName, new ArgumentException("Missing subscription manager"));
        }

        public void Initialize(Action<IBusConfigurator> initializationHandler)
        {           
            IncludeMessageOriginator = true;
            Bridge = this;
            initializationHandler(this);            
            Initialize();
            if (String.IsNullOrEmpty(Name))
            {
                Name = Guid.NewGuid().ToString();
            }
            Info("Bus initialized");
        }

        #endregion


        #region Disposing

        public void Dispose()
        {
            var disposable = Transport as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion


    }




}
