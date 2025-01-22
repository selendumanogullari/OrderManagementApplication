using CustomerOrders.Application.CustomerOrder.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerOrders.Application.CustomerOrder.Commands
{
    public class UpdateCustomerOrdersCommand : IRequest<UpdateCustomerOrdersResponse>
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public int CustomerOrderId { get; set; }
        public IEnumerable<OrderProductDto> OrderProducts { get; set; }

        public class OrderProductDto
        {
            [JsonIgnore]
            public int Id { get; set; } // Var olan ürünün ID'si
            public int? ProductId { get; set; } // Ürün ID'si - Nullable, sadece yeni ürün ekleniyorsa gerekebilir
            public int? Quantity { get; set; } // Ürün adedi - Nullable, sadece adet güncelleniyorsa gerekebilir
        }
    }
}
