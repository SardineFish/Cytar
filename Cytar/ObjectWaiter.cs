using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cytar
{
    internal class ObjectWaiter<T>
    {
        internal T Object { get; set; }
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        internal int Count { get; private set; }
        public ObjectWaiter()
        {
        }

        internal T Wait()
        {
            Count++;
            resetEvent.WaitOne();
            Count--;
            return Object;
        }

        internal void Release(T obj)
        {
            Object = obj;
            resetEvent.Set();
        }
    }
}
