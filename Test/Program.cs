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
        public static void Main(string[] args)
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
            

            var shop = new Shop();
            var bag = new List<int>();

            Cytar.Cytar cytar = new Cytar.Cytar();
            cytar.UseTCP("127.0.0.1", 36514);
            cytar.Start();
            cytar.WaitSession((session) =>
            {
                session.RootContext = shop;
                session.Join(shop.FruitsShelf);
            });
            CytarClient client = new CytarClient(Protocol.TCP, "127.0.0.1", 36514);
            client.Connect();
            Task.Run(async () =>
            {
                bag.Add(await client.Session.CallRemoteAPIAsync<int>("GetIt", 5));
                bag.Add(await client.Session.CallRemoteAPIAsync<int>("/books/GetIt", 100));
                Console.WriteLine(await client.Session.CallRemoteAPIAsync<int>("TTCst", bag.ToArray()));
                try
                {
                    await client.Session.CallRemoteAPIAsync<int>("/apple/GetIt", 1000);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                await client.Session.CallRemoteAPIAsync("Pay", 1000);
            }).Wait();
            
            client.Session.CallRemoteAPI<int>(
                "GetIt",
                (cost) =>
                {
                    bag.Add((int)cost);
                    Console.WriteLine(cost);
                    client.Session.CallRemoteAPI<int>(
                        "/books/GetIt",
                        (bookCost) =>
                        {
                            bag.Add((int)bookCost);
                            Console.WriteLine(bookCost);
                            client.Session.CallRemoteAPI<int>("TTCst",
                                (totalCost) =>
                                {
                                    Console.WriteLine(totalCost);
                                },
                                (error) =>
                                {
                                    Console.WriteLine(error.Message);
                                },
                                bag.ToArray());
                        },
                        (error) =>
                        {
                            Console.WriteLine(error.Message);
                        }, 100);
                },
                (error) =>
                {
                }, 
                5);
            client.Session.CallRemoteAPI<int>(
                "/apple/GetIt",
                (cost) =>
                {
                    bag.Add((int)cost);

                },
                (error) =>
                {
                    Console.WriteLine(error.Message);
                }, 100);

        }
    }
}
