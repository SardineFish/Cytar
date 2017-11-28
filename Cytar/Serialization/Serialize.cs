﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Cytar.Serialization
{
    public static class Serialize
    {
        public static byte[] SerializeToBytes(object obj)
        {
            if (obj is byte)
                return new byte[] { (byte)obj };
            else if (obj is Int16 || obj is UInt16)
                return CytarConvert.ToBytes(Convert.ToUInt16(obj));
            else if (obj is Int32 || obj is UInt32)
                return CytarConvert.ToBytes(Convert.ToUInt32(obj));
            else if (obj is Int64 || obj is UInt64)
                return CytarConvert.ToBytes(Convert.ToUInt64(obj));
            else if (obj is float)
                return CytarConvert.ToBytes(Convert.ToSingle(obj));
            else if (obj is double)
                return CytarConvert.ToBytes(Convert.ToDouble(obj));
            else if (obj is char)
            {
                var data = Encoding.UTF8.GetBytes(obj.ToString());
                return data;
                //return Combine(CytarConvert.NumberToBytes((byte)data.Length), data);
            }
            else if (obj is string)
            {
                var data = Encoding.UTF8.GetBytes(obj as string);
                return Combine(CytarConvert.ToBytes(data.Length), data);
            }
            else if (obj is Array)
            {
                var dataList = new List<byte[]>();
                foreach(var slice in (obj as Array))
                {
                    dataList.Add(SerializeToBytes(slice));
                }
                dataList.Insert(0, CytarConvert.ToBytes(dataList.Count));
                var dataCombined = Combine(dataList.ToArray());
                return dataCombined;
                //return Combine(CytarConvert.NumberToBytes(dataCombined.Length), dataCombined);
            }
            else
            {
                //var attribute = obj.GetType().GetCustomAttributes(true).Where(attr => attr is SerializableProperty).FirstOrDefault() as SerializableProperty;
                if (obj == null)
                    return new byte[] { 0 };
                var dataList = new List<byte[]>();
                var members = obj.GetType().GetMembers().Where(
                    member => member.GetCustomAttributes(true).Where(
                        attr => attr is SerializablePropertyAttribute).FirstOrDefault() != null)
                        .OrderBy(
                    member => (member.GetCustomAttributes(true).Where(
                        attr => attr is SerializablePropertyAttribute).FirstOrDefault() as SerializablePropertyAttribute).Index).ToArray();
                foreach (var mb in members)
                {
                    if (mb.MemberType == MemberTypes.Field)
                        dataList.Add(SerializeToBytes((mb as FieldInfo).GetValue(obj)));
                    else if (mb.MemberType == MemberTypes.Property)
                        dataList.Add(SerializeToBytes((mb as PropertyInfo).GetValue(obj)));
                    else
                        throw new SerializeException("Type error.");
                }
                // Add a symbol to indicate not null
                dataList.Insert(0, new byte[] { 1 });
                return Combine(dataList.ToArray());

                /*var fields = obj.GetType().GetFields().Where(
                    field => field.GetCustomAttributes(true).Where(
                        attr => attr is SerializablePropertyAttribute).FirstOrDefault() != null)
                        .OrderBy(
                    field => (field.GetCustomAttributes(true).Where(
                        attr => attr is SerializablePropertyAttribute).FirstOrDefault() as SerializablePropertyAttribute).Index).ToArray();
                foreach (var field in fields)
                {
                    dataList.Add(SerializeToBytes(field.GetValue(obj)));
                }
                dataList.Insert(0, CytarConvert.NumberToBytes(dataList.Count));
                return Combine(dataList.ToArray());*/
            }
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
            for(var i = 0; i < data.Length; i++)
            {
                for(var j = 0;j<(data[i] as byte[]).Length; j++)
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
            for(var i = 0; i < data.Length; i++)
            {
                data[i].CopyTo(buffer, idx);
                idx += data[i].Length;
            }
            return buffer;
        }
    }

}
