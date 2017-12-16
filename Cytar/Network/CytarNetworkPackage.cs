using System;
using System.Collections.Generic;
using System.Text;
using Force.Crc32;
using System.IO;
using System.Threading;

namespace Cytar.Network
{
    public class CytarNetworkPackage
    {
        internal uint PackSequence = 0;
        internal uint Sequence = 0;

        private uint ack = 0;
        internal uint BeginSequence
        {
            get { return ack; }
            set
            {
                lock (this)
                {
                    var length = (long)value - (long)ack;
                    if (length <= 0)
                        return;
                    var newBuffer = new byte[buffer.Length - length];
                    Array.Copy(buffer, length, newBuffer, 0, buffer.Length - length);
                    buffer = newBuffer;
                    ack = value;

                }
            }
        }

        internal byte[] buffer = new byte [0];
        internal bool Ready = false;
        internal bool Received = false;
        internal long lastAckTime = 0;
        public long ResendTimeOut{ get; set; } = 10;
        
        public bool Lock { get; set; } = false;

        public long Length { get; private set; }

        public long WritePosition { get; set; }

        public long BufferSize{ get; set; } = 655360;

        private AutoResetEvent writeSignal = new AutoResetEvent(false);

        public byte[] Buffer
        {
            get { return buffer; }
            set
            {
                if (Lock)
                    throw new InvalidOperationException("Cannot modify a locked package.");
            }
        }

        public uint CRC32
        {
            get { return Crc32Algorithm.Compute(Buffer); }
        }

        public CytarNetworkPackage():this(new byte[0])
        {
            Length = 0;
        }

        public CytarNetworkPackage(byte[] data)
        {
            buffer = data;
            Length = (uint)data.Length;
            WritePosition = data.Length;
        }

        public CytarNetworkPackage(long length) : this(new byte[0])
        {
            Length = length;
        }

        internal void ResetLength(long length)
        {
            Length = length;
        }

        internal void Ack(uint ack)
        {
            lastAckTime = DateTime.Now.Ticks;
            BeginSequence = ack;
        }

        internal bool AckTimeOut()
        {
            return (DateTime.Now.Ticks - lastAckTime) / 10000 > ResendTimeOut;
        }

        internal int ReadInternal(int seq, int length, byte[] buffer)
        {
            return ReadInternal(seq, length, buffer, 0);
        }

        internal int ReadInternal(int seq, int length, byte[] buffer,int offset)
        {
            lock (this)
            {
                if (seq < BeginSequence)
                    return 0;
                    //throw new IOException("Cannot read abandoned data.");
                length = (int) Math.Min(WritePosition - seq, length);
                Array.Copy(this.buffer, seq - BeginSequence, buffer, offset, length);
                return length;
            }
        }

        public int Read(int seq, int length, byte[] buffer)
        {
            return Read(seq, length, buffer, 0);
        }
        public int Read(int seq, int length, byte[] buffer, int offset)
        {
            lock (this)
            {
                if (seq < BeginSequence)
                    throw new IOException("Cannot read abandoned data.");
                length = (int)Math.Min(WritePosition - seq, length);
                Array.Copy(this.buffer, seq - BeginSequence, buffer, offset, length);
                BeginSequence = (uint)(seq + length);
                return length;
            }
        }

        public byte[] Read()
        {
            var data = buffer;
            BeginSequence = (uint)WritePosition;
            return data;
        }

        public long Write(byte[] data)
        {
            return Write(data, WritePosition, data.Length);
        }

        public long Write(byte[] data, long seq, long length)
        {
            lock (this)
            {
                long srcOffset = 0;
                if (seq < BeginSequence)
                {
                    srcOffset = BeginSequence - seq;
                    length -= (BeginSequence-seq);
                    seq = BeginSequence;
                }
                if (length <= 0)
                    return 0;
                var offset = seq - BeginSequence;
                if (offset + length > BufferSize)
                    length = BufferSize - offset;
                if (offset + length > buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, (int)(offset + length));
                }
                if (offset == buffer.Length)
                    return 0;
                Array.Copy(data, srcOffset, buffer, offset, length);
                WritePosition = seq + length;
                if (WritePosition > Length && !Lock)
                    Length = WritePosition;
                return length;
            }
        }
    }
}
