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
        [CytarAPI("GTRM")]
        public string[] GetRooms(Session session)
        {
            throw new NotImplementedException();
        }

        [CytarAPI("CRRM")]
        public int CreateGameRoom(Session session)
        {
            throw new NotImplementedException();
        }


    }
}
