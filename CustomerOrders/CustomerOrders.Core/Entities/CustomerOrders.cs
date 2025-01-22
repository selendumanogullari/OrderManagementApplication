using CustomerOrders.Core.Entities.Base;
using System.Text.Json.Serialization;

namespace CustomerOrders.Core.Entities
{
    public class CustomerOrders : Entity
    {
        public int CustomerId { get; set; }
        public int Status { get; set; }
        public decimal TotalAmount { get; set; }
        

        [JsonIgnore]
        public Customer Customer { get; set; }
        public List<OrderProduct> OrderProducts { get; set; } // Bir siparişin içinde birden fazla ürün olabilir

    }
    public class OrderProduct
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }  // Ürün bilgisi
        public int CustomerOrderId { get; set; } // Siparişi referans eder
        public int Quantity { get; set; } // Birim fiyat
        public Product Product { get; set; } // Birim fiyat

        [JsonIgnore]
        public CustomerOrders CustomerOrder { get; set; } // Sipariş ilişkisi
    }
}
