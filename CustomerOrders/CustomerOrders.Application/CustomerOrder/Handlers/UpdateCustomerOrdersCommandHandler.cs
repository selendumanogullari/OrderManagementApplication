using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Core.Entities;
using CustomerOrders.Core.Repositories;
using MediatR;

namespace CustomerOrders.Application.CustomerOrder.Handlers
{
    public class UpdateCustomerOrdersCommandHandler : IRequestHandler<UpdateCustomerOrdersCommand, UpdateCustomerOrdersResponse>
    {
        public readonly ICustomerOrdersRepository _customerOrdersRepository;

        public UpdateCustomerOrdersCommandHandler(ICustomerOrdersRepository customerOrdersRepository, ICustomerOrdersRepository customerRepository)
        {
            _customerOrdersRepository = customerOrdersRepository;
        }
        public async Task<UpdateCustomerOrdersResponse> Handle(UpdateCustomerOrdersCommand command, CancellationToken cancellationToken)
        {
            try
            { // Öncelikle CustomerId'ye göre müşteri siparişlerini alalım
                var customerOrders = _customerOrdersRepository.GetByCustomerAndOrderAsync(command.CustomerId).Result.ToList();

                // Mevcut siparişlerin üzerinden geçelim ve OrderItems'ları güncelleyelim
                foreach (var orderItemDto in command.OrderItems)
                {
                    var existingOrder = customerOrders.FirstOrDefault(o => o.OrderItems.Any(oi => oi.Id == orderItemDto.Id));

                    if (existingOrder != null)
                    {
                        var existingItem = existingOrder.OrderItems.First(oi => oi.Id == orderItemDto.Id);
                        existingItem.ProductId = orderItemDto.ProductId;
                    }
                    else
                    {
                        // Yeni item ekleme (müşterinin mevcut siparişlerinden birine)
                        customerOrders.First().OrderItems.Add(new OrderItem { Id = orderItemDto.Id, ProductId = orderItemDto.ProductId });
                    }
                }

                // Silinecek itemları belirleyin
                foreach (var customerOrder in customerOrders)
                {
                    var itemsToRemove = customerOrder.OrderItems.Where(oi => !command.OrderItems.Any(dto => dto.Id == oi.Id)).ToList();
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        customerOrder.OrderItems.Remove(itemToRemove);
                    }
                }
                foreach (var customerOrder in customerOrders)
                {
                    customerOrder.Description = command.Description;
                    customerOrder.Status = command.Status;
                    await _customerOrdersRepository.UpdateAsync(customerOrder);
                }
                var response = new UpdateCustomerOrdersResponse
                {
                    CustomerId = command.CustomerId,
                    Message = "Sipariş durumu başarıyla güncellendi."
                };
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
