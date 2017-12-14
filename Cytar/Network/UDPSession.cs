using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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
        public UDPSession(IPEndPoint iPEndPoint,UdpClient udpServer, CytarUDPQosType qosType)
        {
            QosType = qosType;
            UdpClient = udpServer;
            RemoteIPEndPoint = iPEndPoint;
        }

        public CytarUDPQosType QosType { get; set; }
        public UdpClient UdpClient { get; private set; }
        protected List<CytarNetworkPackage> PackageToSend = new List<CytarNetworkPackage>();
        protected List<CytarNetworkPackage> PackageReceived = new List<CytarNetworkPackage>();
        protected uint PackageSendSequence = 0;
        protected uint PackageReceivedSequence = 0;
        public Thread HandleThread;
        protected AutoResetEvent sendSignal = new AutoResetEvent(false);
        protected AutoResetEvent receiveSignal = new AutoResetEvent(false);
        public override bool Available { get; protected set ; }
        public override uint SSID { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override InputStream InputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override OutputStream OutputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override IPAddress RemoteIPAdress => RemoteIPEndPoint.Address;

        public int RemotePort => RemoteIPEndPoint.Port;

        public IPEndPoint RemoteIPEndPoint { get; private set; }

        protected override Stream InnerStream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Close()
        {
            Available = false;
            HandleThread.Abort();
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
                        if (PackageReceived.Count > 0)
                        {
                            package = PackageReceived[0];
                            PackageReceived.RemoveAt(0);
                        }
                    }
                }
                else if (QosType == CytarUDPQosType.UnreliablePackage)
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

        public void OnStart()
        {
            Available = true;
            HandleThread = new Thread(() =>
            {

            });
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
        }
    }
}
