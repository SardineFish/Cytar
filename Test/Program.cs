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
            var buffer = CytarSerialize.Serialize(new Foo());
            var obj = CytarDeserialize.Deserialize<Foo>(buffer);
        }
    }
}
