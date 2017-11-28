using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cytar;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Bill bill = new Bill("Billlll");
            var root = new RootClass();
            root.Call("sub/output", "Hello world!");
            root.Call("rootFunc");
            var data = Cytar.Serialization.CytarSerialize.Serialize(bill);
            Cytar.Serialization.CytarDeserialize.Deserialize<Bill>(data);
            var serverRoot = new ServerRoot();
            var gate = new Gate();
            var hall = new Hall();

            Cytar.Cytar cytar = new Cytar.Cytar();

            var shop = new Shop();
            var bag = new List<int>();
            var man = new Session();
            man.RootContext = shop;
            bag.Add((int)man.CallPathAPI("/fruit/GetIt", 10));
            man.Join(shop.FruitsShelf);
            bag.Add((int)man.CallAPI("GetIt", 5));
            man.Join(shop.MeatShelf);
            bag.Add((int)man.CallAPI("GetIt", 5));
            //Total Cost
            var money = (int)man.CallAPI("TTCst", bag.ToArray());
            man.CallAPI("Pay", money);

        }
    }
}
