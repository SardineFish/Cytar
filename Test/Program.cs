using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var serverRoot = new ServerRoot();
            var gate = new Gate();
            var hall = new Hall();

            Cytar.Cytar Cytar = new Cytar.Cytar();
            //Cytar.UseTCP("127.0.0.1", 36514);
            /*Cytar.WaitSession((session) =>
            {
                session.Join(gate);
            });*/

        }
    }
}
