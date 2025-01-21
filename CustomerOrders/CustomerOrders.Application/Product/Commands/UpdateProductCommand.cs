using CustomerOrders.Application.Product.Responses;
using MediatR;

namespace CustomerOrders.Application.Product.Commands
{
    public class UpdateProductCommand : IRequest<UpdateProductResponse>
    {
        public string Name { get; set; }
        public int Quanttity { get; set; }
        public int Price { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
    }
}
