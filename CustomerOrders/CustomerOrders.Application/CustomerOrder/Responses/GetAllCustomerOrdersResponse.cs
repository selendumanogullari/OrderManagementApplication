using MediatR;

namespace CustomerOrders.Application.CustomerOrder.Responses
{
    public class GetAllCustomersOrdersResponse : IRequest<IEnumerable<CreateCustomerOrdersResponse>>
    {
        public string CustomerName { get; set; }

        public GetAllCustomersOrdersResponse(string customerName)
        {
            CustomerName = customerName;
        }
    }
}
