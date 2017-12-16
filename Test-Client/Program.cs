using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;
using Cytar.Network;
using Cytar.IO;
using Cytar.Serialization;
using EasyRoute;
using System.Net;
using System.Net.Sockets;

namespace Test_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPSession session = new UDPSession(new IPEndPoint(IPAddress.Loopback, 45678));
            session.QosType = CytarUDPQosType.ReliablePackage;
            session.Start();
            session.SendPackage(new CytarNetworkPackage(new byte[] {2, 3, 3, 3, 3, 3, 3, 3}));
            var package = session.ReceivePackage();
            byte[] buffer = new byte[package.Length];
            long length = 0;
            while (length < package.Length)
            {
                length += package.Read((int)length, (int)package.Length, buffer, (int)length);
            }
        }
    }
}
