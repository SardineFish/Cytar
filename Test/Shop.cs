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
        public Shelf FruitsShelf = new Shelf();
        public Shelf BooksShelf = new Shelf();
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
