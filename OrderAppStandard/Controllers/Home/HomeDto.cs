using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderApp.Controllers.Home
{
    public class HomeDto
    {
        public class CompanyDto
        {
            public string CompanyName { get; set; }
            public string Slug { get; set; }
            public string Address { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Summary { get; set; }
        }
        public class CreateOrderDto
        {
            public CreateOrderDto()
            {
                Order = new OrderDto();
                OrderDetails = new List<OrderDetailDto>();
            }
            public OrderDto Order { get; set; }
            public List<OrderDetailDto> OrderDetails { get; set; }
        }
        public class OrderDto
        {
            public int OrderId { get; set; }
            public int TableId { get; set; }
            public Guid? TableToken { get; set; }
        }
        public class OrderDetailDto
        {
            public int OrderDetailId { get; set;}
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public string ShoppingCartNote { get; set; }
            public int ShoppingCartQuantity { get; set; }
            public decimal ProductPrice { get; set; }
        }
    }
}