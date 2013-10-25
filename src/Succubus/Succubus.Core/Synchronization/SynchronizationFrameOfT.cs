using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Succubus
{
    [Serializable]
    abstract class SynchronizationFrame
    {
        public abstract bool Satisfies(HashSet<Type> responses);

        public Type GenericType { get; set; }

        protected Action<object> handler;

        protected Action<object, object> staticHandler;

        public void CallHandler(object message)
        {
            handler(message);
        }

        public void CallStaticHandler(object message)
        {
            staticHandler(Request, message);
        }

        public object Request { get; set; }

        public bool Resolved { get; set; }

        public abstract bool CanHandle(Type type);
    }

    [Serializable]
    class SynchronizationFrame<TReq, TRes> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<object>(message => Handler((TRes)message));
            staticHandler = new Action<object, object>((request, response) => StaticHandler((TReq)request, (TRes)response));
        }

        public Action<TRes> Handler { get; set; }
        public Action<TReq, TRes> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes)) return true;
            else return false;
        } 

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (responses.Contains(typeof(TReq)))
            {
                return true;
            }
            else return false;
        }
    }

#if false

    class SynchronizationFrame<T1, T2> : SynchronizationFrame
    {
        public Action<T1, T2> Handler { get; set; }

        public override bool Satisfies(List<Type> responses)
        {
            if (responses.Count != 2)
            {
                return false;
            }
            else if (responses[0].GetType() == typeof(T1))
            {
                return true;
            }
            else return false;
        }
    }
#endif
}
