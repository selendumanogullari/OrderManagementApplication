using CustomerOrders.Application.CustomerOrder.Commands;
using CustomerOrders.Application.CustomerOrder.Responses;
using CustomerOrders.Application.RabbitMQ.Interfaces;
using CustomerOrders.Core.Entities;
using CustomerOrders.Core.Repositories;
using CustomerOrders.Infrastructure.Data;
using MediatR;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrders.Application.CustomerOrder.Handlers
{
    public class CreateCustomerOrdersCommandHandler : IRequestHandler<CreateCustomerOrdersCommand, CreateCustomerOrdersResponse>
    {
        public readonly ICustomerOrdersRepository _customerRepository;
        private readonly RabbitMQProducer _rabbitMQProducer;
        private readonly DatabaseContext _context;


        public CreateCustomerOrdersCommandHandler(ICustomerOrdersRepository customerRepository, DatabaseContext context)
        {
            _customerRepository = customerRepository;
            _rabbitMQProducer = new RabbitMQProducer();
            _context = context;
        }
        public async Task<CreateCustomerOrdersResponse> Handle(CreateCustomerOrdersCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(command.CustomerId);
                // Müşteri kontrolü yapalım

                if (customer == null)
                {
                    throw new ApplicationException("Müşteri bulunamadı");
                }
                var customerOrder = new Core.Entities.CustomerOrders
                {
                    CustomerId = command.CustomerId,
                    Status = command.Status,
                    Description = command.Description,
                    OrderProducts = command.OrderProducts.Select(item => new OrderProduct
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity // Ürün ID'si ve Miktar
                    }).ToList(),  // Siparişin içindeki ürünler
                    TotalAmount = command.OrderProducts
                     .Sum(item => item.Quantity * _context.Products
                    .FirstOrDefault(p => p.Id == item.ProductId)?.Price ?? 0) // Fiyatları çarpıyoruz ve topluyoruz
                };

                // Siparişi veritabanına ekliyoruz
                //customerOrder.TotalAmount = customerOrder.OrderProducts.Sum(op => op.Price);

                await _customerRepository.AddAsync(customerOrder);

                // Siparişin içerisindeki ürünlerin bilgilerini almak için OrderItems'ı ekliyoruz
                var response = new CreateCustomerOrdersResponse
                {
                    CustomerId = customerOrder.CustomerId,
                    Status = customerOrder.Status,
                    Description = customerOrder.Description,
                    OrderProducts = customerOrder.OrderProducts.Select(item => new CreateOrderProductsResponse
                    {
                        ProductId = item.ProductId,
                        //ProductName = item.Product.Name,  // Ürün adı
                        //Price = item.Product.Price, // Ürün fiyatı
                        Quantity = item.Quantity // Ürün fiyatı
                    }).ToList(),
                    TotalAmount = customerOrder.TotalAmount
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
