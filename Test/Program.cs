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
            TestStream local_remote = new TestStream();
            TestStream remote_local = new TestStream();
            var remoteSession = new Session(new TestNetworkSession(local_remote, remote_local));
            var localSession = new Session(new TestNetworkSession(remote_local, local_remote));

            remoteSession.RootContext = shop;
            remoteSession.Join(shop.FruitsShelf);
            remoteSession.Start();
            localSession.Start();

            localSession.CallRemoteAPI<int>(
                "GetIt",
                (cost) =>
                {
                    bag.Add((int)cost);
                },
                (error) =>
                {
                }, 
                5);
            localSession.CallRemoteAPI<int>(
                "/books/GetIt",
                (cost) =>
                {
                    bag.Add((int)cost);
                },
                (error) =>
                {
                    Console.WriteLine(error.Message);
                }, 100);
            localSession.CallRemoteAPI<int>(
                "/apple/GetIt",
                (cost) =>
                {
                    bag.Add((int)cost);
                },
                (error) =>
                {
                    Console.WriteLine(error.Message);
                }, 100);

            localSession.CallRemoteAPI<int>(
                "EatIt",
                (cost) =>
                {
                    bag.Add((int)cost);
                },
                (error) =>
                {
                    Console.WriteLine(error.Message);
                }, 100);

            //Total Cost
            var money = (int)remoteSession.CallAPI("TTCst", bag.ToArray());
            remoteSession.CallAPI("Pay", money);

        }
    }
}
