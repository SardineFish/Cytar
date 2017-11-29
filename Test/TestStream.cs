using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Test
{
    public class TestStream : Stream
    {
        public MemoryStream ms = new MemoryStream();
        public long ReadPosition = 0;
        public long WritePosition = 0;
        public AutoResetEvent waiting = new AutoResetEvent(false);

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => ms.Length;

        public override long Position { get => ms.Position; set
            {
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            while(count + ReadPosition >WritePosition)
            {
                waiting.WaitOne();
            }
            int length = 0;
            lock (ms)
            {
                ms.Position = ReadPosition;
                length = ms.Read(buffer, offset, count);
                ReadPosition = ms.Position;
            }
            return length;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return 0;
        }

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock(ms)
            {
                ms.Position = WritePosition;
                ms.Write(buffer, offset, count);
                WritePosition = ms.Position;
            }
            waiting.Set();
        }
    }
}
