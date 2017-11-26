using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class Shelf: APIContext
    {
        [CytarAPI("GetIt")]
        public int TakeIt(int x)
        {
            return x * 10;
        }

    }
}
