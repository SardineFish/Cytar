using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class APITest: APIContext
    {
        [CytarAPIAttribute("Foo")]
        public int Foo(int x)
        {
            return -x;
        }
    }
}
