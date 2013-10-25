using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnibus
{
    abstract class SynchronizationFrame
    {
        public abstract bool Satisfies(List<Type> responses);

        public Type GenericType { get; set; }

        protected Action<object> handler;

        public void CallHandler(object message)
        {
            handler(message);
        }
    }

    class SynchronizationFrame<T> : SynchronizationFrame
    {
        public SynchronizationFrame()
        {
            handler = new Action<object>(message => Handler((T)message));
        }

        public Action<T> Handler { get; set; }

        public override bool Satisfies(List<Type> responses)
        {
            if (responses.Count != 1)
            {
                return false;
            }
            else if (responses[0].GetType() == typeof(T))
            {
                return true;
            }
            else return false;
        }
    }

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
}
