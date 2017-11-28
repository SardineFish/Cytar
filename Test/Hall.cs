using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class Hall: APIContext
    {
        [CytarAPIAttribute("GTRM")]
        public string[] GetRooms(Session session)
        {
            throw new NotImplementedException();
        }

        [CytarAPIAttribute("CRRM")]
        public int CreateGameRoom(Session session)
        {
            throw new NotImplementedException();
        }


    }
}
