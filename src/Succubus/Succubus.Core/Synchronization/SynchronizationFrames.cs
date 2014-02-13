using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Runtime.Remoting;
using System.Threading;

namespace Succubus
{
    [Serializable]
    abstract class SynchronizationFrame
    {
        internal SynchronizationFrame Clone()
        {
            SynchronizationFrame frame = (SynchronizationFrame)this.MemberwiseClone();
            return frame;
        }

        public abstract bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages);

        public Type GenericType { get; set; }

        protected Action<Dictionary<Type, object>> handler;

        protected Action<object, Dictionary<Type, object>> staticHandler;

        protected Action<string, object, Dictionary<Type, object>> correlationHandler;

        readonly ManualResetEvent handleEvent = new ManualResetEvent(false);
        public ManualResetEvent HandleEvent { get { return handleEvent; } }

        public void CallHandler(Dictionary<Type, object> messages)
        {
            if (handler != null) handler(messages);
            handleEvent.Set();
        }

        public void CallStaticHandler(Dictionary<Type, object> messages)
        {
            if (staticHandler != null) staticHandler(Request, messages);
        }

        public void CallDeferredHandler(Dictionary<Type, object> messages)
        {
            if (correlationHandler != null) correlationHandler(CorrelationId, Request, messages);
        }

        public string CorrelationId { get; set; }

        public object Request { get; set; }

        public bool Resolved { get; set; }


