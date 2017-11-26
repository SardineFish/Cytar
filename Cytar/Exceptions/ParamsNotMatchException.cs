using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class ParamsNotMatchException : Exception
    {
        public APIContext APIContext { get; private set; }
        public String APIName { get; private set; }
        public ParamsNotMatchException(APIContext context, string apiName) : base("Params not match.")
        {
            APIContext = context;
            APIName = apiName;
        }
    }
}
