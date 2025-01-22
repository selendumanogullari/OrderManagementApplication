using CustomerOrders.Application.CustomerOrder.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerOrders.Application.CustomerOrder.Commands
{
    public class CreateCustomerOrdersCommand : IRequest<CreateCustomerOrdersResponse>
    {
        public int CustomerId { get; set; }  // Siparişi veren müşteri
        public int Status { get; set; }  // Sipariş durumu
        public string Description { get; set; }  // Sipariş açıklaması

        // Siparişin içindeki ürünlerin listesi
        public List<CreateOrderProductCommand> OrderProducts { get; set; }  // Her bir ürünün sipariş adedi ve ürünü

        [JsonIgnore]
        public decimal TotalAmount { get; set; } // Toplam tutar hesaplanacak
       
    }

    public class CreateOrderProductCommand
    {
        public int ProductId { get; set; }  // Ürünün ID'si
        public int Quantity { get; set; }  // Siparişteki ürün adedi
    }
}
