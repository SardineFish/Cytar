using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar.IO;
using Cytar.Network;

namespace Test
{
    public class TestNetworkSession : NetworkSession
    {
        public TestNetworkSession(TestStream inputStream,TestStream outputStream)
        {

            InputStream = new InputStream(inputStream);
            OutputStream = new OutputStream(outputStream);
        }

        public override bool Connected { get => true; protected set{ } }
        public override bool SSID { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override InputStream InputStream { get ; protected set; }
        public override OutputStream OutputStream { get ; protected set; }
        protected override Stream InnerStream { get ; set ; }

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
