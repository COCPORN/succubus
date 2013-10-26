using System;
using System.Collections.Generic;

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
            if (handler != null) handler(messages);
        }

        public void CallStaticHandler(Dictionary<Type,object> messages)
        {
            if (staticHandler != null) staticHandler(Request, messages);
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
            else if (responses.Contains(typeof(TRes)))
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
            else if (responses.Contains(typeof(TRes1)) && responses.Contains(typeof(TRes2)))
            {
                return true;
            }
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
        }

        public Action<TRes1, TRes2, TRes3> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) || 
                type == typeof(TRes2) || 
                type == typeof(TRes3)) return true;
            else return false;
        }

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (
                responses.Contains(typeof(TRes1)) && 
                responses.Contains(typeof(TRes2)) &&
                responses.Contains(typeof(TRes3)) 
                )
            {
                return true;
            }
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
        }

        public Action<TRes1, TRes2, TRes3, TRes4> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) ||
                type == typeof(TRes2) ||
                type == typeof(TRes3) ||
                type == typeof(TRes4)) return true;
            else return false;
        }

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (
                responses.Contains(typeof(TRes1)) &&
                responses.Contains(typeof(TRes2)) &&
                responses.Contains(typeof(TRes3)) &&
                responses.Contains(typeof(TRes4))
                )
            {
                return true;
            }
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
        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) ||
                type == typeof(TRes2) ||
                type == typeof(TRes3) ||
                type == typeof(TRes4) ||
                type == typeof(TRes5)) return true;
            else return false;
        }

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (
                responses.Contains(typeof(TRes1)) &&
                responses.Contains(typeof(TRes2)) &&
                responses.Contains(typeof(TRes3)) &&
                responses.Contains(typeof(TRes4)) &&
                responses.Contains(typeof(TRes5))
                )
            {
                return true;
            }
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
        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) ||
                type == typeof(TRes2) ||
                type == typeof(TRes3) ||
                type == typeof(TRes4) ||
                type == typeof(TRes5) ||
                type == typeof(TRes6)) return true;
            else return false;
        }

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (
                responses.Contains(typeof(TRes1)) &&
                responses.Contains(typeof(TRes2)) &&
                responses.Contains(typeof(TRes3)) &&
                responses.Contains(typeof(TRes4)) &&
                responses.Contains(typeof(TRes5)) &&
                responses.Contains(typeof(TRes6))
                )
            {
                return true;
            }
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
        }

        public Action<TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> Handler { get; set; }
        public Action<TReq, TRes1, TRes2, TRes3, TRes4, TRes5, TRes6, TRes7> StaticHandler { get; set; }

        public override bool CanHandle(Type type)
        {
            if (type == typeof(TRes1) ||
                type == typeof(TRes2) ||
                type == typeof(TRes3) ||
                type == typeof(TRes4) ||
                type == typeof(TRes5) ||
                type == typeof(TRes6) ||
                type == typeof(TRes7)) return true;
            else return false;
        }

        public override bool Satisfies(HashSet<Type> responses)
        {
            if (responses.Count == 0)
            {
                return false;
            }
            else if (
                responses.Contains(typeof(TRes1)) &&
                responses.Contains(typeof(TRes2)) &&
                responses.Contains(typeof(TRes3)) &&
                responses.Contains(typeof(TRes4)) &&
                responses.Contains(typeof(TRes5)) &&
                responses.Contains(typeof(TRes6)) &&
                responses.Contains(typeof(TRes7))
                )
            {
                return true;
            }
            else return false;
        }
    }

}
