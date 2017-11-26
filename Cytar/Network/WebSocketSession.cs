using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cytar.Network
{
    public class WebSocketSession : NetworkSession
    {
        public override bool Connected { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override bool SSID { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
        public override Stream Stream { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

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
