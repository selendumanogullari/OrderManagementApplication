using static CustomerOrders.Core.Repositories.Base.IRepository;

namespace CustomerOrders.Core.Repositories
{
    public interface IProductRepository : IRepository<CustomerOrders.Core.Entities.Product>
    {
        Task<CustomerOrders.Core.Entities.Product> GetByProductByNameAsync(string productName);
    }
}
