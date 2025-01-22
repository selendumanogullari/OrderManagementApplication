using AutoMapper;
using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Application.Product.Commands;

namespace CustomerOrders.Application.CustomerOrder.Mappers
{
    public class CustomerOrdersMappingProfile : Profile
    {
        public CustomerOrdersMappingProfile()
        {

            CreateMap<CreateCustomerOrdersCommand, CustomerOrders.Core.Entities.CustomerOrders>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()); // TotalAmount backend'de hesaplanacak, ignore ediyoruz

            CreateMap<CustomerOrders.Core.Entities.CustomerOrders, CreateCustomerOrdersResponse>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount)); // backend'deki toplam tutarı response'a aktarıyoruz

            CreateMap<CustomerOrders.Core.Entities.CustomerOrders, CreateCustomerOrdersCommand>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore()); // response'dan gelen totalamount'u ignore ediyoruz

        }
    }
}
