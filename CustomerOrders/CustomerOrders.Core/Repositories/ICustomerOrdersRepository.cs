
using static CustomerOrders.Core.Repositories.Base.IRepository;

namespace CustomerOrders.Core.Repositories
{
    public interface ICustomerOrdersRepository : IRepository<CustomerOrders.Core.Entities.CustomerOrders>
    {
        Task<List<CustomerOrders.Core.Entities.CustomerOrders>> GetByCustomerAndOrderAsync(int customerId);
    }
}