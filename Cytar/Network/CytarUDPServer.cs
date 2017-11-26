using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cytar.Network
{
    public class CytarUDPServer: CytarNetworkServer
    {
        public UdpClient UdpClient { get; private set; }
        public override bool Running
        {
            get
            {
                if (UdpClient == null || !UdpClient.Client.Connected)
                    return false;
                return true;
            }
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
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
