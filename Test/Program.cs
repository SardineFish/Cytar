using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cytar;
using Cytar.Serialization;
using UnityEngine;
using Cytar.Unity;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            CytarForUnity.Extent();
            var obj = new Foo();
            var buffer = CytarSerialize.Serialize(obj);
            obj = CytarDeserialize.Deserialize<Foo>(buffer);
            var vector2 = CytarDeserialize.Deserialize<Vector4>(CytarSerialize.Serialize(new Vector4(1.1f, 2.2f, .3f, 4.0f)));
        }
    }
}