        public abstract bool CanHandle(object type);
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler((TRes)messages[typeof(TRes)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) => StaticHandler((TReq)request, (TRes)responses[typeof(TRes)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
                 (id, request, responses) => CorrelationHandler(id, (TReq)request, (TRes)responses[typeof(TRes)]));

        }

        public Action<TRes> Handler { get; set; }
        public Action<TReq, TRes> StaticHandler { get; set; }
        public Action<string, TReq, TRes> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            if (type is TRes) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes)
                {
                    messages.Add(typeof(TRes), type.Value);
                }
            }
            if (messages.Count == 1) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler((TRes1)messages[typeof(TRes1)], (TRes2)messages[typeof(TRes2)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
                 (id, request, responses) => CorrelationHandler(id, (TReq)request,
                     (TRes1)responses[typeof(TRes1)],
                     (TRes2)responses[typeof(TRes2)]));
        }

        public Action<TRes1, TRes2> Handler { get; set; }
        public Action<TReq, TRes1, TRes2> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            // if (type.IsInstanceOfType(typeof (TRes1)) || type.IsInstanceOfType(typeof (TRes2))) return true;
            if (type is TRes1 || type is TRes2) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
            }
            if (messages.Count == 2) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2, TRes3> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler((TRes1)messages[typeof(TRes1)], (TRes2)messages[typeof(TRes2)], (TRes3)messages[typeof(TRes3)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)],
                        (TRes3)responses[typeof(TRes3)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
                 (id, request, responses) => CorrelationHandler(id, (TReq)request,
                     (TRes1)responses[typeof(TRes1)],
                     (TRes2)responses[typeof(TRes2)],
                     (TRes3)responses[typeof(TRes3)]));
        }

        public Action<TRes1, TRes2, TRes3> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2, TRes3> CorrelationHandler { get; set; }


        public override bool CanHandle(object type)
        {
            if (type is TRes1 ||
                type is TRes2 ||
                type is TRes3) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
                else if (type.Value is TRes3)
                {
                    messages.Add(typeof(TRes3), type.Value);
                }
            }
            if (messages.Count == 3) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2, TRes3, TRes4> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler(
                    (TRes1)messages[typeof(TRes1)],
                    (TRes2)messages[typeof(TRes2)],
                    (TRes3)messages[typeof(TRes3)],
                    (TRes4)messages[typeof(TRes4)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)],
                        (TRes3)responses[typeof(TRes3)],
                        (TRes4)responses[typeof(TRes4)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
                (id, request, responses) => CorrelationHandler(id, (TReq)request,
                    (TRes1)responses[typeof(TRes1)],
                    (TRes2)responses[typeof(TRes2)],
                    (TRes3)responses[typeof(TRes3)],
                    (TRes4)responses[typeof(TRes4)]));
        }

        public Action<TRes1, TRes2, TRes3, TRes4> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2, TRes3, TRes4> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            if (type is TRes1 ||
                type is TRes2 ||
                type is TRes3 ||
                type is TRes4) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
                else if (type.Value is TRes3)
                {
                    messages.Add(typeof(TRes3), type.Value);
                }
                else if (type.Value is TRes4)
                {
                    messages.Add(typeof(TRes4), type.Value);
                }
            }
            if (messages.Count == 4) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2, TRes3, TRes4, TRes5> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler(
                    (TRes1)messages[typeof(TRes1)],
                    (TRes2)messages[typeof(TRes2)],
                    (TRes3)messages[typeof(TRes3)],
                    (TRes4)messages[typeof(TRes4)],
                    (TRes5)messages[typeof(TRes5)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)],
                        (TRes3)responses[typeof(TRes3)],
                        (TRes4)responses[typeof(TRes4)],
                        (TRes5)responses[typeof(TRes5)]));

            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
               (id, request, responses) => CorrelationHandler(id, (TReq)request,
                   (TRes1)responses[typeof(TRes1)],
                   (TRes2)responses[typeof(TRes2)],
                   (TRes3)responses[typeof(TRes3)],
                   (TRes4)responses[typeof(TRes4)],
                   (TRes5)responses[typeof(TRes5)]));
        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2, TRes3, TRes4, TRes5> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            if (type is TRes1 ||
                type is TRes2 ||
                type is TRes3 ||
                type is TRes4 ||
                type is TRes5) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
                else if (type.Value is TRes3)
                {
                    messages.Add(typeof(TRes3), type.Value);
                }
                else if (type.Value is TRes4)
                {
                    messages.Add(typeof(TRes4), type.Value);
                }
                else if (type.Value is TRes5)
                {
                    messages.Add(typeof(TRes5), type.Value);
                }
            }
            if (messages.Count == 5) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler(
                    (TRes1)messages[typeof(TRes1)],
                    (TRes2)messages[typeof(TRes2)],
                    (TRes3)messages[typeof(TRes3)],
                    (TRes4)messages[typeof(TRes4)],
                    (TRes5)messages[typeof(TRes5)],
                    (TRes6)messages[typeof(TRes6)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)],
                        (TRes3)responses[typeof(TRes3)],
                        (TRes4)responses[typeof(TRes4)],
                        (TRes5)responses[typeof(TRes5)],
                        (TRes6)responses[typeof(TRes6)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
               (id, request, responses) => CorrelationHandler(id, (TReq)request,
                   (TRes1)responses[typeof(TRes1)],
                   (TRes2)responses[typeof(TRes2)],
                   (TRes3)responses[typeof(TRes3)],
                   (TRes4)responses[typeof(TRes4)],
                   (TRes5)responses[typeof(TRes5)],
                   (TRes6)responses[typeof(TRes6)]));
        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            if (type is TRes1 ||
                type is TRes2 ||
                type is TRes3 ||
                type is TRes4 ||
                type is TRes5 ||
                type is TRes6) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
                else if (type.Value is TRes3)
                {
                    messages.Add(typeof(TRes3), type.Value);
                }
                else if (type.Value is TRes4)
                {
                    messages.Add(typeof(TRes4), type.Value);
                }
                else if (type.Value is TRes5)
                {
                    messages.Add(typeof(TRes5), type.Value);
                }
                else if (type.Value is TRes6)
                {
                    messages.Add(typeof(TRes6), type.Value);
                }
            }
            if (messages.Count == 6) return true;
            else return false;

        }
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<Dictionary<Type, object>>(
                messages => Handler(
                    (TRes1)messages[typeof(TRes1)],
                    (TRes2)messages[typeof(TRes2)],
                    (TRes3)messages[typeof(TRes3)],
                    (TRes4)messages[typeof(TRes4)],
                    (TRes5)messages[typeof(TRes5)],
                    (TRes6)messages[typeof(TRes6)],
                    (TRes7)messages[typeof(TRes7)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) =>
                    StaticHandler((TReq)request, (TRes1)responses[typeof(TRes1)],
                        (TRes2)responses[typeof(TRes2)],
                        (TRes3)responses[typeof(TRes3)],
                        (TRes4)responses[typeof(TRes4)],
                        (TRes5)responses[typeof(TRes5)],
                        (TRes6)responses[typeof(TRes6)],
                        (TRes7)responses[typeof(TRes7)]));
            correlationHandler = new Action<string, object, Dictionary<Type, object>>(
                (id, request, responses) => CorrelationHandler(id, (TReq)request,
                    (TRes1)responses[typeof(TRes1)],
                    (TRes2)responses[typeof(TRes2)],
                    (TRes3)responses[typeof(TRes3)],
                    (TRes4)responses[typeof(TRes4)],
                    (TRes5)responses[typeof(TRes5)],
                    (TRes6)responses[typeof(TRes6)],
                    (TRes7)responses[typeof(TRes7)]));

        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> StaticHandler { get; set; }
        public Action<string, TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> CorrelationHandler { get; set; }

        public override bool CanHandle(object type)
        {
            if (type is TRes1 ||
                type is TRes2 ||
                type is TRes3 ||
                type is TRes4 ||
                type is TRes5 ||
                type is TRes6 ||
                type is TRes7) return true;
            else return false;
        }

        public override bool Satisfies(Dictionary<Type, object> responses, out Dictionary<Type, object> messages)
        {
            messages = new Dictionary<Type, object>();
            foreach (var type in responses)
            {
                if (type.Value is TRes1)
                {
                    messages.Add(typeof(TRes1), type.Value);
                }
                else if (type.Value is TRes2)
                {
                    messages.Add(typeof(TRes2), type.Value);
                }
                else if (type.Value is TRes3)
                {
                    messages.Add(typeof(TRes3), type.Value);
                }
                else if (type.Value is TRes4)
                {
                    messages.Add(typeof(TRes4), type.Value);
                }
                else if (type.Value is TRes5)
                {
                    messages.Add(typeof(TRes5), type.Value);
                }
                else if (type.Value is TRes6)
                {
                    messages.Add(typeof(TRes6), type.Value);
                }
                else if (type.Value is TRes7)
                {
                    messages.Add(typeof(TRes7), type.Value);
                }
            }
            if (messages.Count == 7) return true;
            else return false;

        }
    }

}
