using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Cytar.Serialization
{
    public static class Deserialize
    {
        public static object DeserializeFromBytes(Type type, byte[] data)
        {
            throw new NotImplementedException();
            if (data.Length < 0)
                throw new ArgumentException("No data.");
            if (type == typeof(byte))
                return data[0];
            else if (type == typeof(UInt16))
                return (UInt16)CytarConvert.ToInt16(data);
            else if (type == typeof(Int16))
                return CytarConvert.ToInt16(data);
            else if (type == typeof(UInt32))
                return (UInt32)CytarConvert.ToInt32(data);
            else if (type == typeof(Int32))
                return CytarConvert.ToInt32(data);
            else if (type == typeof(UInt64))
                return (UInt64)CytarConvert.ToInt64(data);
            else if (type == typeof(Int64))
                return CytarConvert.ToInt64(data);
            else if (type == typeof(Single))
                return CytarConvert.ToSingle(data);
            else if (type == typeof(Double))
                return CytarConvert.ToDouble(data);
            else if (type == typeof(char))
            {
                var length = CytarConvert.ToByte(data.Take(1).ToArray());
                if (length != data.Length - 1)
                    throw new DataSizeException(data, length + 1);
                var decode = Encoding.UTF8.GetString(data.SubBytes(1));
                if (decode.Length <= 0)
                    return '\0';
                return Encoding.UTF8.GetString(data)[0];
            }
            else if (type == typeof(string))
            {
                var length = CytarConvert.ToInt32(data.Take(4).ToArray());
                return Encoding.UTF8.GetString(data.SubBytes(4));
            }
            else if (type.IsArray)
            {
                var size = CytarConvert.ToInt32(data.Take(4).ToArray());
                var count = CytarConvert.ToInt32(data.SubBytes(4, 4));
                var elementType = type.GetElementType();
                var arr = Array.CreateInstance(elementType, count);

            }
        }
        public static object DeserializeFromByte(Type type,byte[] data,int idx,out int length)
        {
            throw new NotImplementedException();
        }

        public static T DeserializeFromBytes<T>(byte[] data)
        {
            return (T)DeserializeFromBytes(typeof(T), data);
        }

        static byte[] Combine(params object[] data)
        {
            int length = 0;

            foreach (object slice in data)
            {
                if (!(slice is byte[]))
                    throw new ArgumentException("Type error.");
                length += (slice as byte[]).Length;
            }
            byte[] buffer = new byte[length];
            var idx = 0;
            for (var i = 0; i < data.Length; i++)
            {
                for (var j = 0; j < (data[i] as byte[]).Length; j++)
                {
                    buffer[idx++] = (data[i] as byte[])[j];
                }
            }
            return buffer;
        }
        static byte[] Combine(byte[][] data)
        {
            int length = 0;
            foreach (byte[] slice in data)
                length += slice.Length;
            byte[] buffer = new byte[length];
            var idx = 0;
            for (var i = 0; i < data.Length; i++)
            {
                data[i].CopyTo(buffer, idx);
                idx += data[i].Length;
            }
            return buffer;
        }
        static byte[] SubBytes(this byte[] data,int start,int count)
        {
            if (start + count > data.Length)
                throw new IndexOutOfRangeException();
            byte[] sub = new byte[count];
            for(var i = start; i < start + count; i++)
            {
                sub[i - start] = data[i]; 
            }
            return sub;
        }
        static byte[] SubBytes(this byte[] data, int start)
        {
            return SubBytes(data, start, data.Length - start);
        }
    }
}
