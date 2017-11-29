using System;
using System.Collections.Generic;
using System.Text;
using Cytar.Network;
using System.IO;
using System.Threading;
using System.Linq;
using EasyRoute;
using Cytar.IO;

namespace Cytar
{
    public class Session: RoutableObject, IDObject, IAPIContext
    {
        public const string ErrorHandlerAPI = "__ERR";
        public virtual NetworkSession NetworkSession { get; protected set; }
        public virtual Thread HandleThread { get; protected set; }

        public virtual List<APIContext> APIContext { get; protected set; }

        public virtual APIContext RootContext { get; set; }

        public virtual event Action<Session, string> Error;

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

        public virtual byte[] ReadPackage(int sizeLimit = int.MaxValue)
        {
            lock (NetworkSession.InputStream)
            {
                CytarStreamReader cr = new CytarStreamReader(NetworkSession.InputStream);
                var size = cr.ReadInt32();
                if (size > sizeLimit)
                    throw new DataSizeException(sizeLimit, size);
                return cr.ReadBytes(size);
            }
        }

        public virtual void SendPackage(byte[] data)
        {
            lock (NetworkSession.OutputStream)
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
                                attr => attr is CytarAPIAttribute && (attr as CytarAPIAttribute).Name == apiName).FirstOrDefault() != null)
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

        public virtual object CallAPI(string apiName, Stream stream)
        {
            try
            {
                var api = GetAPI(apiName);
                var paramsList = new List<object>();
                CytarStreamReader cr = new CytarStreamReader(stream);
                foreach(var paramType in api.Parameters)
                {
                    paramsList.Add(cr.ReadObject(paramType));
                }
                return api.Method.Invoke(api.APIContext, paramsList.ToArray());
            }
            catch (Exception ex)
            {
                CallRemoteAPI(ErrorHandlerAPI, ex.Message);
                return null;
            }
        }

        public virtual void CallRemoteAPI(string apiName,params object[] param)
        {

        }

        public virtual void APIReturn(int cid,params object[] param)
        {

        }

        [CytarAPI(ErrorHandlerAPI)]
        public virtual void OnError(string message)
        {
            if (Error != null)
                Error.Invoke(this, message);
        }

        public virtual object CallPathAPI(string path,params object[] param)
        {
            if (path.StartsWith("/"))
            {
                if (this.RootContext == null)
                    throw new NoRootException(this);
                return this.RootContext.CallPathAPI(path.Substring(1), param);
            }
            try
            {
                return Call(path, param);
            }
            catch (MemberNotFoundException)
            {
                throw new APINotFoundException(path);
            }
            catch (UnreachableException)
            {
                throw new APINotFoundException(path);
            }
        }


        public APIInfo GetPathAPI(string path)
        {
            var pathList = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathList.Length <= 0)
                throw new ArgumentException("Invalid path.");
            else if (pathList.Length == 1)
            {
                var method = GetMethod(pathList[0]);
                return new APIInfo(this, method);
            }
            else
            {
                var subPath = pathList[1];
                for (var i = 2; i < pathList.Length; i++)
                    subPath += "/" + pathList[i];

                try
                {
                    var property = GetProperty(pathList[0]);
                    if (property != null)
                    {
                        if (!property.DeclaringType.IsSubclassOf(typeof(APIContext)))
                            throw new APINotFoundException(path);

                        return (property.GetValue(this) as APIContext).GetPathAPI(subPath);
                    }
                }
                catch (MemberNotFoundException)
                {

                }


                var field = GetField(pathList[0]);
                if (field != null)
                {
                    if (!field.DeclaringType.IsSubclassOf(typeof(APIContext)))
                        throw new APINotFoundException(path);
                    return (field.GetValue(this) as APIContext).GetPathAPI(subPath);
                }

                throw new MemberNotFoundException(this, pathList[0]);
            }
        }

        public APIInfo GetAPI(string apiName)
        {
            if (apiName.Contains("/"))
            {
                return GetPathAPI(apiName);
            }

            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPIAttribute && (attr as CytarAPIAttribute).Name == apiName).FirstOrDefault() != null)
                                    .ToArray();
            if (apiMethods.Length <= 0)
            {
                foreach (var context in APIContext)
                {
                    try
                    {
                        return context.GetAPI(apiName);
                    }
                    catch (APINotFoundException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                throw new APINotFoundException(apiName);
            }
            else
            {
                return new APIInfo(this, apiMethods[0]);
            }
        }
    }
}