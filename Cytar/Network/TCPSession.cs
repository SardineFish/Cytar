using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Cytar.IO;

namespace Cytar.Network
{
    public class TCPSession : NetworkSession
    {
        public override bool Available
        {
            get
            {
                if (TcpClient == null)
                    return false;
                return TcpClient.Connected;
            }
            protected set { }
        }

        public override uint SSID { get; protected set; }

        public TcpClient TcpClient { get; private set; }
        protected override Stream InnerStream { get; set; }
        public override InputStream InputStream { get ; protected set; }
        public override OutputStream OutputStream { get; protected set; }

        public override IPAddress RemoteIPAdress
        {
            get
            {
                return (TcpClient.Client.RemoteEndPoint as IPEndPoint).Address;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (InputStream)
            {
                return InputStream.Read(buffer, offset, count);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (OutputStream)
            {
                OutputStream.Write(buffer, offset, count);
            }
        }

        public override int ReadByte()
        {
            lock (InputStream)
            {
                return InputStream.ReadByte();
            }
        }

        public override void WriteByte(byte value)
        {
            lock (OutputStream)
            {
                OutputStream.WriteByte(value);
            }
        }

        public override void Close()
        {
            TcpClient.Close();
        }

        public TCPSession(TcpClient client)
        {
            TcpClient = client;
            InnerStream = client.GetStream();
            InputStream = new InputStream(InnerStream);
            OutputStream = new OutputStream(InnerStream);
        }
    }
}
