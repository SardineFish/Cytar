using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class SerializeException: Exception
    {
        public SerializeException(string msg) : base(msg) { }
    }
}
