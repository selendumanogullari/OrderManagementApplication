using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Application.RabbitMQ.Interfaces;
using CustomerOrders.Core.Entities;
using CustomerOrders.Core.Repositories;
using MediatR;
using Newtonsoft.Json;

namespace CustomerOrders.Application.CustomerOrder.Handlers
{
    public class CreateCustomerOrdersCommandHandler : IRequestHandler<CreateCustomerOrdersCommand, CreateCustomerOrdersResponse>
    {
        public readonly ICustomerOrdersRepository _customerRepository;
        private readonly RabbitMQProducer _rabbitMQProducer;


        public CreateCustomerOrdersCommandHandler(ICustomerOrdersRepository customerRepository)
        {
            _customerRepository = customerRepository;
            _rabbitMQProducer = new RabbitMQProducer();
        }
        public async Task<CreateCustomerOrdersResponse> Handle(CreateCustomerOrdersCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var customer = _customerRepository.GetByIdAsync(command.CustomerId);
                // Müşteri kontrolü yapalım

                if (customer == null)
                {
                    throw new ApplicationException("Müşteri bulunamadı");
                }

                // Yeni sipariş nesnesi oluşturuyoruz
                var customerOrder = new CustomerOrders.Core.Entities.CustomerOrders
                {
                    //CustomerId = command.CustomerId,
                    //Status = command.Status,
                    //Description = command.Description,
                    //OrderItems = command.OrderItems.Select(item => new OrderItem
                    //{
                    //    ProductId = item.ProductId,  // Ürün ID'si
                    //}).ToList()  // Siparişin içindeki ürünler
                };
                // Siparişi veritabanına ekliyoruz
                await _customerRepository.AddAsync(customerOrder);

                // Siparişin içerisindeki ürünlerin bilgilerini almak için OrderItems'ı ekliyoruz
                var response = new CreateCustomerOrdersResponse
                {
                    //CustomerId = customerOrder.CustomerId,
                    //Status = customerOrder.Status,
                    //Description = customerOrder.Description,
                    //OrderItems = customerOrder.OrderItems.Select(item => new CreateOrderItemResponse
                    //{
                    //    ProductId = item.ProductId,
                    //    //ProductName = item.Product.Name,  // Ürün adı
                    //    //Price = item.Product.Price // Ürün fiyatı
                    //}).ToList()
                };

                var orderMessage = JsonConvert.SerializeObject(customerOrder);

                _rabbitMQProducer.PublishOrderMessage(orderMessage);

                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Sipariş oluşturulurken bir hata oluştu", ex);
            }
        }
    }
}
