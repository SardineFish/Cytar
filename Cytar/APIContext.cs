using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;
using RoutableObject;

namespace Cytar
{
    public abstract class APIContext: RoutableObject.RoutableObject, IDObject
    {
        public APIContextChildren Children { get; set; }

        public APIContext Parent { get; internal set; }

        public List<Session> Sessions { get; set; }

        public uint ID { get; internal set; }

        public virtual object CallAPI(string name, params object[] param)
        {
            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPI && (attr as CytarAPI).Name == name).FirstOrDefault() != null)
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

        public APIContext()
        {
            Children = new APIContextChildren(this);
            Sessions = new List<Session>();
        }
    }
}
