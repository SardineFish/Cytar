using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property, Inherited =false, AllowMultiple =false)]
    public class SerializablePropertyAttribute : System.Attribute
    {
        public int Index { get; private set; }
        public SerializablePropertyAttribute(int index)
        {
            Index = index;
        }
    }
}
