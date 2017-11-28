using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cytar.Exceptions
{
    public class CytarStreamReader
    {
        public Stream Stream {get;set;}
        public CytarStreamReader(Stream stream)
        {
            Stream = stream;
        }

        public byte ReadByte()
        {
            return (byte)Stream.ReadByte();
        }

        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            var left = length;
            while (left > 0)
            {
                var count = Stream.Read(buffer, length - left, left);
                if (count <= 0)
                    throw new EndOfStreamException();
                left -= count;
            }
            return buffer;
        }

        public Int16 ReadInt16()
        {
            return CytarConvert.BytesToInt16(ReadBytes(2));
        }

        public UInt16 ReadUInt16()
        {
            return (UInt16)CytarConvert.BytesToInt16(ReadBytes(2));
        }

        public Int32 ReadInt32()
        {
            return CytarConvert.BytesToInt32(ReadBytes(4));
        }

        public UInt32 ReadUInt32()
        {
            return (UInt32)CytarConvert.BytesToInt32(ReadBytes(4));
        }

        public Int64 ReadInt64()
        {
            return CytarConvert.BytesToInt64(ReadBytes(4));
        }

        public UInt64 ReadUInt64()
        {
            return (UInt64)CytarConvert.BytesToInt64(ReadBytes(2));
        }

        public char ReadChar()
        {
            byte[] buffer = new byte[4];
            buffer[0] = (byte)Stream.ReadByte();
            if((buffer[0]& 0b10000000) == 0)
            {
                return Encoding.UTF8.GetString(new byte[1] { buffer[0] })[0];
            }
            else if ((buffer[0] & 0b11100000) == 0b11000000)
            {
                buffer[1] = (byte)Stream.ReadByte();
                return Encoding.UTF8.GetString(new byte[2] { buffer[0], buffer[1] })[0];
            }
            else if((buffer[0] & 0b11110000) == 0b11100000)
            {
                buffer[1] = (byte)Stream.ReadByte();
                buffer[2] = (byte)Stream.ReadByte();
                return Encoding.UTF8.GetString(new byte[3] { buffer[0], buffer[1], buffer[2] })[0];
            }
            else if((buffer[0] & 0b11111000) == 0b11110000)
            {
                buffer[1] = (byte)Stream.ReadByte();
                buffer[2] = (byte)Stream.ReadByte();
                buffer[3] = (byte)Stream.ReadByte();
                return Encoding.UTF8.GetString(buffer)[0];
            }
            else
                return Encoding.UTF8.GetString(new byte[1] { buffer[0] })[0];
        }

        public string ReadString()
        {
            var size = CytarConvert.BytesToInt32(ReadBytes(4));
            return Encoding.UTF8.GetString(ReadBytes(size));
        }

        public object ReadObject(Type type)
        {
            throw new NotImplementedException();
        }

        public T ReadObject<T>()
        {
            throw new NotImplementedException();
        }
    }
}
