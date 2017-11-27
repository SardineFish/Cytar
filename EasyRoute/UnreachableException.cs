using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRoute
{
    public class UnreachableException: Exception
    {
        public string Path { get; private set; }
        public UnreachableException(string path) : base("Rnreachable path " + path)
        {
            Path = path;
        }
    }
}
