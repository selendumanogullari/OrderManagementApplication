using CustomerOrders.Core.Repositories;
using CustomerOrders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrders.Infrastructure.Repositories
{
    public class ProductRepository : Repository<CustomerOrders.Core.Entities.Product>, IProductRepository
    {
        public ProductRepository(DatabaseContext bookContext) : base(bookContext)
        {
        }
        public async Task<CustomerOrders.Core.Entities.Product> GetByProductByNameAsync(string productName)
        {

            var product = await _context.Products.Where(co => co.Name == productName).FirstOrDefaultAsync();
            if (product != null)
            {
                return product;
            }
            else
            {
                throw new ApplicationException("There is no exist product with this name");
            }
        }
    }
}
