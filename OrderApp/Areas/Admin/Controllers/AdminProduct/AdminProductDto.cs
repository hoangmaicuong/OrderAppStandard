using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Areas.Admin.Controllers.AdminProduct
{
    public class AdminProductDto
    {
        public class UpdateDto
        {
            public UpdateDto()
            {
                Product = new ProductDto();
            }
            public ProductDto Product { get; set; }
        }
        public class ProductDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductDescription { get; set; }
            public Nullable<decimal> ProductPrice { get; set; }
            public Nullable<decimal> OriginalPrice { get; set; }
            public Nullable<bool> IsAvailable { get; set; }
            public int NumberOfProductsSold { get; set; }
        }
    }
}