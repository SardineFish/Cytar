using System;
using System.Collections.Generic;
using System.Text;
using Cytar.Network;
using System.IO;
using System.Threading;
using System.Linq;

namespace Cytar
{
    public class Session: IDObject
    {
        public virtual NetworkSession NetworkSession { get; protected set; }
        public virtual Thread HandleThread { get; protected set; }

        public virtual List<APIContext> APIContext { get; protected set; }

        public virtual APIContext RootContext { get; set; }

        public uint ID { get; internal set; }

        public Session(NetworkSession netSession)
        {
            NetworkSession = netSession;
            netSession.Session = this;
            APIContext = new List<APIContext>();
        }

        public Session()
        {
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

        public virtual object CallAPI(string apiName, params object[] param)
        {
            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPI && (attr as CytarAPI).Name == apiName).FirstOrDefault() != null)
                                    .ToArray();
            if (apiMethods.Length <= 0)
            {
                foreach (var context in APIContext)
                {
                    try
                    {
                        return context.CallAPI(apiName, param);
                    }
                    catch (APINotFoundException)
                    {
                        continue;
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }
                throw new APINotFoundException(apiName);
            }
            else
            {
                return apiMethods[0].Invoke(this, param);
            }
        }

        public virtual object CallPathAPI(string path,params object[] param)
        {
            if (this.RootContext == null)
                throw new NoRootException(this);
            return null;
        }
    }
}