using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Cytar
{
    public abstract class APIContext
    {
        public APIContextChildren Children { get; set; }

        public APIContext Parent { get; internal set; }

        public List<Session> Sessions { get; set; }

        public virtual object CallAPI(string name, object[] param)
        {
            var apiMethods = this.GetType().GetMethods().Where(
                        method => method.GetCustomAttributes(true).Where(
                                attr => attr is CytarAPI && (attr as CytarAPI).Name == name).FirstOrDefault() != null)
                                    .ToArray();
            if (apiMethods.Length <= 0)
            {
                if (Parent == null)
                    throw new Exception("API Not Found.");
                return Parent.CallAPI(name, param);
            }
            else
            {
                return apiMethods[0].Invoke(this, param);
            }

        }

        public APIContext()
        {
            Children = new APIContextChildren(this);
            Sessions = new List<Session>();
        }
    }
}
