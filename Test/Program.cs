using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cytar;
using Cytar.Serialization;
using UnityEngine;
using Cytar.Unity;
using Cytar.Network;

namespace Test
{
    class Program
    {
        public static void Main(string[] args)
        {
            var random = new System.Random();
            CytarUDPServer udp = new CytarUDPServer("127.0.0.1", 45678);
            udp.QosType = CytarUDPQosType.ReliablePackage;
            udp.OnSessionSetupCallback = (session) =>
            {
                var package = session.ReceivePackage();
                Console.WriteLine(package.Buffer);
                package = new CytarNetworkPackage(800000);
                session.SendPackage(package);
                var length = 0;
                while (length < 800000)
                {
                    var buffer = new byte[package.BufferSize];
                    random.NextBytes(buffer);
                    length += (int)package.Write(buffer);
                }
            };
            udp.Start();
        }
    }
}
