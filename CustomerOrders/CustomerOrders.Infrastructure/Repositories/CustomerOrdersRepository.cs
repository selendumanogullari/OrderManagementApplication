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
                var customerOrders = await _context.CustomerOrders.Where(co => co.CustomerId == customerId).Include(co=>co.OrderProducts).ToListAsync(); 
                return customerOrders;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<CustomerOrders.Core.Entities.CustomerOrders>> GetByCustomerOrdersWithIdAsync(int customerOrderId)
        {
            try
            {
                var customerOrders = await _context.CustomerOrders
                  .Where(co => co.Id == customerOrderId)
                  .Include(co => co.OrderProducts)
                      .ThenInclude(op => op.Product) // OrderProducts içindeki Product detaylarını da getir
                  .ToListAsync();

                return customerOrders;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
