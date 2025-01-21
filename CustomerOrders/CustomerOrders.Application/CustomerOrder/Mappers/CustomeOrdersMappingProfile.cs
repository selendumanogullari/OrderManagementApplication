using AutoMapper;
using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;

namespace CustomerOrders.Application.CustomerOrder.Mappers
{
    public class CustomerOrdersMappingProfile : Profile
    {
        public CustomerOrdersMappingProfile()
        {
            CreateMap<CustomerOrders.Core.Entities.Customer, CreateCustomerOrdersResponse>().ReverseMap();
            CreateMap<CustomerOrders.Core.Entities.Customer, CreateCustomerOrdersCommand>().ReverseMap();
        }
    }
}
