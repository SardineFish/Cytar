using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Cytar.IO;

namespace Cytar.Network
{
    public class HTTPSession : NetworkSession
    {
        public override bool Available { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override uint SSID { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override InputStream InputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override OutputStream OutputStream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        public override IPAddress RemoteIPAdress => throw new NotImplementedException();

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
    }
}
