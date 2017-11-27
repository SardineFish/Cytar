using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Cytar;

namespace Cytar
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CytarAPI : NameAttribute
    {
        public CytarAPI(string name): base(name)
        {
        }
        
    }
}