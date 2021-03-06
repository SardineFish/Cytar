﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Force.Crc32;
using System.IO;
using Cytar.IO;
using System.Threading.Tasks;

namespace Cytar.Network
{
    public enum CytarUDPQosType
    {
        Unreliable,
        ReliablePackage,
        ReliableSequenced,
        ReliableStateUpdate,
        AllCostDelivery,
        Stream,
    }
    public class CytarUDPServer: CytarNetworkServer
    {
        public UdpClient UdpClient { get; private set; }

        private CytarUDPQosType qosType = CytarUDPQosType.Unreliable;
        public CytarUDPQosType QosType
        {
            get { return qosType; }
            set
            {
                if (Running)
                    return;
                qosType = value;
            }
        }
        public Dictionary<uint, UDPSession> Sessions { get; private set; } = new Dictionary<uint, UDPSession>();
        public Action<UDPSession> OnSessionSetupCallback = null;
        public override bool Running
        {
            get;
            protected set;
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

        public IPAddress Address
        {
            get
            {
                IPAddress address;
                if (IPAddress.TryParse(Host, out address))
                    return address;
                return Dns.GetHostAddresses(Host)[0];
            }
        }

        int port;

        public override event Action<Exception> OnError;

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

        public IPEndPoint IPEndPoint
        {
            get;private set;
        }

        public override Thread ServerThread { get; protected set; }

        public CytarUDPServer(Cytar Cytar,string host,int port): base(Cytar)
        {
            Host = host;
            Port = port;
        }

        public CytarUDPServer(string host, int port) : base(null)
        {
            Host = host;
            Port = port;
        }

        protected CytarUDPServer(Cytar Cytar):base (Cytar)
        {
            Host = "0.0.0.0";
            Port = 0;
        }

        public override void Start()
        {
            IPEndPoint = new IPEndPoint(IPAddress.Any, Port);
            UdpClient = new UdpClient(IPEndPoint);
            Running = true;
            RunningThread();
            /*
            ServerThread = new Thread(RunningThread);
            ServerThread.Start();*/
        }

        public void RunningThread()
        {
            while (Running)
            {
                IPEndPoint remoteIP = new IPEndPoint(IPEndPoint.Address, Port);
                var data = UdpClient.Receive(ref remoteIP);
                if (data.Length < 12)
                    continue;
                MemoryStream ms = new MemoryStream(data);
                CytarStreamReader cr = new CytarStreamReader(ms);
                var ssid = cr.ReadUInt32();
                // Handle session setup
                if (ssid == 0)
                {
                    ssid = cr.ReadUInt32();
                    // Reset a existed session
                    if (ssid != 0)
                    {
                        if (Sessions.ContainsKey(ssid))
                        {
                            if (Sessions[ssid].RemoteIPAdress == remoteIP.Address)
                            {
                                Sessions[ssid].OnReset(UdpClient, remoteIP);
                            }
                        }
                    }
                    // Start a new session
                    else
                    {
                        ssid = (uint)remoteIP.GetHashCode();
                        UDPSession session = new UDPSession(remoteIP, UdpClient, QosType, ssid);
                        Sessions[ssid] = session;
                        session.StartInternal();
                        Task.Run(() =>
                        {
                            OnSessionSetupCallback?.Invoke(session);
                        });
                        continue;
                    }
                }
                if (!Sessions[ssid].RemoteIPAdress.Equals(remoteIP.Address))
                    continue;
                if(Sessions[ssid].RemotePort != remoteIP.Port)
                    Sessions[ssid].OnReset(UdpClient, remoteIP);
                Sessions[ssid].OnDataReceived(cr.ReadBytes(data.Length - 4));
            }

            foreach (var session in Sessions.Values)
            {
                session.CloseAndWait();
            }
            Sessions = null;
            UdpClient.Dispose();
        }

        public override void Stop()
        {
            Running = false;
            //ServerThread.Abort();
        }
        
    }


}
