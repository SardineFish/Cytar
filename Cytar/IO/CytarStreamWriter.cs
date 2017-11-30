using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cytar.Serialization;

namespace Cytar.IO
{
    public class CytarStreamWriter
    {
        public Stream Stream { get; set; }
        public CytarStreamWriter(Stream stream)
        {
            Stream = stream;
        }

        public void Write(byte[] data) => Stream.Write(data, 0, data.Length);

        public void Write(byte[] data, int offset, int count) => Stream.Write(data, offset, count);
        public void Write(byte number) => Write(new byte[] { number }, 0, 1);
        public void Write(Boolean value) => Write(CytarSerialize.Serialize(value), 0, 1);
        public void Write(Int16 number) => Write(CytarSerialize.Serialize(number), 0, 2);
        public void Write(UInt16 number) => Write(CytarSerialize.Serialize(number), 0, 2);
        public void Write(Int32 number) => Write(CytarSerialize.Serialize(number), 0, 4);
        public void Write(UInt32 number) => Write(CytarSerialize.Serialize(number), 0, 4);
        public void Write(Int64 number) => Write(CytarSerialize.Serialize(number), 0, 8);
        public void Write(UInt64 number) => Write(CytarSerialize.Serialize(number), 0, 8);
        public void Write(Single number) => Write(CytarSerialize.Serialize(number), 0, 4);
        public void Write(Double number) => Write(CytarSerialize.Serialize(number), 0, 8);

        public void Write(char ch) => Write(CytarSerialize.Serialize(ch));
        public void Write(string text) => Write(CytarSerialize.Serialize(text));
        public void Write(Array arr) => Write(CytarSerialize.Serialize(arr));
        public void Write(object obj) => Write(CytarSerialize.Serialize(obj));
    }
}
