using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerOrders.Application.CustomerOrder.Responses
{
    public class CreateCustomerOrdersResponse
    {
        public int CustomerId { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public List<CreateOrderItemResponse> OrderItems { get; set; } // Siparişteki ürünler
    }

    public class CreateOrderItemResponse
    {
        public int ProductId { get; set; }  // Ürün ID'si
        //public string ProductName { get; set; }  // Ürün adı (isteğe bağlı)
        //public decimal Price { get; set; }  // Ürün fiyatı (isteğe bağlı)
    }
}
