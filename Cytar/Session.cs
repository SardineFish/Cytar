﻿using System;
using System.Collections.Generic;
using System.Text;
using Cytar.Network;
using System.IO;
using System.Threading;

namespace Cytar
{
    public class Session: IDObject
    {
        public virtual NetworkSession NetworkSession { get; protected set; }
        public virtual Thread HandleThread { get; protected set; }

        public virtual List<APIContext> APIContext { get; protected set; }
        public Session(NetworkSession netSession)
        {
            NetworkSession = netSession;
            netSession.Session = this;
            APIContext = new List<APIContext>();
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


        public virtual void Join(APIContext context)
        {
            if (!context.Sessions.Contains(this))
                context.Sessions.Add(this);
            if (!APIContext.Contains(context))
                APIContext.Add(context);
        }

        public virtual void Exit(APIContext context)
        {
            if (context.Sessions.Contains(this))
                context.Sessions.Remove(this);
            if (APIContext.Contains(context))
                APIContext.Remove(context);
        }

        public virtual void Close(int errCode)
        {

        }

        public virtual void APIRoute(string apiName,object[] param)
        {
            
        }
    }
}