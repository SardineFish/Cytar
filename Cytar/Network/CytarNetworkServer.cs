using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cytar.Network
{
    public abstract class CytarNetworkServer
    {
        public abstract bool Running { get; }

        public abstract Thread ServerThread { get; protected set; }

        public Cytar Cytar { get; internal set; }

        public abstract void Start();

        public abstract void Stop();

        public abstract event Action<Exception> OnError;

        public CytarNetworkServer(Cytar cytar)
        {
            Cytar = cytar;
        }
    }
}
