using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using EasyRoute;
using System.IO;

namespace Cytar
{
    public abstract class APIContext: RoutableObject, IDObject, IAPIContext
    {
        public APIContextChildren Children { get; set; }

        public APIContext Parent { get; internal set; }

        public List<Session> Sessions { get; set; }

        public uint ID { get; protected set; }

        public virtual object CallAPI(string name, params object[] param)
        {
            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPIAttribute && (attr as CytarAPIAttribute).Name == name).FirstOrDefault() != null)
                                    .ToArray();
            if (apiMethods.Length <= 0)
            {
                if (Parent == null)
                    throw new APINotFoundException(name);
                return Parent.CallAPI(name, param);
            }
            else
            {
                try
                {
                    return apiMethods[0].Invoke(this, param);
                }
                catch (TargetParameterCountException)
                {
                    throw new ParamsNotMatchException(this, name);
                }
                catch (ArgumentException)
                {
                    throw new ParamsNotMatchException(this, name);
                }
            }
        }
        public virtual object CallPathAPI(string path, params object[] param)
        {
            try
            {
                return Call(path, param);
            }
            catch(MemberNotFoundException)
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
        public APIInfo GetAPI(string name)
        {
            if (name.Contains("/"))
            {
                return GetPathAPI(name);
            }

            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPIAttribute && (attr as CytarAPIAttribute).Name == name).FirstOrDefault() != null)
                                    .ToArray();
            if (apiMethods.Length <= 0)
            {
                if (Parent == null)
                    throw new APINotFoundException(name);
                return Parent.GetAPI(name);
            }
            else
            {
                try
                {
                    return new APIInfo(this, apiMethods[0]);
                }
                catch (TargetParameterCountException)
                {
                    throw new ParamsNotMatchException(this, name);
                }
                catch (ArgumentException)
                {
                    throw new ParamsNotMatchException(this, name);
                }
            }
        }

        public APIContext()
        {
            Children = new APIContextChildren(this);
            Sessions = new List<Session>();
        }
    }
}
