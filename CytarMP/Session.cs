using System;
using System.Collections.Generic;
using System.Text;
using CytarMP.Network;
using System.IO;
using System.Threading;

namespace CytarMP
{
    public class Session: IDObject
    {
        public NetworkSession NetworkSession { get; protected set; }
        public Thread HandleThread { get; protected set; }
        public Session(NetworkSession netSession)
        {
            NetworkSession = netSession;
        }

        public virtual void Start()
        {
            HandleThread = new Thread(StartHandle);
            HandleThread.Start();
        }

        protected virtual void StartHandle()
        {
            while (NetworkSession.Connected)
            {

            }
        }


        public virtual void Join(Room room)
        {

        }
    }
}