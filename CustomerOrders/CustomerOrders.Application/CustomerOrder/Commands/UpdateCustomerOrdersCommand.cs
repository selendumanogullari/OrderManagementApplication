using CustomerOrders.Application.CustomerOrder.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerOrders.Application.CustomerOrder.Commands
{
    public class UpdateCustomerOrdersCommand : IRequest<UpdateCustomerOrdersResponse>
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public IEnumerable<OrderItemDto> OrderItems { get; set; }

        public class OrderItemDto
        {
            public int Id { get; set; }
            public int ProductId { get; set; }
        }

    }
}
