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
        public const string ErrorHandlerAPI = "_ERR";
        public const string APIReturnHandlerAPI = "_RTN";
        public virtual NetworkSession NetworkSession { get; protected set; }
        public virtual Thread HandleThread { get; protected set; }

        public virtual List<APIContext> APIContext { get; protected set; }

        public virtual APIContext RootContext { get; set; }

        public virtual event Action<Session, string> Error;
        
        public virtual int PackageSizeLimit { get; set; }

        private int callingID = 0;
        protected virtual int NextCallingID
        {
            get
            {
                return callingID++;
            }
        }

        protected virtual Dictionary<int,RemoteAPIInfo> RemoteCallingRecode { get; set; }

        public uint ID { get; internal set; }

        public Session(NetworkSession netSession):this()
        {
            NetworkSession = netSession;
            netSession.Session = this;
        }

        public Session()
        {
            APIContext = new List<APIContext>();
            PackageSizeLimit = int.MaxValue;
            RemoteCallingRecode = new Dictionary<int, RemoteAPIInfo>();
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
                try
                {
                    HandlePackage(ReadPackage(PackageSizeLimit));
                }
                catch (IOException)
                {

                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, ex.Message);
                }
            }
        }

        protected virtual byte[] ReadPackage(int sizeLimit = int.MaxValue)
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

        protected virtual void HandlePackage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            CytarStreamReader cr = new CytarStreamReader(ms);
            var cid = cr.ReadInt32();
            string apiName = cr.ReadString();
            if(apiName == APIReturnHandlerAPI)
            {
                OnAPIReturn(cid, ms);
                return;
            }
            else if(apiName == ErrorHandlerAPI)
            {
                OnError(cid, cr.ReadObject<RemoteException>());
                return;
            }

            try
            {
                var (result, isVoid) = CallAPI(apiName, ms);
                if (!isVoid)
                {
                    CallRemoteAPI(APIReturnHandlerAPI, result);
                }
            }
            catch (Exception ex)
            {
                CallRemoteAPI(ErrorHandlerAPI, new RemoteException(ex.Message));
            }
        }
        

        public virtual void SendPackage(byte[] data)
        {
            lock (NetworkSession.OutputStream)
            {
                CytarStreamWriter cw = new CytarStreamWriter(NetworkSession.OutputStream);
                cw.Write(data.Length);
                cw.Write(data, 0, data.Length);
                NetworkSession.OutputStream.Flush();
            }
        }

        public virtual void SendPackage(MemoryStream sourceStream)
        {
            lock (NetworkSession.OutputStream)
            {
                CytarStreamReader cr = new CytarStreamReader(sourceStream);
                CytarStreamWriter cw = new CytarStreamWriter(NetworkSession.OutputStream);
                cw.Write((int)sourceStream.Length);
                cw.Write(cr.ReadBytes((int)sourceStream.Length));
                //sourceStream.CopyTo(NetworkSession.OutputStream);
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

        public virtual (object, bool) CallAPI(string apiName, Stream stream)
        {
            var api = GetAPI(apiName);
            var paramsList = new List<object>();
            CytarStreamReader cr = new CytarStreamReader(stream);
            foreach (var paramType in api.Parameters)
            {
                paramsList.Add(cr.ReadObject(paramType));
            }
            return (api.Method.Invoke(api.APIContext, paramsList.ToArray()), api.Method.ReturnType == typeof(void));
        }


        public virtual void CallRemoteAPI(string apiName, Type returnType, Action<object> returnCallback, Action<RemoteException> errorCallback, params object[] param)
        {
            var callingID = NextCallingID;

            RemoteAPIInfo remoteAPI = new RemoteAPIInfo(apiName, callingID, returnType, returnCallback, errorCallback);
            RemoteCallingRecode.Add(remoteAPI.CallingID, remoteAPI);
            MemoryStream ms = new MemoryStream();
            CytarStreamWriter cw = new CytarStreamWriter(ms);
            cw.Write(callingID);
            cw.Write(apiName);
            foreach (var arg in param)
            {
                cw.Write(arg);
            }
            ms.Position = 0;
            SendPackage(ms);
        }
        public virtual void CallRemoteAPI<T>(string apiName, Action<object> returnCallback, Action<RemoteException> errorCallback, params object[] param)
        {
            CallRemoteAPI(apiName, typeof(T), returnCallback, errorCallback, param);
        }
        public virtual void CallRemoteAPI(string apiName, Type returnType, Action<object> callback, params object[] param)
        {
            CallRemoteAPI(apiName, returnType, callback, null, param);
        }
        public virtual void CallRemoteAPI<T>(string apiName, Action<T> callback, params object[] param)
        {
            CallRemoteAPI(apiName, typeof(T), callback, null, param);
        }

        public virtual void CallRemoteAPI(string apiName, Type returnType, params object[] param)
        {
            CallRemoteAPI(apiName, returnType, null, null, param);
        }
        public virtual void CallRemoteAPI<T>(string apiName, params object[] param)
        {
            CallRemoteAPI(apiName, typeof(T), null, null, param);
        }
        public virtual void CallRemoteAPI(string apiName,params object[] param)
        {
            CallRemoteAPI(apiName, typeof(void), null, null, param);
        }

        [CytarAPI(APIReturnHandlerAPI)]
        public virtual void OnAPIReturn(int cid, Stream returnStream)
        {
            if (!RemoteCallingRecode.ContainsKey(cid))
                return;
            CytarStreamReader cr = new CytarStreamReader(returnStream);
            RemoteCallingRecode[cid].Return(cr.ReadObject(RemoteCallingRecode[cid].ReturnType));
            RemoteCallingRecode.Remove(cid);
        }

        [CytarAPI(ErrorHandlerAPI)]
        public virtual void OnError(int cid, RemoteException exception)
        {
            if (!RemoteCallingRecode.ContainsKey(cid))
                return;
            RemoteCallingRecode[cid].OnError(exception);
            RemoteCallingRecode.Remove(cid);
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
            if (path.StartsWith("/"))
            {
                if (this.RootContext == null)
                    throw new NoRootException(this);
                return RootContext.GetPathAPI(path.Substring(1));
            }
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