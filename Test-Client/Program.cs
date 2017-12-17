﻿using System;
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
            UDPSession session = new UDPSession(new IPEndPoint(Dns.GetHostAddresses("server.dwscdv3.com")[0], 45678));
            session.QosType = CytarUDPQosType.ReliablePackage;
            session.Start();
            session.SendPackage(new CytarNetworkPackage(new byte[] {2, 3, 3, 3, 3, 3, 3, 3}));
            var package = session.ReceivePackage();
            byte[] buffer = new byte[package.Length];
            long length = 0;
            var time = DateTime.Now.Ticks;
            var count = 0;
            while (length < package.Length)
            {
                package.Read();
                /*if (++count % 100 == 0)
                {
                    var speed = (double)package.WritePosition / 1000 / (double)((DateTime.Now.Ticks - time) / 10000000);
                    Console.WriteLine("Speed = {0} KB/s", speed);
                }*/
                //length += package.Read((int)length, (int)package.Length, buffer, (int)length);
            }
        }
    }
}
