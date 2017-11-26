using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using Cytar;

namespace Cytar
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CytarAPI : Attribute
    {
        public CytarAPI(string name)
        {
            if(!Regex.IsMatch(name, @"^[a-zA-Z0-9_-]+$"))
            {
                throw new Exception("Invalid Identifier");
            }
            Name = name;
        }

        public string Name { get; private set; }

        public UInt32 Code
        {
            get
            {
                char[] fill = new char[] { 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A' };
                Name.Replace("-", "+").Replace("_", "/").CopyTo(0, fill, 0, Name.Length);
                return CytarConvert.BytesToUInt32(Convert.FromBase64CharArray(fill, 0, 8));
            }
        }
        
    }
}