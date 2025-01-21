using CustomerOrders.Application.Product.Commands;
using CustomerOrders.Application.Product.Mappers;
using CustomerOrders.Application.Product.Responses;
using CustomerOrders.Core.Repositories;
using MediatR;
using Serilog;

namespace CustomerOrders.Application.Product.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, UpdateProductResponse>
    {
        public readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public CreateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _logger = Log.ForContext<CreateProductCommandHandler>();
        }

        public async Task<UpdateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.Information("Handling request: {@Command} at {Timestamp}", command, DateTime.UtcNow);
                var product = ProductMapper.Mapper.Map<CustomerOrders.Core.Entities.Product>(command);

                if (product != null)
                {
                    var productResponse = ProductMapper.Mapper.Map<UpdateProductResponse>(product);
                    _logger.Information("Handled response: {@Response} at {Timestamp}", productResponse, DateTime.UtcNow);
                    stopwatch.Stop();
                    _logger.Information("Handled request in {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
                    await _productRepository.AddAsync(product);
                    return productResponse;
                }
                else
                {
                    throw new ApplicationException("There is no exist product");
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(ex, "Error handling request: {@Command} at {Timestamp}, elapsed time: {ElapsedMilliseconds} ms",
                    command, DateTime.UtcNow, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
