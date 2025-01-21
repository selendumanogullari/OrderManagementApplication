using AutoMapper;
using CustomerOrders.Application.Product.Commands;
using CustomerOrders.Application.Product.Responses;

namespace CustomerOrders.Application.Product.Mappers
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<CustomerOrders.Core.Entities.Product, CreateProductCommand>();
            CreateMap<CustomerOrders.Core.Entities.Product, UpdateProductResponse>();
        }
    }
}
