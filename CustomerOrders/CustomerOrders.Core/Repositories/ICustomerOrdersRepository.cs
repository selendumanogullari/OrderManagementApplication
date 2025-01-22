using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomerOrders.Core.Repositories.Base.IRepository;

namespace CustomerOrders.Core.Repositories
{
    public interface ICustomerOrdersRepository : IRepository<CustomerOrders.Core.Entities.CustomerOrders>
    {
        Task<List<CustomerOrders.Core.Entities.CustomerOrders>> GetByCustomerAndOrderAsync(int customerId);
        Task<List<CustomerOrders.Core.Entities.CustomerOrders>> GetByCustomerOrdersWithIdAsync(int customerOrderId);

    }
}
