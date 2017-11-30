using System;
using System.Collections.Generic;
using System.Text;
using Cytar.Network;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Cytar
{
    public class CytarClient
    {
        public CytarClient(Protocol protocol, string host, int port)
        {
            Protocol = protocol;
            Host = host;
            Port = port;
        }

        public NetworkSession NetworkSession { get; private set; }
        public Session Session { get; private set; }

        public Protocol Protocol { get; private set; }
        public string Host { get;}
        public int Port { get; }

        public Session Connect()
        {
            return Connect<Session>();
        }

        public SessionT Connect<SessionT>() where SessionT : Session, new()
        {
            if (Protocol == Protocol.TCP)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(Host, Port);
                NetworkSession = new TCPSession(tcpClient);
                Session = Activator.CreateInstance(typeof(SessionT), NetworkSession) as Session;
                Session.Start();
                return Session as SessionT;
            }
            else
                throw new NotImplementedException();
        }
        public void Close(int code)
        {
            if (Protocol == Protocol.TCP)
            {
                Session.Close(0);
            }
        }
        public void Close()
        {
            Close(0);
        }
    }
}
