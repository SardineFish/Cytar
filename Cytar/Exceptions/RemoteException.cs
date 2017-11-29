using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class RemoteException: Exception
    {
        [SerializableProperty(0)]
        public int ErrorCode { get; set; }
        [SerializableProperty(1)]
        public new string Message { get; set; }
        public RemoteException(string msg, int errorCode = -1) : base(msg)
        {
            ErrorCode = errorCode;
            Message = msg;
        }
        public RemoteException() : this("", -1) { }
    }
}
