using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRoute
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class Routable : Attribute
    {
        public string Name { get; private set; }
        public Routable(string name)
        {
            Name = name;
        }
        public Routable()
        {
            Name = null;
        }
    }
}
