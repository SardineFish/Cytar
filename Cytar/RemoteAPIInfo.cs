using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class RemoteAPIInfo
    {
        public RemoteAPIInfo(string name, int callingID) : this(name, callingID, typeof(void), null, null)
        {
        }

        public RemoteAPIInfo(string name, int callingID, Type returnType, Action<object> returnCallback) : this(name, callingID, returnType, returnCallback, null)
        {
        }

        public RemoteAPIInfo(string name, int callingID, Type returnType, Action<object> returnCallback, Action<RemoteException> errorCallback)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CallingID = callingID;
            ReturnType = returnType;
            ReturnCallback = returnCallback;
            ErrorCallback = errorCallback;
        }

        public string Name { get; private set; }
        public int CallingID { get; private set; }
        public Type ReturnType { get; private set; }
        public Action<object> ReturnCallback { get; private set; }

        public Action<RemoteException> ErrorCallback { get; private set; }

    }
}
