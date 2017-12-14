using System;
using System.Collections.Generic;
using System.Text;
using Force.Crc32;

namespace Cytar.Network
{
    public class CytarNetworkPackage
    {
        internal uint PackSequence = 0;
        internal uint Sequence = 0;

        private uint ack = 0;
        internal uint AckSequence
        {
            get { return ack; }
            set
            {
                var length = value - ack;
                if (length <= 0)
                    return;
                var newBuffer = new byte[buffer.Length - length];
                Array.Copy(buffer, length, newBuffer, 0, buffer.Length - length);
                buffer = newBuffer;
            }
        }

        internal byte[] buffer = new byte [0];
        internal bool Ready = false;
        public bool Lock { get; set; } = false;

        public uint Length => (uint)buffer.LongLength;

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
        }

        public CytarNetworkPackage(byte[] data)
        {
            buffer = data;
        }

        public CytarNetworkPackage(long length) : this(new byte[length])
        {
            
        }
    }
}
