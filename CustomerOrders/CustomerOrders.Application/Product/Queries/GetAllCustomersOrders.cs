using CustomerOrders.Application.CustomerOrder.Responses;
using MediatR;

namespace Customer.Application.CreateCustomer.Queries;

public class GetAllCustomersOrdersCommand : IRequest<IEnumerable<CreateCustomerOrdersResponse>>
{
    public string CustomerName { get; set; }

    public GetAllCustomersOrdersCommand(string customerName)
    {
        CustomerName = customerName;
    }
}