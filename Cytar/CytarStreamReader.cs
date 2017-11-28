using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Cytar
{
    public class CytarStreamReader
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

        public Int16 ReadInt16() => CytarConvert.ToInt16(ReadBytes(2));

        public UInt16 ReadUInt16() => CytarConvert.ToUInt16(ReadBytes(2));

        public Int32 ReadInt32() => CytarConvert.ToInt32(ReadBytes(4));

        public UInt32 ReadUInt32() => CytarConvert.ToUInt32(ReadBytes(4));

        public Int64 ReadInt64() => CytarConvert.ToInt64(ReadBytes(4));

        public UInt64 ReadUInt64() => CytarConvert.ToUInt64(ReadBytes(2));

        public float ReadSingle() => CytarConvert.ToSingle(ReadBytes(4));

        public double ReadDouble() => CytarConvert.ToDouble(ReadBytes(8));

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
            var size = ReadInt32();
            return Encoding.UTF8.GetString(ReadBytes(size));
        }

        public Array ReadArray(Type elementType)
        {
            var count = ReadInt32();
            var array = Array.CreateInstance(elementType, count);
            for(var i = 0; i < count; i++)
                array.SetValue(ReadObject(elementType), i);
            return array;
        }

        public T[] ReadArray<T>()
        {
            return (T[])ReadArray(typeof(T));
        }

        public object ReadObject(Type type)
        {

            if (type == typeof(byte))
                return ReadByte();
            else if (type == typeof(UInt16))
                return ReadUInt16();
            else if (type == typeof(Int16))
                return ReadInt16();
            else if (type == typeof(UInt32))
                return ReadUInt32();
            else if (type == typeof(Int32))
                return ReadInt32();
            else if (type == typeof(UInt64))
                return ReadUInt64();
            else if (type == typeof(Int64))
                return ReadInt64();
            else if (type == typeof(Single))
                return ReadSingle();
            else if (type == typeof(Double))
                return ReadDouble();
            else if (type == typeof(char))
                return ReadChar();
            else if (type == typeof(string))
                return ReadString();
            else if (type.IsArray)
                return ReadArray(type.GetElementType());

            var members = type.GetMembers().Where(
                       member => member.GetCustomAttributes(true).Where(
                           attr => attr is SerializablePropertyAttribute).FirstOrDefault() != null)
                           .OrderBy(
                       member => (member.GetCustomAttributes(true).Where(
                           attr => attr is SerializablePropertyAttribute).FirstOrDefault() as SerializablePropertyAttribute).Index).ToArray();
            var obj = Activator.CreateInstance(type);
            foreach (var mb in members)
            {
                if (mb.MemberType == MemberTypes.Field)
                    (mb as FieldInfo).SetValue(obj, ReadObject((mb as FieldInfo).FieldType));
                else if (mb.MemberType == MemberTypes.Property)
                    (mb as PropertyInfo).SetValue(obj, ReadObject((mb as PropertyInfo).PropertyType));
                else
                    throw new DeserializeException("Type error.");
            }
            return obj;
        }

        public T ReadObject<T>()
        {
            return (T)ReadObject(typeof(T));
        }
    }
}
