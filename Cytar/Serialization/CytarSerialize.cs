using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Cytar.Serialization
{
    public static class CytarSerialize
    {
        private static Dictionary<Type, Func<object, byte[]>> ExtendedSerialization = new Dictionary<Type, Func<object, byte[]>>();
        public static byte[] Serialize(Type type,object obj)
        {
            if (ExtendedSerialization.ContainsKey(type))
                return ExtendedSerialization[type].Invoke(obj);
            if (type == typeof(byte))
                return new byte[] { (byte)obj };
            else if (type == typeof(Boolean))
                return CytarConvert.ToBytes((Boolean)obj);
            else if (type == typeof(Int16))
                return CytarConvert.ToBytes((Int16)obj);
            else if (type == typeof(UInt16))
                return CytarConvert.ToBytes((UInt16)obj);
            else if (type == typeof(Int32))
                return CytarConvert.ToBytes((Int32)obj);
            else if (type == typeof(UInt32))
                return CytarConvert.ToBytes((UInt32)obj);
            else if (type == typeof(Int64))
                return CytarConvert.ToBytes((Int64)obj);
            else if (type == typeof(UInt64))
                return CytarConvert.ToBytes((UInt64)obj);
            else if (type == typeof(float))
                return CytarConvert.ToBytes(Convert.ToSingle(obj));
            else if (type == typeof(double))
                return CytarConvert.ToBytes(Convert.ToDouble(obj));
            else if (type == typeof(char))
            {
                var data = Encoding.UTF8.GetBytes(obj.ToString());
                return data;
                //return Combine(CytarConvert.NumberToBytes((byte)data.Length), data);
            }
            else if (type == typeof(string))
            {
                if(obj == null)
                {
                    return CytarConvert.ToBytes(-1);
                }

                var data = Encoding.UTF8.GetBytes(obj as string);
                return Combine(CytarConvert.ToBytes(data.Length), data);
            }
            else if (type.IsArray)
            {
                if(obj == null)
                {
                    return CytarConvert.ToBytes(-1);
                }

                var dataList = new List<byte[]>();
                foreach (var slice in (obj as Array))
                {
                    dataList.Add(Serialize(type.GetElementType(), slice));
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
                        dataList.Add(Serialize((mb as FieldInfo).FieldType, (mb as FieldInfo).GetValue(obj)));
                    else if (mb.MemberType == MemberTypes.Property)
                        dataList.Add(Serialize((mb as PropertyInfo).PropertyType, (mb as PropertyInfo).GetValue(obj)));
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
        public static byte[] Serialize(object obj)
        {
            return Serialize(obj.GetType(), obj);
        }

        public static void ExtendSerialize(Type type,Func<object,byte[]> serializeCallback)
        {
            ExtendedSerialization[type] = serializeCallback;
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
