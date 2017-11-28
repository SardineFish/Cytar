using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class DeserializeException:Exception
    {
        public DeserializeException(string msg):base(msg)
        {

        }
    }
}
