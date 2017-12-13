using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Cytar.IO;
using System.Reflection;

namespace Cytar.Serialization
{
    public static class CytarDeserialize
    {
        private static Dictionary<Type, Func<Stream, object>> ExtendedDeserialization = new Dictionary<Type, Func<Stream, object>>();
        public static object Deserialize(Type type,byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (CytarStreamReader cr = new CytarStreamReader(ms))
            {
                return cr.ReadObject(type);
            }
        }
        public static T Deserialize<T>(byte[] data)
        {
            return (T)Deserialize(typeof(T), data);
        }
        public static object Deserialize(Type type,Stream stream)
        {
            if (ExtendedDeserialization.ContainsKey(type))
                return ExtendedDeserialization[type].Invoke(stream);
            if (type == typeof(byte))
                return ReadBytes(stream, 1)[0];
            if (type == typeof(Boolean))
                return CytarConvert.ToBoolean(ReadBytes(stream, 1));
            else if (type == typeof(UInt16))
                return CytarConvert.ToUInt16(ReadBytes(stream, 2));
            else if (type == typeof(Int16))
                return CytarConvert.ToInt16(ReadBytes(stream, 2));
            else if (type == typeof(UInt32))
                return CytarConvert.ToUInt32(ReadBytes(stream, 4));
            else if (type == typeof(Int32))
                return CytarConvert.ToInt32(ReadBytes(stream, 4));
            else if (type == typeof(UInt64))
                return CytarConvert.ToUInt64(ReadBytes(stream, 8));
            else if (type == typeof(Int64))
                return CytarConvert.ToInt64(ReadBytes(stream, 8));
            else if (type == typeof(Single))
                return CytarConvert.ToSingle(ReadBytes(stream, 4));
            else if (type == typeof(Double))
                return CytarConvert.ToDouble(ReadBytes(stream, 8));
            else if (type == typeof(char))
            {
                byte[] buffer = new byte[4];
                buffer[0] = ReadBytes(stream, 1)[0];
                if ((buffer[0] & 0b10000000) == 0)
                {
                    return Encoding.UTF8.GetString(new byte[1] { buffer[0] })[0];
                }
                else if ((buffer[0] & 0b11100000) == 0b11000000)
                {
                    buffer[1] = ReadBytes(stream, 1)[0];
                    return Encoding.UTF8.GetString(new byte[2] { buffer[0], buffer[1] })[0];
                }
                else if ((buffer[0] & 0b11110000) == 0b11100000)
                {
                    buffer[1] = ReadBytes(stream, 1)[0];
                    buffer[2] = ReadBytes(stream, 1)[0];
                    return Encoding.UTF8.GetString(new byte[3] { buffer[0], buffer[1], buffer[2] })[0];
                }
                else if ((buffer[0] & 0b11111000) == 0b11110000)
                {
                    buffer[1] = ReadBytes(stream, 1)[0];
                    buffer[2] = ReadBytes(stream, 1)[0];
                    buffer[3] = ReadBytes(stream, 1)[0];
                    return Encoding.UTF8.GetString(buffer)[0];
                }
                else
                    return Encoding.UTF8.GetString(new byte[1] { buffer[0] })[0];
            }
            else if (type == typeof(string))
            {
                var size = Deserialize<int>(stream);
                if (size < 0)
                    return null;
                return Encoding.UTF8.GetString(ReadBytes(stream, size));
            }
            else if (type.IsArray)
            {
                var length = Deserialize<int>(stream);
                if (length < 0)
                    return null;
                var elementType = type.GetElementType();
                var array = Array.CreateInstance(elementType, length);
                for (var i = 0; i < length; i++)
                    array.SetValue(Deserialize(elementType, stream), i);
                return array;
            }

            if (ReadByte(stream) == 0)
                return null;
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
                    (mb as FieldInfo).SetValue(obj, Deserialize((mb as FieldInfo).FieldType, stream));
                else if (mb.MemberType == MemberTypes.Property)
                    (mb as PropertyInfo).SetValue(obj, Deserialize((mb as PropertyInfo).PropertyType, stream));
                else
                    throw new DeserializeException("Type error.");
            }
            return obj;
        }
        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(typeof(T), stream);
        }
        private static  byte[] ReadBytes(Stream stream,int length)
        {
            byte[] buffer = new byte[length];
            var left = length;
            while (left > 0)
            {
                var count = stream.Read(buffer, length - left, left);
                if (count <= 0)
                    throw new EndOfStreamException();
                left -= count;
            }
            return buffer;
        }
        private static byte ReadByte(Stream stream) => ReadBytes(stream, 1)[0];
        public static void ExtendDeserialize(Type type,Func<Stream,object> deserializeCallback)
        {
            ExtendedDeserialization[type] = deserializeCallback;
        }

    }
}
