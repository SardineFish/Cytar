using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cytar.Network
{
    public class CytarTCPServer: CytarNetworkServer
    {
        public override bool Running
        {
            get
            {
                if (TcpListener == null || !TcpListener.Server.Connected)
                    return false;
                return true;
            }
        }
        
        public TcpListener TcpListener { get; private set; }

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

        public override Thread ServerThread { get; protected set; }

        public override event Action<Exception> OnError;

        public CytarTCPServer(Cytar Cytar, string host, int port):base(Cytar)
        {
            Host = host;
            Port = port;
        }

        protected CytarTCPServer(Cytar Cytar):base(Cytar)
        {
            Host = "0.0.0.0";
            Port = 0;
        }

        public override void Start()
        {
            ServerThread = new Thread(threadFunction);
            ServerThread.Start();
        }

        public void threadFunction()
        {
            try
            {   
                var addr = Dns.GetHostAddresses(Host);
                if (addr.Length < 1)
                    throw new ArgumentException("Unknown host.");

                TcpListener = new TcpListener(addr[0], Port);
                TcpListener.Start();
                while (Running)
                {
                    var client = TcpListener.AcceptTcpClient();
                }
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError.Invoke(ex);
            }
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
