using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Cytar.Serialization
{
    public static class CytarDeserialize
    {
        public static object Deserialize(Type type,byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (CytarStreamReader cr = new CytarStreamReader(ms))
            {
                return cr.ReadObject(type);
            }
        }
        public static T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(typeof(T), data);
        }
    }
}
