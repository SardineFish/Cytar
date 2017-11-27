using System;
using System.Collections.Generic;
using System.Text;

namespace EasyRoute
{
    public class MemberNotFoundException: Exception
    {
        public RoutableObject Object { get; private set; }
        public String Name { get; private set; }
        public MemberNotFoundException(RoutableObject obj, string name):base("Member ["+name +"] not found.")
        {
            Object = obj;
            Name = name;
        }
    }
}
