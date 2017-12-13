using System;
using System.Collections.Generic;
using System.Text;
using Cytar;
using Cytar.Serialization;
using UnityEngine;
using System.IO;
using Cytar.IO;

namespace Cytar.Unity
{
    public static class CytarDeserializeUnity
    {
        internal static void ExtentToCytar()
        {
            CytarDeserialize.ExtendDeserialize(typeof(Vector2), Vector2Deserialize);
            CytarDeserialize.ExtendDeserialize(typeof(Vector3), Vector3Deserialize);
            CytarDeserialize.ExtendDeserialize(typeof(Vector4), Vector4Deserialize);
        }

        public static object Vector2Deserialize(Stream stream)
        {
            CytarStreamReader cr = new CytarStreamReader(stream);
            return new Vector2(cr.ReadSingle(), cr.ReadSingle());
        }

        public static object Vector3Deserialize(Stream stream)
        {
            CytarStreamReader cr = new CytarStreamReader(stream);
            return new Vector3(cr.ReadSingle(), cr.ReadSingle(),cr.ReadSingle());
        }

        public static object Vector4Deserialize(Stream stream)
        {
            CytarStreamReader cr = new CytarStreamReader(stream);
            return new Vector4(cr.ReadSingle(), cr.ReadSingle(), cr.ReadSingle(), cr.ReadSingle());
        }
    }
}
