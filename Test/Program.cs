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
            CytarUDPServer udp = new CytarUDPServer("127.0.0.1", 45678);
            udp.QosType = CytarUDPQosType.ReliableSequenced;
            udp.Start();

        }
    }
}
