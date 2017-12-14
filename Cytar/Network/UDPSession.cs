using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Cytar.IO;
using System.Net.Sockets;
using System.Threading;

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

        public UDPSession(IPEndPoint iPEndPoint, UdpClient udpServer, CytarUDPQosType qosType)
        {
            QosType = qosType;
            UdpClient = udpServer;
            RemoteIPEndPoint = iPEndPoint;
        }
        internal UDPSession(IPEndPoint iPEndPoint,UdpClient udpServer, CytarUDPQosType qosType, uint ssid)
        {
            QosType = qosType;
            UdpClient = udpServer;
            RemoteIPEndPoint = iPEndPoint;
            SSID = ssid;
        }

        /// <summary>
        /// The max sleep time of sending loop in millisecond.
        /// </summary>
        public int SendFrequence { get; set; } = 5;

        public CytarUDPQosType QosType { get; set; }
        public UdpClient UdpClient { get; private set; }
        protected List<CytarNetworkPackage> PackageToSend = new List<CytarNetworkPackage>();
        protected List<CytarNetworkPackage> PackageReceived = new List<CytarNetworkPackage>();
        private SortedDictionary<uint, AckData> AckToSend = new SortedDictionary<uint, AckData>();
        protected uint PackageSendSequence = 0;
        protected uint PackageReceivedSequence = 0;
        public Thread HandleThread { get; private set; }
        protected AutoResetEvent sendSignal = new AutoResetEvent(false);
        protected AutoResetEvent receiveSignal = new AutoResetEvent(false);
        protected AutoResetEvent closeSignal = new AutoResetEvent(false);
        public override bool Available { get; protected set ; }
        public override uint SSID { get; protected set; }
        /*public uint Token { get; protected set; }*/
        public override InputStream InputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override OutputStream OutputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override IPAddress RemoteIPAdress => RemoteIPEndPoint.Address;

        public int RemotePort => RemoteIPEndPoint.Port;

        public IPEndPoint RemoteIPEndPoint { get; private set; }

        protected override Stream InnerStream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Close()
        {
            Available = false;
            //HandleThread.Abort();
        }

        public void CloseAndWait()
        {
            Available = false;
            closeSignal.WaitOne();
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

        public CytarNetworkPackage ReceivePackage()
        {
            CytarNetworkPackage package = null;
            while (Available)
            {
                if (QosType == CytarUDPQosType.ReliableSequenced)
                {
                    lock (PackageReceived)
                    {
                        if (PackageReceived[0].PackSequence == PackageReceivedSequence + 1 && PackageReceived[0].Ready)
                        {
                            package = PackageReceived[0];
                            PackageReceived.RemoveAt(0);
                            PackageReceivedSequence = package.PackSequence;
                        }   
                    }
                }
                else if (QosType == CytarUDPQosType.ReliableStateUpdate)
                {
                    lock (PackageReceived)
                    {
                        for (var i = PackageReceived.Count - 1; i >= 0; i--)
                        {
                            if (PackageReceived[i].Ready)
                            {
                                package = PackageReceived[i];
                                PackageReceived.RemoveRange(0, i + 1);
                                PackageReceivedSequence = package.PackSequence;
                                break;
                            }
                        }
                    }
                }
                else if (QosType == CytarUDPQosType.Unreliable)
                {
                    lock (PackageReceived)
                    {
                        for (var i = 0; i < PackageReceived.Count; i++)
                        {
                            if (PackageReceived[i].Ready)
                            {
                                package = PackageReceived[0];
                                PackageReceived.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                /*else if (QosType == CytarUDPQosType.UnreliablePackage)
                {
                    lock (PackageReceived)
                    {
                        for (var i = 0; i < PackageReceived.Count; i++)
                        {
                            if (PackageReceived[i].Ready)
                            {
                                package = PackageReceived[0];
                                PackageReceived.RemoveAt(i);
                                break;
                            }     
                        }
                    }
                }*/
                if (package != null)
                    return package;
                // Wait for next signal
                receiveSignal.WaitOne();
            }
            throw new InvalidOperationException("Session has been closed.");
        }

        public void SendPackage(CytarNetworkPackage package)
        {
            if (!Available)
                throw new InvalidOperationException("Cannot send package to a unavailable session.");
            lock (PackageToSend)
            {
                package.PackSequence = (uint) (PackageSendSequence + PackageToSend.Count);
                PackageToSend.Add(package);
            }
            sendSignal.Set();
        }

        protected void SendPackageInternal()
        {
            while (Available)
            {
                for (var i = 0; i < PackageToSend.Count; i++)
                {
                    //if(PackageToSend[i].SendSequence)
                }
            }
        }

        private void SendThread()
        {
            while (Available)
            {
                for (var i = 0; i < PackageToSend.Count; i++)
                {
                    var package = PackageToSend[i];
                    if (QosType == CytarUDPQosType.ReliableSequenced || QosType == CytarUDPQosType.ReliableStateUpdate || QosType == CytarUDPQosType.Unreliable)
                    {
                        lock (package)
                        {
                            int sentLength = (int) (package.SendSequence - package.AckSequence);
                            while (sentLength == package.Buffer.Length)
                            {
                                sentLength += SendDate(
                                    packSeq: package.PackSequence,
                                    dataSeq: package.SendSequence,
                                    restLength: (uint) (package.buffer.Length - sentLength),
                                    data: package.buffer,
                                    offset: sentLength,
                                    length: package.Buffer.Length - sentLength);
                                package.SendSequence = (uint)(package.AckSequence + 1 + sentLength);
                            }
                        }
                        if(QosType == CytarUDPQosType.Unreliable)
                        {
                            lock (PackageToSend)
                            {
                                PackageToSend.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    else if (QosType == CytarUDPQosType.AllCostDelivery)
                    {
                        lock (package)
                        {
                            var sentLength = 0;
                            while (sentLength == package.Buffer.Length)
                            {
                                sentLength += SendDate(
                                    packSeq: package.PackSequence,
                                    dataSeq: package.SendSequence,
                                    restLength: (uint)(package.buffer.Length - sentLength),
                                    data: package.buffer,
                                    offset: sentLength,
                                    length: package.Buffer.Length - sentLength);
                                package.SendSequence = (uint)(package.AckSequence + 1 + sentLength);
                            }
                        }
                    }
                }
                sendSignal.WaitOne(SendFrequence);
            }

        }

        private int SendDate(uint packSeq, uint dataSeq,uint restLength, byte[] data, int offset, int length)
        {
            MemoryStream ms = new MemoryStream();
            CytarStreamWriter cw = new CytarStreamWriter(ms);
            cw.Write(SSID);
            cw.Write(packSeq);
            cw.Write(dataSeq);
            cw.Write(restLength);
            if (AckToSend.Count > 0)
            {
                AckData ack;
                lock (AckToSend)
                {
                    var keyValuePair = AckToSend.First();
                    AckToSend.Remove(keyValuePair.Key);
                    ack = keyValuePair.Value;
                }
                cw.Write(ack.PackageSequence);
                cw.Write(ack.DataSequence);
            }
            else
            {
                cw.Write((uint)0);
                cw.Write((uint)0);
            }
            cw.Write(data, offset, length);
            return UdpClient.Send(ms.GetBuffer(), length, RemoteIPEndPoint) - 24;
        }

        public void OnReset(UdpClient udpClient, IPEndPoint iPEndPoint)
        {
            UdpClient = UdpClient;
            RemoteIPEndPoint = iPEndPoint;
            Available = true;
            if (!HandleThread.IsAlive)
            {
                HandleThread = new Thread(SendThread);
                HandleThread.Start();
            }
        }

        public void Start()
        {
            Handshake();
            Available = true;
            HandleThread = new Thread(SendThread);
            HandleThread.Start();
        }

        internal void Handshake()
        {
            /*if (!Available)
                throw new InvalidOperationException("Cannot send handshake on an unavailable session.");*/

            MemoryStream ms = new MemoryStream();
            CytarStreamWriter cw = new CytarStreamWriter(ms);
            cw.Write((uint)0);
            cw.Write((uint)0);
            cw.Write((uint)0);
            Again:
            UdpClient.Send(ms.GetBuffer(), 12, RemoteIPEndPoint);
            IPEndPoint ip = null;
            var data = UdpClient.Receive(ref ip);
            if(!ip.Equals(RemoteIPEndPoint))
                goto Again;
            ms = new MemoryStream(data);
            CytarStreamReader cr = new CytarStreamReader(ms);
            var packseq = cr.ReadUInt32();
            if (packseq != 0)
                throw new UnexpectDataException();
            var ssid = cr.ReadUInt32();
            SSID = cr.ReadUInt32();
        }

        internal void ReplyHandshake()
        {
            MemoryStream ms = new MemoryStream();
            CytarStreamWriter cw = new CytarStreamWriter(ms);
            cw.Write(0);
            cw.Write(SSID);
            UdpClient.Send(ms.GetBuffer(), 8, RemoteIPEndPoint);
        }

        internal void StartInternal()
        {
            ReplyHandshake();
            Available = true;
            HandleThread = new Thread(SendThread);
            HandleThread.Start();
        }

        public void OnBadData(byte[] data)
        {
            if (!Available)
                return;
        }

        public void OnDataReceived(byte[] data)
        {
            if (!Available)
                return;
            MemoryStream ms = new MemoryStream(data);
            CytarStreamReader cr = new CytarStreamReader(ms);
            
        }
    }
}
