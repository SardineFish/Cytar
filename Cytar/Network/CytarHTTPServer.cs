using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Cytar.Network
{
    public class CytarHTTPServer: CytarNetworkServer
    {
        public HttpListener HttpListener { get; private set; }
        public override bool Running
        {
            get
            {
                if (HttpListener == null || !HttpListener.IsListening)
                    return false;
                return true;
            }
            protected set { }
        }

        public override event Action<Exception> OnError;
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

        public override Thread ServerThread { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public CytarHTTPServer(Cytar Cytar, string host, int port) : base(Cytar)
        {
            Host = host;
            Port = port;
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

        protected void HandleHttpRequest(HttpListenerContext context)
        {

        }

        public override void Start()
        {
            
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
