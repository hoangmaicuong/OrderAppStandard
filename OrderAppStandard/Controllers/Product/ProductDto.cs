using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Controllers.Product
{
    public class ProductDto
    {
        public class ProductItemDto
        {
            public ProductItemDto()
            {
                ProductItemOfProductTypeItems = new List<ProductItemOfProductTypeItemDto>();
            }
            public int ProductItemId { get; set; }
            public Nullable<int> ProductId { get; set; }
            public int ProductQuantity { get; set; }
            public List<ProductItemOfProductTypeItemDto> ProductItemOfProductTypeItems { get; set; }
        }
        public class ProductItemOfProductTypeItemDto
        {
            public int ProductItemId { get; set; }
            public int ProductTypeItemId { get; set; }
        }
        public class ProductTypeItemDto
        {
            public int ProductTypeItemId { get; set; }
        }
    }
}