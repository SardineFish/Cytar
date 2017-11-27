﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Cytar
{
    public abstract class APIContext: IDObject
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
            var pathList = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathList.Length == 1)
            {
                return CallAPI(pathList[0], param);
            }

        }

        public APIContext()
        {
            Children = new APIContextChildren(this);
            Sessions = new List<Session>();
        }
    }
}
