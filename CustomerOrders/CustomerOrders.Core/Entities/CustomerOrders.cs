using CustomerOrders.Core.Entities.Base;
using System.Text.Json.Serialization;

namespace CustomerOrders.Core.Entities
{
    public class CustomerOrders : Entity
    {
        public int CustomerId { get; set; }
        //public int ProductId { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }
        public List<OrderItem> OrderItems { get; set; } // Bir siparişin içinde birden fazla ürün olabilir
        //public ICollection<Product> Product { get; set; }

    }
    public class OrderItem 
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }  // Ürün bilgisi
        public int CustomerOrderId { get; set; } // Siparişi referans eder
        public Product Product { get; set; } // Ürün ilişkisi

        [JsonIgnore]
        public CustomerOrders CustomerOrder { get; set; } // Sipariş ilişkisi
    }
}
