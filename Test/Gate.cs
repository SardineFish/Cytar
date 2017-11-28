using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class Gate: APIContext
    {
        [CytarAPIAttribute("LGN")]
        public void Login(Session session, string username,byte[] pwdHash)
        {
        }

        [CytarAPIAttribute("REG")]
        public void Register(Session session, string username,byte[] pwdHash)
        {

        }
    }
}
