using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;
using Cytar.Serialization;

namespace Test
{
    public class Foo
    {
        [SerializableProperty(0)]
        public int Number = 10;
        [SerializableProperty(1)]
        public string Text { get; set; }
        [SerializableProperty(2)]
        public double[] List { get; set; }
        [SerializableProperty(3)]
        public Bar obj = new Bar();

        public Foo()
        {
            //Text = "";
        }

        public class Bar
        {
            [SerializableProperty(0)]
            public char[] Emmmm = new char[] { 'E', 'm', 'm', 'm', 'm', 'm' };
        }
    }
}
