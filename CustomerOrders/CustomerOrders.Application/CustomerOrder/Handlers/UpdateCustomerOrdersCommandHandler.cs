using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Core.Entities;
using CustomerOrders.Core.Repositories;
using MediatR;

namespace CustomerOrders.Application.CustomerOrder.Handlers
{
    public class UpdateCustomerOrdersCommandHandler : IRequestHandler<UpdateCustomerOrdersCommand, UpdateCustomerOrdersResponse>
    {
        private readonly ICustomerOrdersRepository _customerOrdersRepository;

        public UpdateCustomerOrdersCommandHandler(ICustomerOrdersRepository customerOrdersRepository)
        {
            _customerOrdersRepository = customerOrdersRepository;
        }

        public async Task<UpdateCustomerOrdersResponse> Handle(UpdateCustomerOrdersCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var customerOrders = await _customerOrdersRepository.GetByCustomerOrdersWithIdAsync(command.CustomerOrderId);

                foreach (var customerOrder in customerOrders)
                {
                    foreach (var orderProductDto in command.OrderProducts)
                    {
                        var existingProduct = customerOrder.OrderProducts
                            .FirstOrDefault(op => op.ProductId == orderProductDto.ProductId);

                        if (existingProduct != null)
                        {
                            if (existingProduct.Product.Quanttity >= orderProductDto.Quantity)
                            {
                                existingProduct.Quantity = (int)orderProductDto.Quantity;

                                customerOrder.TotalAmount = (decimal)customerOrder.OrderProducts
                                 .Sum(item => item.Quantity * existingProduct.Product.Price);

                            }
                            else
                            {
                                throw new ApplicationException(
                                        $"Stok yetersiz. Ürün: {existingProduct.Product.Name}, Mevcut Stok: {existingProduct.Product.Quanttity}, Talep Edilen: {orderProductDto.Quantity}");
                            }
                        }
                        else
                        {
                            var firstOrder = customerOrders.FirstOrDefault();
                            if (firstOrder != null)
                            {
                                firstOrder.OrderProducts.Add(new OrderProduct
                                {
                                    ProductId = (int)orderProductDto.ProductId,
                                    Quantity = (int)orderProductDto.Quantity
                                });
                            }
                            throw new ApplicationException(
                                       $"Güncellemek istediğiniz ürün bu siparişte bulunamadı: {orderProductDto.ProductId}, Dilerseniz yeni sipariş oluşturupi, güncelleyebilirsiniz");
                        }
                    }
                    await _customerOrdersRepository.UpdateAsync(customerOrder);

                }

                var response = new UpdateCustomerOrdersResponse
                {
                    CustomerId = command.CustomerId,
                    CustomerOrderId = command.CustomerOrderId,
                    Message = "Sipariş durumu başarıyla güncellendi."
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Sipariş güncellenirken bir hata oluştu", ex);
            }
        }
    }
}
