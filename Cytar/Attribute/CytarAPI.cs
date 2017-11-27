using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Cytar;
using EasyRoute;

namespace Cytar
{
    [System.AttributeUsage(
        AttributeTargets.Method | 
        AttributeTargets.Property | 
        AttributeTargets.Field | 
        AttributeTargets.Class | 
        AttributeTargets.Struct, 
        Inherited = false, 
        AllowMultiple = false)]
    public class CytarAPI : Routable
    {
        public CytarAPI(string name): base(name)
        {
        }
        
    }
}