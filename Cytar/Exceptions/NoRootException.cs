using System;
using System.Collections.Generic;
using System.Text;

namespace Cytar
{
    public class NoRootException: Exception
    {
        public Session Session;
        public NoRootException(Session session): base("No root assigned.")
        {
            Session = session;
        }
    }
}
