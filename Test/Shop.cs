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
        [CytarAPI("fruit")]
        public Shelf FruitsShelf = new Shelf();
        [CytarAPI("books")]
        public Shelf BooksShelf = new Shelf();
        [CytarAPI("meat")]
        public Shelf MeatShelf = new Shelf();

        [CytarAPI("TTCst")]
        public int TotalCost(int[] goods)
        {
            int sum = 0;
            foreach (var value in goods)
                sum += value;
            return sum;
        }

        [CytarAPI("Pay")]
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
