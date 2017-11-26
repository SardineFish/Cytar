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

            var shop = new Shop();
            var bag = new List<int>();
            bag.Add((int)shop.BooksShelf.CallAPI("GetIt", 5));
            bag.Add((int)shop.FruitsShelf.CallAPI("GetIt", "2333"));
            //Total Cost
            var money = (int)shop.CallAPI("TTCst", bag.ToArray());
            shop.CallAPI("Pay", money);

        }
    }
}
