using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyRoute;

namespace Test
{
    [Routable]
    public class RootClass:RoutableObject
    {
        [Routable]
        public class SubClass : RoutableObject
        {
            [Routable("number")]
            public int Number { get; set; }

            [Routable("output")]
            public void Output(string text)
            {
                Console.WriteLine(text);
            }
        }

        [Routable("sub")]
        public SubClass SubObject = new SubClass();

        [Routable("rootFunc")]
        public void RootFunction()
        {
            SubObject.Call("output", "RootFunction");
        }
    }
}
