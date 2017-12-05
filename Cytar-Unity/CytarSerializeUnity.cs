using System;
using System.Collections.Generic;
using System.Text;
using Cytar;
using Cytar.Serialization;
using UnityEngine;
using Cytar.IO;

namespace Cytar.Unity
{
    public static class CytarSerializeUnity
    {
        internal static void ExtentToCytar()
        {
            CytarSerialize.ExtendSerialize(typeof(Vector2), Vector2Serialize);
            CytarSerialize.ExtendSerialize(typeof(Vector3), Vector3Serialize);
            CytarSerialize.ExtendSerialize(typeof(Vector4), Vector4Serialize);
        }
        public static byte[] Vector2Serialize(object obj)
        {
            var vector2 = (Vector2)obj;
            return Combine(new byte[][] { CytarConvert.ToBytes(vector2.x), CytarConvert.ToBytes(vector2.y) });
        }
        
        public static byte[] Vector3Serialize(object obj)
        {
            Vector3 vector3 = (Vector3)obj;
            return Combine(new byte[][] { CytarConvert.ToBytes(vector3.x), CytarConvert.ToBytes(vector3.y), CytarConvert.ToBytes(vector3.z) });
        }

        public static byte[] Vector4Serialize(object obj)
        {
            Vector4 vector4 = (Vector4)obj;
            return Combine(new byte[][] {
                CytarConvert.ToBytes(vector4.x),
                CytarConvert.ToBytes(vector4.y),
                CytarConvert.ToBytes(vector4.z),
                CytarConvert.ToBytes(vector4.w),
            });
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
    }
}
