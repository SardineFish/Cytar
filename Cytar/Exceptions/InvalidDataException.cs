using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class InvalidDataException: Exception
    {
        public byte[] Buffer { get; private set; }
        public InvalidDataException(byte[] data) : base("Invalid data.")
        {
            Buffer = data;
        }
        public InvalidDataException(byte[] data, string msg) : base(msg)
        {
            Buffer = data;
        }
    }
}
