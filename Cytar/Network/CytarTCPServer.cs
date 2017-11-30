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
        public override bool Running { get; protected set; }
        
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

        public CytarTCPServer(Cytar cytar, string host, int port):base(cytar)
        {
            Host = host;
            Port = port;
            Running = false;
        }

        protected CytarTCPServer(Cytar cytar) : this(cytar, "0.0.0.0", 0)
        {
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
                Running = true;
                while (true)
                {
                    try
                    {
                        var client = TcpListener.AcceptTcpClient();
                        Cytar.SetupSession(new TCPSession(client));
                    }
                    catch (SocketException)
                    {
                        Running = false;
                        return;
                    }
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
            Running = false;
            TcpListener.Stop();
        }
    }
}
