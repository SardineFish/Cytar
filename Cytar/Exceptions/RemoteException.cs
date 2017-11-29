using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class RemoteException: Exception
    {
        public int ErrorCode { get; private set; }
        public RemoteException(string msg, int errorCode = -1) : base(msg)
        {
            ErrorCode = errorCode;
        }
    }
}
