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
        public Bill SubBill;

        [SerializableProperty(0)]
        public int ID = 1 ;

        [SerializableProperty(10)]
        public string Name { get; set; }

        [SerializableProperty(100)]
        public int[] OrderList = new int[] { 1, 2, 3, 4, 5, 6 };

        [SerializableProperty(1000)]
        public long Cost = long.MaxValue;


        public Bill()
        {
            Name = "Bill";
        }
        public Bill(string name)
        {
            Name = name;
            SubBill = new Bill();
        }
    }
}
