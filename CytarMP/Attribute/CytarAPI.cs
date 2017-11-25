using System;
using System.Collections.Generic;
using System.Text;

namespace CytarMP
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CytarAPI : Attribute
    {
        public CytarAPI(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
