using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cytar;
using Cytar.Serialization;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            var obj = new Foo();
            var buffer = CytarSerialize.Serialize(obj);
            obj = CytarDeserialize.Deserialize<Foo>(buffer);
        }
    }
}
