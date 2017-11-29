using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cytar.IO
{
    public class OutputStream: Stream
    {
        public OutputStream(Stream innerStream)
        {
            this.innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        }

        private Stream innerStream { get; set; }
        public override bool CanRead => false;

        public override bool CanSeek => innerStream.CanSeek;

        public override bool CanWrite => false;

        public override long Length => innerStream.Length;

        public override long Position
        {
            get => innerStream.Position;
            set => innerStream.Position = value;
        }

        public override void Flush() => innerStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) => throw new IOException("WriteOnly");

        public override long Seek(long offset, SeekOrigin origin) => innerStream.Seek(offset, origin);

        public override void SetLength(long value) => innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => innerStream.Write(buffer, offset, count);
    }
}
