using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Cytar.Network
{
    public class TCPSession : NetworkSession
    {
        public override bool Connected
        {
            get
            {
                if (TcpClient == null)
                    return false;
                return TcpClient.Connected;
            }
            protected set { }
        }

        public override bool SSID { get; protected set; }
        
        public override Stream Stream
        {
            get
            {
                if (TcpClient == null)
                    return null;
                return TcpClient.GetStream();
            }
            protected set { }
        }

        public TcpClient TcpClient { get; private set; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return Stream.ReadByte();
        }

        public override void WriteByte(byte value)
        {
            Stream.WriteByte(value);
        }

        public TCPSession(TcpClient client)
        {
            TcpClient = client;
        }
    }
}
