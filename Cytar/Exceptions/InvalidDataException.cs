using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class InvalidDataException: Exception
    {
        public byte[] Data { get; private set; }
        public InvalidDataException(byte[] data) : base("Invalid data.")
        {
            Data = data;
        }
        public InvalidDataException(byte[] data, string msg) : base(msg)
        {
            Data = data;
        }
    }
}
