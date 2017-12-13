using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cytar.IO;
using System.Net.Sockets;

namespace Cytar.Network
{
    public class UDPSession : NetworkSession
    {
        public UDPSession(IPEndPoint iPEndPoint) : this(iPEndPoint, CytarUDPQosType.Unreliable)
        {
        }
        public UDPSession(IPEndPoint iPEndPoint, CytarUDPQosType qosType):this(iPEndPoint,new UdpClient(), qosType)
        {
        }
        public UDPSession(IPEndPoint iPEndPoint,UdpClient udpServer, CytarUDPQosType qosType)
        {
            QosType = qosType;
            UdpClient = udpServer;
            RemoteIPEndPoint = iPEndPoint;
        }

        public CytarUDPQosType QosType { get; set; }
        public UdpClient UdpClient { get; private set; }
        public override bool Connected { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override uint SSID { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override InputStream InputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override OutputStream OutputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override IPAddress RemoteIPAdress => RemoteIPEndPoint.Address;

        public int RemotePort => RemoteIPEndPoint.Port;

        public IPEndPoint RemoteIPEndPoint { get; private set; }

        protected override Stream InnerStream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int idx, int count)
        {
            throw new NotImplementedException();
        }

        public override int ReadByte()
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteByte(byte value)
        {
            throw new NotImplementedException();
        }

        public void OnStart()
        {
            
        }

        public void OnBadData(byte[] data)
        {
            
        }

        public void OnDataReceived(byte[] data)
        {
            
        }
    }
}
