using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CytarMP.Network
{
    public abstract class CytarNetworkServer
    {
        public abstract bool Running { get; }

        public abstract Thread ServerThread { get; protected set; }

        public CytarMP CytarMP { get; internal set; }

        public abstract void Start();

        public abstract void Stop();

        public abstract event Action<Exception> OnError;

        public CytarNetworkServer(CytarMP cytarMP)
        {
            CytarMP = cytarMP;
        }
    }
}
