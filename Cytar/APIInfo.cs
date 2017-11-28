using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Cytar
{
    public class APIInfo
    {
        public MethodInfo Method { get; private set; }

        public IAPIContext APIContext { get; private set; }

        public string APIName
        {
            get
            {
                if (Method == null)
                    return null;
                var attribute = Method.GetCustomAttributes(true).Where(attr => attr is CytarAPIAttribute).FirstOrDefault() as CytarAPIAttribute;
                if (attribute == null)
                    return null;
                if (attribute.Name == null)
                    return Method.Name;
                return attribute.Name;
            }
        }

        public Type[] Parameters { get; private set; }

        public APIInfo(IAPIContext apiContext, MethodInfo method)
        {
            Method = method;
            APIContext = apiContext;
            var parameters = method.GetParameters();
            Parameters = new Type[parameters.Length];
            for(var i = 0; i < parameters.Length; i++)
            {
                Parameters[i] = parameters[i].ParameterType;
            }
        }
    }
}
