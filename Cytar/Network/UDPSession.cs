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
        protected SortedDict<uint, CytarNetworkPackage> PackageReceived = new SortedDict<uint, CytarNetworkPackage>();
        private SortedDict<uint, AckData> AckToSend = new SortedDict<uint, AckData>();
        protected uint PackageSendSequence = 0;
        protected uint PackageReceivedSequence = 0;
        public Thread HandleThread { get; private set; }
        public Thread ReceiveThread { get; private set; }
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
                if (PackageReceived.Count > 0)
                {

                    if (QosType == CytarUDPQosType.ReliableSequenced)
                    {
                        lock (PackageReceived)
                        {

                            if (PackageReceived.First.PackSequence == PackageReceivedSequence + 1 && PackageReceived.First.Ready)
                            {
                                package = PackageReceived.First;
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
                                if (PackageReceived.Values[i].Ready)
                                {
                                    package = PackageReceived.Values[i];
                                    PackageReceived.RemoveRange(0, i + 1);
                                    PackageReceivedSequence = package.PackSequence;
                                    break;
                                }
                            }
                        }
                    }
                    else if (QosType == CytarUDPQosType.ReliablePackage)
                    {
                        lock (PackageReceived)
                        {
                            for (var i = PackageReceived.Count - 1; i >= 0; i--)
                            {
                                if (!PackageReceived.Values[i].Received)
                                {
                                    package = PackageReceived.Values[i];
                                    package.Received = true;
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
                                if (PackageReceived.Values[i].Ready)
                                {
                                    package = PackageReceived.Values[i];
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
                }

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


        private void ReceiveFunc()
        {
            while (Available)
            {
                IPEndPoint ip = null;
                var data = UdpClient.Receive(ref ip);
                if (!ip.Address.Equals(RemoteIPAdress))
                    continue;
                MemoryStream ms = new MemoryStream(data);
                CytarStreamReader cr = new CytarStreamReader(ms);
                var ssid = cr.ReadUInt32();
                if (ssid == 0)
                {
                    ssid = cr.ReadUInt32();
                    if (ssid != SSID)
                        continue;
                }
                else if (ssid != SSID)
                    continue;

                OnDataReceived(cr.ReadBytes(data.Length - 4));
            }
        }
        private void SendThread()
        {
            while (Available)
            {
                for (var i = 0; i < PackageToSend.Count; i++)
                {
                    var package = PackageToSend[i];
                    if (package.AckTimeOut())
                        package.Sequence = package.BeginSequence;
                    SendPackageInternal(package);
                    /*
                    if (QosType == CytarUDPQosType.Unreliable)
                    {
                        lock (PackageToSend)
                        {
                            PackageToSend.RemoveAt(i);
                            i--;
                        }
                    }
                    if (QosType == CytarUDPQosType.ReliableSequenced || QosType == CytarUDPQosType.ReliableStateUpdate || QosType == CytarUDPQosType.Unreliable)
                    {
                        lock (package)
                        {
                            int sentLength = (int) (package.Sequence - package.BeginSequence);
                            while (sentLength < package.Buffer.Length)
                            {
                                sentLength += SendData(
                                    packSeq: package.PackSequence,
                                    dataSeq: package.Sequence,
                                    restLength: (uint) (package.Length-package.Sequence),
                                    data: package.buffer,
                                    offset: sentLength,
                                    length: package.Buffer.Length - sentLength);
                                package.Sequence = (uint)(package.BeginSequence + 1 + sentLength);
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
                            while (sentLength <= package.Buffer.Length)
                            {
                                sentLength += SendData(
                                    packSeq: package.PackSequence,
                                    dataSeq: package.Sequence,
                                    restLength: (uint)(package.Length - package.Sequence),
                                    data: package.buffer,
                                    offset: sentLength,
                                    length: package.Buffer.Length - sentLength);
                                package.Sequence = (uint)(package.BeginSequence + 1 + sentLength);
                            }
                        }
                    }*/
                }
                while (AckToSend.Count > 0)
                {
                    SendData(0, 0, 0, new byte[0], 0, 0);
                }
                sendSignal.WaitOne(SendFrequence);
            }

        }

        private void SendPackageInternal(CytarNetworkPackage package)
        {
            lock (package)
            {
                if (QosType == CytarUDPQosType.ReliableSequenced || QosType == CytarUDPQosType.ReliableStateUpdate || QosType == CytarUDPQosType.Unreliable || QosType == CytarUDPQosType.ReliablePackage)
                {
                    var sentLength = 0;

                    byte[] buffer;
                    while (sentLength < package.WritePosition-package.Sequence)
                    {
                        var readLength = UdpClient.Client.ReceiveBufferSize - 64;
                        buffer = new byte[readLength];
                        readLength = package.ReadInternal((int) (package.Sequence + sentLength), readLength,
                            buffer, 0);

                        sentLength += SendData(
                            packSeq: package.PackSequence,
                            dataSeq: package.Sequence,
                            restLength: (uint) (package.Length - package.Sequence),
                            data: buffer,
                            offset: 0,
                            length: readLength);

                        package.Sequence = (uint) (package.Sequence + sentLength);
                    }
                }
                else if (QosType == CytarUDPQosType.AllCostDelivery)
                {
                    var sentLength = 0;
                    package.Sequence = package.BeginSequence;
                    byte[] buffer;
                    while (sentLength < package.WritePosition - package.Sequence)
                    {
                        var readLength = UdpClient.Client.ReceiveBufferSize;
                        buffer = new byte[readLength];
                        readLength = package.ReadInternal((int)(package.Sequence + sentLength), readLength,
                            buffer, 0);

                        sentLength += SendData(
                            packSeq: package.PackSequence,
                            dataSeq: package.Sequence,
                            restLength: (uint)(package.Length - package.Sequence),
                            data: buffer,
                            offset: 0,
                            length: readLength);

                        package.Sequence = (uint)(package.Sequence + sentLength);
                    }
                }
            }
        }
        private int SendData(uint packSeq, uint dataSeq,uint restLength, byte[] data, int offset, int length)
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
                    ack = AckToSend.RemoveAt(0);
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
            return UdpClient.Send(ms.GetBuffer(), (int)ms.Length, RemoteIPEndPoint) - 24;
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
            ReceiveThread = new Thread(ReceiveFunc);
            ReceiveThread.Start();
            PackageSendSequence = 1;
        }

        internal void Handshake()
        {
            /*if (!Available)
                throw new InvalidOperationException("Cannot send handshake on an unavailable session.");*/

            MemoryStream ms = new MemoryStream();
            CytarStreamWriter cw = new CytarStreamWriter(ms);
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
            PackageSendSequence = 1;
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
            if (QosType == CytarUDPQosType.Stream)
            {
                
            }
            else
            {
                MemoryStream ms = new MemoryStream(data);
                CytarStreamReader cr = new CytarStreamReader(ms);
                var packSeq = cr.ReadUInt32();
                var dataSeq = cr.ReadUInt32();
                var restLength = cr.ReadUInt32();
                var packAck = cr.ReadUInt32();
                var dataAck = cr.ReadUInt32();
                var dataRecv = cr.ReadBytes(data.Length - 20);
                //Console.WriteLine("Rceive pack={0}, seq={1}, ackPack={2}, ack={3}", packSeq, dataSeq, packAck, dataAck);
                // Handle ACK
                if (packAck != 0)
                {
                    var packageToAck = PackageToSend.Where(package => package.PackSequence == packAck).FirstOrDefault();
                    if (packageToAck != null)
                    {
                        packageToAck.Ack(dataAck);
                    }
                }
                if (packSeq == 0)
                    return;
                // Check whether package is out of time.
                if (packSeq <= PackageReceivedSequence)
                {
                    if (QosType == CytarUDPQosType.Unreliable)
                        return;
                    AckToSend[packSeq] = new AckData(packSeq, dataSeq + restLength);
                    return;
                }

                if (!PackageReceived.Keys.Contains(packSeq))
                {
                    var package = new CytarNetworkPackage(dataSeq + restLength);
                    package.PackSequence = packSeq;
                    PackageReceived.Add(packSeq, package);
                }
                //Array.Copy(dataRecv, 0, PackageReceived[packSeq].buffer, dataSeq, dataRecv.Length);
                if (dataSeq <= PackageReceived[packSeq].Sequence)
                {
                    var receiveLength = PackageReceived[packSeq].Write(dataRecv,dataSeq,dataRecv.Length);
                    if (receiveLength > 0)
                        PackageReceived[packSeq].Sequence = (uint) (dataSeq + dataRecv.Length);
                    PackageReceived[packSeq].ResetLength(dataSeq + restLength);
                    if (PackageReceived[packSeq].Sequence == PackageReceived[packSeq].Length || restLength == 0)
                        PackageReceived[packSeq].Ready = true;

                }
                lock (AckToSend)
                {
                    AckToSend[packSeq] = new AckData(packSeq, PackageReceived[packSeq].Sequence);
                }
                receiveSignal.Set();
                sendSignal.Set();
            }
        }
    }
}
