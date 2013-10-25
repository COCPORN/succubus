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

        protected Action<Dictionary<Type,object>> handler;

        protected Action<object, Dictionary<Type, object>> staticHandler;

        public void CallHandler(Dictionary<Type,object> messages)
        {
            handler(messages);
        }

        public void CallStaticHandler(Dictionary<Type,object> messages)
        {
            staticHandler(Request, messages);
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
            handler = new Action<Dictionary<Type,object>>(
                messages => Handler((TRes)messages[typeof(TRes)])
            );
            staticHandler = new Action<object, Dictionary<Type, object>>(
                (request, responses) => StaticHandler((TReq)request, (TRes)responses[typeof(TRes)]));
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
        }

        public Action<TRes1, TRes2> Handler { get; set; }
        public Action<TReq, TRes1, TRes2> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) || type == typeof(TRes2)) return true;
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
