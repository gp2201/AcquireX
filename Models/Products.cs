using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcquireX.Models
{
    public static class Products
    {
        public class ProductList
        { 
            public List<Product> products { get; set; }
        }

        public class Product
        {
            public string ItemCode { get; set; }
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public string? Upc { get; set; }
            public string? Mpn { get; set; }
            public double Price { get; set; }
            public string? Brand { get; set; }
        }
    }
}
