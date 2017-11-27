using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar.Attribute
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    sealed class SubContext : NameAttribute
    {
        public SubContext(string name) : base(name)
        {
        }
    }
}
