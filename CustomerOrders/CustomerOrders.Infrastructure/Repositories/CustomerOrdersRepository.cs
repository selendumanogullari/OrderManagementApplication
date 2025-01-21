using CustomerOrders.Core.Repositories;
using CustomerOrders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrders.Infrastructure.Repositories
{
    public class CustomerOrdersRepository : Repository<CustomerOrders.Core.Entities.CustomerOrders>, ICustomerOrdersRepository
    {
        public CustomerOrdersRepository(DatabaseContext bookContext) : base(bookContext)
        {
        }
        public async Task<List<CustomerOrders.Core.Entities.CustomerOrders>> GetByCustomerAndOrderAsync(int customerId)
        {
            try
            {
                var customerOrders = await _context.CustomerOrders.Where(co => co.CustomerId == customerId).ToListAsync(); 
                return customerOrders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
