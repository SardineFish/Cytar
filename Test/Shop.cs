using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cytar;

namespace Test
{
    public class Shop:APIContext
    {
        [CytarAPIAttribute("fruit")]
        public Shelf FruitsShelf = new Shelf();
        [CytarAPIAttribute("books")]
        public Shelf BooksShelf = new Shelf();
        [CytarAPIAttribute("meat")]
        public Shelf MeatShelf = new Shelf();

        [CytarAPIAttribute("TTCst")]
        public int TotalCost(int[] goods)
        {
            int sum = 0;
            foreach (var value in goods)
                sum += value;
            return sum;
        }

        [CytarAPIAttribute("Pay")]
        public void Pay(int money)
        {

        }

        public Shop()
        {
            this.Children.Add(FruitsShelf);
            this.Children.Add(BooksShelf);
            this.Children.Add(MeatShelf);
        }
    }
}
