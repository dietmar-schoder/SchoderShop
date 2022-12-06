using SchoderShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoderShopUnitTests.TestModels
{
    public class TestProduct : IProduct
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string ImageFileUrl { get; set; }
        public int PriceAsInteger { get; set; }
        public string Currency { get; set; }
    }
}
