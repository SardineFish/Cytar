using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited =false, AllowMultiple =false)]
    sealed class Serialize : Attribute
    {
        // This is a positional argument
        public Serialize()
        {
        }
    }
}
