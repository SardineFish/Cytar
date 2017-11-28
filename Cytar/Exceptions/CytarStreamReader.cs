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

        public Int16 ReadInt16()
        {
            byte[] buffer = new byte[2];
            Stream.Read(buffer, 0, 2);
            return CytarConvert.BytesToInt16(buffer);
        }

        public UInt16 ReadUInt16()
        {
            byte[] buffer = new byte[2];
            Stream.Read(buffer, 0, 2);
            return (UInt16)CytarConvert.BytesToInt16(buffer);
        }

        public Int32 ReadInt32()
        {
            byte[] buffer = new byte[4];
            Stream.Read(buffer, 0, 4);
            return CytarConvert.BytesToInt32(buffer);
        }

        public UInt32 ReadUInt32()
        {
            byte[] buffer = new byte[4];
            Stream.Read(buffer, 0, 4);
            return (UInt32)CytarConvert.BytesToInt32(buffer);
        }

        public Int64 ReadInt64()
        {
            byte[] buffer = new byte[8];
            Stream.Read(buffer, 0, 8);
            return CytarConvert.BytesToInt64(buffer);
        }

        public UInt64 ReadUInt64()
        {
            byte[] buffer = new byte[8];
            Stream.Read(buffer, 0, 8);
            return (UInt64)CytarConvert.BytesToInt64(buffer);
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

        }
    }
}
