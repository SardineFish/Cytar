using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class Bill
    {
        [SerializableProperty(30)]
        public Bill SubBill = new Bill();

        [SerializableProperty(0)]
        public int ID = 1 ;

        [SerializableProperty(10)]
        public string Name { get; set; }

        public Bill()
        {
            Name = "Bill";
        }
    }
}
