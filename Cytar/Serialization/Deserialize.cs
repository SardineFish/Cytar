using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar.Serialization
{
    public static class Deserialize
    {
        public static object DeserializeFromBytes(Type type, byte[] data)
        {
            throw new NotImplementedException();
        }
        public static T DeserializeFromBytes<T>(byte[] data)
        {
            return (T)DeserializeFromBytes(typeof(T), data);
        }
    }
}
