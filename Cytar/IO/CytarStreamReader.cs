using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using Cytar.Serialization;

namespace Cytar.IO
{
    public class CytarStreamReader: IDisposable
    {
        public Stream Stream {get;set;}
        public CytarStreamReader(Stream stream)
        {
            Stream = stream;
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

        public byte ReadByte() => ReadBytes(1)[0];

        public Boolean ReadBoolean() => CytarDeserialize.Deserialize<Boolean>(Stream);

        public Int16 ReadInt16() => CytarDeserialize.Deserialize<Int16>(Stream);

        public UInt16 ReadUInt16() => CytarDeserialize.Deserialize<UInt16>(Stream);

        public Int32 ReadInt32() => CytarDeserialize.Deserialize<Int32>(Stream);

        public UInt32 ReadUInt32() => CytarDeserialize.Deserialize<UInt32>(Stream);

        public Int64 ReadInt64() =>CytarDeserialize.Deserialize<Int64>(Stream);

        public UInt64 ReadUInt64() => CytarDeserialize.Deserialize<UInt64>(Stream);

        public float ReadSingle() => CytarDeserialize.Deserialize<Single>(Stream);

        public double ReadDouble() => CytarDeserialize.Deserialize<double>(Stream);

        public char ReadChar() => CytarDeserialize.Deserialize<char>(Stream);

        public string ReadString() => CytarDeserialize.Deserialize<string>(Stream);

        public Array ReadArray(Type elementType) => (Array)CytarDeserialize.Deserialize(elementType.MakeArrayType(), Stream);

        public T[] ReadArray<T>() => CytarDeserialize.Deserialize<T[]>(Stream);

        public object ReadObject(Type type) => Serialization.CytarDeserialize.Deserialize(type, Stream);

        public T ReadObject<T>() => (T)ReadObject(typeof(T));

        public void Dispose() => ((IDisposable)Stream).Dispose();
    }
}
