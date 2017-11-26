using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using Cytar.Network;

namespace Cytar
{
    public class Cytar
    {
        public CytarTCPServer TCPServer { get; private set; }

        public CytarUDPServer UDPServer { get; private set; }

        public CytarHTTPServer HTTPServer { get; private set; }

        public CytarWebSocketServer WebSocketServer { get; private set; }

        internal Func<NetworkSession, Session> SetupSessionCallback;

        public Cytar()
        {
            
        }

        public void Start()
        {

        }

        public void UseTCP(string host,int port)
        {
            TCPServer = new CytarTCPServer(this, host, port);
        }

        public void UseTCP<T>(string host,int port) where T: CytarTCPServer, new()
        {
            TCPServer = new T();
            TCPServer.Host = host;
            TCPServer.Port = port;
            TCPServer.Cytar = this;
        }

        public void UseUDP(string host,int port)
        {
            UDPServer = new CytarUDPServer(this, host, port);
        }

        public void UseHTTP(string host, int port)
        {
            HTTPServer = new CytarHTTPServer(this, host, port);
        }

        public void UseWebSocket(string host,int port)
        {
            WebSocketServer = new CytarWebSocketServer(this, host, port);
        }

        public void CustomSession(Func<NetworkSession,Session> callback)
        {
            SetupSessionCallback = callback;
        }

        public void CustomSession<SessionT>() where SessionT: Session, new()
        {
            SetupSessionCallback = (netSession) =>
            {
                return Activator.CreateInstance(typeof(SessionT), netSession) as Session;
            };
        }

        private void SetupSession(NetworkSession netSession)
        {
            Session session;
            if (SetupSessionCallback != null)
                session = SetupSessionCallback(netSession);
            
        }

        public Session WaitSession()
        {
            throw new NotImplementedException();
        }

        public void WaitSession(Action<Session> callback)
        {
            throw new NotImplementedException();
        }

        public SessionT WaitSession<SessionT>()
        {
            throw new NotImplementedException();
        }

        public void WaitSession<SessionT>(Action<SessionT> callback)
        {
            throw new NotImplementedException();
        }
    }
    public enum Protocol : byte
    {
        TCP = 1,
        UDP = 2,
        HTTP = 4,
        WebSocket = 8,
    }
}
