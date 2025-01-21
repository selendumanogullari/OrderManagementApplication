using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerOrders.Application.CustomerOrder.Responses
{
    public class UpdateCustomerOrdersResponse
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public string Message { get; set; }
    }
}
