using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class DataSizeException: InvalidDataException
    {
        public int RequireSize { get; private set; }
        public DataSizeException(byte[] data, int requireSize):base(data,"Invalid size of data.")
        {
            RequireSize = requireSize;
        }
    }
}
