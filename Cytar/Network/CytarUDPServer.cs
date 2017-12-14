using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Force.Crc32;
using System.IO;
using Cytar.IO;

namespace Cytar.Network
{
    public enum CytarUDPQosType
    {
        Unreliable,
        UnreliablePackage,
        ReliableSequenced,
        ReliableStateUpdate,
        AllCostDelivery
    }
    public class CytarUDPServer: CytarNetworkServer
    {
        public UdpClient UdpClient { get; private set; }

        private CytarUDPQosType qosType = CytarUDPQosType.Unreliable;
        public CytarUDPQosType QosType
        {
            get { return QosType; }
            set
            {
                if (Running)
                    return;
                qosType = value;
            }
        }

        public Dictionary<IPEndPoint, UDPSession> Sessions { get; private set; } = new Dictionary<IPEndPoint, UDPSession>();
        public override bool Running
        {
            get;
            protected set;
        }
        string host;
        public string Host
        {
            get { return host; }
            set
            {
                if (!Running)
                    host = value;
                else
                    throw new Exception("Invalid action.");
            }
        }

        public IPAddress Address
        {
            get
            {
                IPAddress address;
                if (IPAddress.TryParse(Host, out address))
                    return address;
                return Dns.GetHostAddresses(Host)[0];
            }
        }

        int port;

        public override event Action<Exception> OnError;

        public int Port
        {
            get { return port; }
            set
            {
                if (!Running)
                    port = value;
                else
                    throw new Exception("Invalid action.");
            }
        }

        public IPEndPoint IPEndPoint
        {
            get;private set;
        }

        public override Thread ServerThread { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public CytarUDPServer(Cytar Cytar,string host,int port): base(Cytar)
        {
            Host = host;
            Port = port;
        }

        protected CytarUDPServer(Cytar Cytar):base (Cytar)
        {
            Host = "0.0.0.0";
            Port = 0;
        }

        public override void Start()
        {
            IPEndPoint = new IPEndPoint(IPAddress.Any, Port);
            UdpClient = new UdpClient(IPEndPoint);
            Running = true;
            ServerThread = new Thread(RunningThread);
            ServerThread.Start();
        }

        private void RunningThread()
        {
            while (Running)
            {
                IPEndPoint remoteIP = new IPEndPoint(IPEndPoint.Address, Port);
                var data = UdpClient.Receive(ref remoteIP);
                if (data.Length < 12)
                    continue;
                MemoryStream ms = new MemoryStream(data);
                CytarStreamReader cr = new CytarStreamReader(ms);
                var handshake = cr.ReadUInt32();
                if (handshake == 0 )
                {
                    UDPSession session = new UDPSession(remoteIP, UdpClient, QosType);
                    Sessions[remoteIP] = session;
                    session.OnStart();
                    continue;
                }
                if (!Sessions.ContainsKey(remoteIP))
                    continue;
                Sessions[remoteIP].OnDataReceived(data);
            }
        }

        public override void Stop()
        {
            Running = false;
            ServerThread.Abort();
            UdpClient.Dispose();
        }
    }


}
