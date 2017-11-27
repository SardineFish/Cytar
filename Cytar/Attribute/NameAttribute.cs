using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cytar
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class NameAttribute : System.Attribute
    {
        
        public NameAttribute(string name)
        {
            if (name.Length > 5)
                throw new Exception("Over 5 chars.");
            if (!Regex.IsMatch(name, @"^[a-zA-Z0-9_-]+$"))
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
