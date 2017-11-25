using System;
using System.Collections.Generic;
using System.Text;

namespace CytarMP
{
    public class User: IDObject
    {
        public string Name { get; protected set; }

        public User(string name)
        {
            Name = name;
        }
    }
}
