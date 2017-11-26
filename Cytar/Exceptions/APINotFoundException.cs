using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class APINotFoundException: Exception
    {
        public string APIName { get; private set; }
        public APINotFoundException (string apiName):base("API Not Found.")
        {
            APIName = apiName;
        }
    }
}
