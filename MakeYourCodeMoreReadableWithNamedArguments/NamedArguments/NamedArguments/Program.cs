using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedArguments
{
    public class Product
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal UnitsInStock { get; set; }

        public Product(string name, decimal price, decimal unitsInStock)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.Price = (price > 0) ? price : throw new ArgumentException(paramName: nameof(price), message: "Price should be a positive value");
            this.UnitsInStock = unitsInStock;
        }
    }

    internal class Program
    {
        public static void HardToRead()
        {
            var type = Type.GetType("System.Random", false);

            var product = new Product("Nutella", 5, 4);

        }

        public static void EasyToRead()
        {
            var type = Type.GetType("System.Random", throwOnError: false);
            var product = new Product("Nutella", price: 5, unitsInStock: 4);
            //product = new Product("Nutella", 0, 0);
            product = new Product("Nutella", price: 0, 0);
        }

        private static void Main(string[] args) { HardToRead(); EasyToRead(); }
    }
}
