using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace Cytar.Network
{
    public class CytarWebSocketServer: CytarNetworkServer
    {
        public CytarWebSocketServer( Cytar Cytar,string host, int port):base(Cytar)
        {
            Host = host;
            Port = port;
        }

        public CytarWebSocketServer(Cytar Cytar) : base(Cytar)
        {

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
        public HttpListener HttpListener { get; protected set; }
        public override bool Running
        {
            get
            {
                if (HttpListener == null || !HttpListener.IsListening)
                    return false;
                return true;
            }
        }

        public override Thread ServerThread { get; protected set; }

        public override event Action<Exception> OnError;

        public override void Start()
        {

            ServerThread = new System.Threading.Thread(threadStart);
            ServerThread.Start();
        }

        async void threadStart()
        {
            HttpListener = new HttpListener();
            HttpListener.Start();
            while (Running)
            {
                var context = await HttpListener.GetContextAsync();
                HandleHttpRequest(context);
            }
        }

        private void HandleHttpRequest(HttpListenerContext context)
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
