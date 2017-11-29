using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class DataSizeException: InvalidDataException
    {
        public int RequireSize { get; private set; }

        public int LimitSize { get; private set; }

        public int ActualSize { get; private set; }
        public DataSizeException(byte[] data, int requireSize):base(data,"Invalid size of data.")
        {
            RequireSize = requireSize;
        }

        public DataSizeException(int limitSize, int actualSize) : base(null, "Invalid size of data.")
        {
            LimitSize = limitSize;
            ActualSize = actualSize;
        }
    }
}
